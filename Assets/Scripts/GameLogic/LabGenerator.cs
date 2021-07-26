using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Генератор лабиринта
/// </summary>
public class LabGenerator : MonoBehaviour
{
    private int sizeX;
    private int sizeY;
    
    Cell cellPrefab;
    Cell startCell;

    Cell[,] cells;
    private List<Cell> unvisitedCells;
    List<Cell> visitedCells;
    
    GameManager gm;
    Board board;

    Slider _slider;
    int progressBar;
    int totalCount;
    Text _text;

    public bool complete = false;

   

    /// <summary>
    /// Инициализация генератора
    /// </summary>
    /// <param name="gm"></param>
    public void Init(GameManager gm)
    {
        this.gm = gm;
        this.board = gm.board;
        sizeX = board.SizeX;
        sizeY = board.SizeY;
        cellPrefab = gm.CellPrefab;
    }

    /// <summary>
    /// Запуск генерации с отображением состяния на прогресс баре
    /// </summary>
    public void Generate()
    {
        _slider = gm.slider;
        _text = gm.text;
        StartCoroutine(GenerateLabirint());
    }
    /// <summary>
    /// Генерация лабиринта, за основу взята статья https://habr.com/ru/post/262345/
    /// </summary>
    /// <returns></returns>
    public IEnumerator GenerateLabirint()
    {

        cells = InitializeBoard();
        // Итоговое значение для переменной прогресс бара равное 100%.
        // Вычисляется как размер поля деленный на 4, т.к. проходятся только нечетные клетки, и количество уникальных клеток 5 типов 
        totalCount = (sizeX * sizeY) / 4 + board.UnusualCellsCount * 5;
        visitedCells = new List<Cell>();

        // Генерация лабиринта начинается с нижнего левого угла
        startCell = cells[sizeX - 1, 0];

        List<Cell> finishedCell = new List<Cell>();
        List<Cell> wallsCell = new List<Cell>();
        Cell currentCell = startCell;
        
        visitedCells.Add(currentCell);
        
        Cell nextCell;
        Stack<Cell> path = new Stack<Cell>();
        
        bool notReturned = false;
        int steps = 0;
        // Максимальное количество шагов для изменения ветвления лабиринта, 10 шагов на 13 клеток ширины лабиринта
        int maxsteps = 10 * sizeX / 13;
        bool firstDeadLock = true;
        
        Debug.Log("RemoveWall");
        
        // П
        do
        {
            _slider.value = visitedCells.Count / totalCount;
            _text.text = "Генерация лабиринта";
            //Получаем список непосещенных соседей
            List<Cell> neighbours = GetUnvisitedNeighbour(currentCell);
            
            /* Если есть не посещенные соседи и количество шагов меньше порога ветвления:
             * 1.Добавляем соседей клетки в непосещенные клетки
             * 2.Выбираем случайного соседа и добавляем в путь
             * 3.Удаляем стену между текущей клеткой и выбранной.
             * 4.Перемещаемся в выбранную клетку
             * 5. Удаляем ее из непосещенных клеток и добавляем в посещенные
             * 6. Устанавливаем флаг не возврата
             * 7. Увеличиваем количество шагов
             */
            if (neighbours.Count != 0 && steps < maxsteps)
            {
                AddNeighbourToUnvisitedCells(currentCell);
                int rand = Random.Range(0, 100) % (neighbours.Count);
                nextCell = neighbours[rand];
                path.Push(currentCell);
                RemoveWall(currentCell, nextCell);
                currentCell = nextCell;
                unvisitedCells.Remove(currentCell);
                visitedCells.Add(currentCell);
                notReturned = true;
                steps+= 2;
            }
            /* Если соседей нет:
             * 1. Если это не клетка возврата и не первый тупик добавляем в список финишных(тупиковых) клеток.
             * 2. Возвращаемся в предыдущую клетку и устанавливаем флаг возврата
             */

            else if (path.Count > 0)
            {
                if(notReturned && !firstDeadLock)
                {
                    finishedCell.Add(currentCell);
                }
                if (firstDeadLock)
                    firstDeadLock = false;
                currentCell = path.Pop();
                notReturned = false;
                steps = 0;
            }
            // Если вернулись в начало, выбираем случайную непосещенную клетку 
            else
            {
                int rand = Random.Range(0, unvisitedCells.Count - 1);
                currentCell = unvisitedCells[rand];
            }
            yield return null;
        }
        // Повторяем пока есть непосещенные клетки
        while (unvisitedCells.Count > 0);

        // Удаляем стены
        Debug.Log("RemoveRandomWall");
        RemoveRandomWalls();
        yield return null;
        
        // Размещаем клетки победы и проигрыша
        SetupFinishAndLoseCells(finishedCell);
        _slider.value += board.UnusualCellsCount / totalCount;
        _text.text = "Размещение куриц";
        yield return null;

        // Размещаем клетки нор
        SetupHoleCells();
        _slider.value += board.UnusualCellsCount / totalCount;
        _text.text = "Размещение нор";
        yield return null;
        
        // Размещаем клетки с картой
        SetupCellsOfType(CellType.Map);
        _slider.value += board.UnusualCellsCount / totalCount;
        _text.text = "Размещение карт";
        yield return null;
        
        // Размещаем клетки начала следов
        SetupCellsOfType(CellType.Path);
        _slider.value += board.UnusualCellsCount / totalCount;
        _text.text = "Размещение путей";
        complete = true;
        yield return null;
        
        // Для клеток со следами устанавливаем клетки назначения
        foreach (CellWithPath c in gm.board.CellsWithPath)
        {
            c.SetTargetPoint(board);
        }

        // Активируем игровую доску
        board.gameObject.SetActive(true);

        // Скрываем полосу прогресса
        gm.progress.gameObject.SetActive(false);

        // Активируем кнопку меню
        FindObjectOfType<Menu>().GameMenuButton.SetActive(true);
        
        // Снимаем игру с паузы
        gm.Pause = false;
    }

    /// <summary>
    /// Размещение клеток с норами
    /// </summary>
    void SetupHoleCells()
    {
        // Устанавливаем радиус исключения
        int excludeRange = board.MapRange;
        // Для каждого охотника устанавливаем клетки с норами на расстоянии 5 + board.SizeX / 13 от охотника
        foreach (var h in board.Hunters)
        {
            Cell cell = h.GetStartCell();
            var tempPossibleCells = PathFindingCommand.GetPath(cell, radius: 5 + board.SizeX / 13);
            var possibleCells = new List<Cell>();

            // Если каждая достижимая клетка есть в списке возможных клеток, добавляем в коллекцию из которой случайно выберем одну из клеток
            foreach (var t in tempPossibleCells)
            {
                if (GetPossibleCells(excludeRange).Contains(t))
                    possibleCells.Add(t);
            }
            if (possibleCells.Count > 0)
                    possibleCells[Random.Range(0, possibleCells.Count - 1)].ChangeCell(CellType.Hole);
        }

    }

    /// <summary>
    /// Размещение клеток указанного типа
    /// </summary>
    /// <param name="type"></param>
    void SetupCellsOfType(CellType type)
    {
        // Задаем радиус исключения 
        int excludeRange = board.MapRange;

        // Находим все возможные клетки для размещения
        var possibleCells = GetPossibleCells(excludeRange);
        int count = 0;
        int amount = board.UnusualCellsCount;

        // Пока количество размещенных клеток меньше количества уникальных клеток на доске, выбираем случайную клетку из возможных
        // Из списка возможных клеток исключаем клетки находящиеся в радиусе excludeRange от выбранной клетки
        while (count < amount)
        {
            if (possibleCells.Count > 0)
            {
                int rand = Random.Range(0, possibleCells.Count - 1);
                possibleCells[rand].ChangeCell(type);
                possibleCells = possibleCells.Except(PathFindingCommand.GetPath(possibleCells[rand], radius: excludeRange)).ToList();
                count++;
                //Debug.Log(type.ToString() + " " + count + " " + possibleCells.Count);
            }
            else break;
        }
    }

    // Выбор всех возможных клеток со списком исключений
    List<Cell> GetPossibleCells(int excludeRange)
    {
        List<Cell> possibleCells = new List<Cell>();
        List<Cell> excludeRootCells = new List<Cell>();
        // Добавляем в список корневых точек исключения стартовую точку
        excludeRootCells.Add(startCell);
        // Если клетка проходимая добавляем ее в список возможных, если клетка уникальная добавляем в список корневых точек исключений
        foreach (var c in cells)
        {
            if (c.Type == CellType.Passable)
            {
                if (c.coord.X > sizeX - 2 && c.coord.Y < 2)
                    continue;
                possibleCells.Add(c);
            }
            else if (c.Type != CellType.Impassable)
                excludeRootCells.Add(c);
        }
        // Для каждой корневой точки исключений добавляем точки достижимые из нее за excludeRange ходов
        foreach (var cell in excludeRootCells) 
        {
            possibleCells = possibleCells.Except(PathFindingCommand.GetPath(cell, radius: excludeRange)).ToList();
        }

        return possibleCells;

    }
 /*   void SetupUnusualCells()
    {
        int excludeRange = board.MapRange;
        var possibleCells = GetPossibleCells(excludeRange);
        int count = board.UnusualCellsCount;
        Debug.Log("Map");
        

    }
    private List<Cell> ExceptListFromAnotherList(List<Cell> list1, List<Cell> excludeList)
    {
        return list1.Except(excludeList).ToList();
    }
    private List<Cell> UnionTwoLists(List<Cell> list1, List<Cell> list2)
    {
        return list1.Union(list2).ToList();
    }
 */

    /// <summary>
    /// Устанавливаем клетки победы и поражения
    /// </summary>
    /// <param name="finishedCells">Список возможных клеток</param>
    void SetupFinishAndLoseCells(List<Cell> finishedCells)
    {
        int count = board.UnusualCellsCount;
        var PassebleEndGameCell = finishedCells;
        // Если список возможных клеток больше чем количество уникальных клеток 2 типов размещаем клетки победы и поражения,
        // Иначе размещаем клетки по очереди пока количество возмоных клеток не закончится
        if (finishedCells.Count > count * 2)
        {
            for (int i = 0; i < count; i++)
            {
                ChangeRandomCellType(PassebleEndGameCell, CellType.Win);
                ChangeRandomCellType(PassebleEndGameCell, CellType.Lose);

            }
        }
        else
        {
            if (finishedCells.Count < count)
                count = finishedCells.Count;
                for (int i = 0; i < count; i++)
                {
                    ChangeRandomCellType(PassebleEndGameCell, CellType.Win);
                    if (count - i <= 1) continue;
                    ChangeRandomCellType(PassebleEndGameCell, CellType.Lose);
                    i++;
                }
        }
    }
    /// <summary>
    /// Изменение типа случайной клетки
    /// </summary>
    /// <param name="passibleCells">Список возможных клеток</param>
    /// <param name="type">Тип клетки</param>
    void ChangeRandomCellType(List<Cell> passibleCells, CellType type)
    {
        // Выбираем случайную клетку
        var rand = Random.Range(0, passibleCells.Count - 1);
        Cell selectedCell = passibleCells[rand];
        // Меняем тип клетки
        selectedCell.ChangeCell(type);
        // Удаляем клетку из списка возможных
        passibleCells.Remove(selectedCell);

        // Если тип клетки "Проигрыш", размещаем в этой клетке охотника
        if(type == CellType.Lose)
        {
            Hunter hunter = Instantiate(gm.hunterPrefab, selectedCell.transform);
            hunter.Initialize(selectedCell, board);

        }
    }

    // Удаление случайных стен, для увеличение ветвления
    void RemoveRandomWalls()
    {
        var wallCellsList = new List<Cell>();
        
        //Определяем количество удаляемых стен
        int count = sizeX > sizeY ? 2 + sizeX / 7 : 2 + sizeY / 7;

        // Если клетка не была посещена, то добавляем в список стен
        foreach(var c in cells)
        {
            if(!visitedCells.Contains(c))
            {
                wallCellsList.Add(c);
            }
        }
        int rand; 

        // Пока количество удаляемых стен больше 0 повторяем цикл удаления стен
        // Если у стены больше 2 соседей она не удаляется
        // Удаляются стены не находящиеся на границе карты и имеющие двух проходимых соседей
        while(count > 0)
        {
            rand = Random.Range(0, wallCellsList.Count - 1);
            Cell selectedCell = wallCellsList[rand];
            int posX = selectedCell.coord.X;
            int posY = selectedCell.coord.Y;
            int passebleNeighbours = selectedCell.GetPassableNeighbours(null).Count;
            if (passebleNeighbours > 2)
                continue;
            if (posX > 0 && posX < board.SizeX - 1)
                if (cells[posX - 1, posY].Type == CellType.Passable && cells[posX + 1, posY].Type == CellType.Passable)
                {
                    selectedCell.ChangeCell(CellType.Passable);
                    visitedCells.Add(selectedCell);
                    wallCellsList.Remove(selectedCell);
                    count--;
                    continue; 
                }
            if (posY > 0 && posY < board.SizeY - 1)
                if (cells[posX, posY - 1].Type == CellType.Passable && cells[posX, posY + 1].Type == CellType.Passable)
                {
                    selectedCell.ChangeCell(CellType.Passable);
                    visitedCells.Add(selectedCell);
                    wallCellsList.Remove(selectedCell);

                    count--;
                    continue; 
                }
        }
    }

    /// <summary>
    /// Метод для удаления стен между двумя клетками
    /// </summary>
    /// <param name="currentCell">Текущая клетка</param>
    /// <param name="nextCell">Следующая клетка</param>
    void RemoveWall(Cell currentCell, Cell nextCell)
    {
        // Вычисляем координаты стены
        int x = (currentCell.coord.X + nextCell.coord.X) / 2;
        int y = (currentCell.coord.Y + nextCell.coord.Y) / 2;
        
        // Меняем тип клетка на проходимую
        cells[x, y].ChangeCell(CellType.Passable);

        // Добавляем в список посещенных клеток
        visitedCells.Add(cells[x, y]);
    }

    /// <summary>
    /// Первичное заполнение игрокой доски, четные строки и столбцы заполняются проходимыми клетками
    /// Нечетные строки и столбцы непроходимыми
    /// </summary>
    /// <returns></returns>
    Cell[,] InitializeBoard()
    {
        Cell[,] cells = new Cell[sizeX, sizeY];
        unvisitedCells = new List<Cell>();
        for(int i = 0; i < sizeX; i++)
            for(int j = 0; j < sizeY; j++)
            {
                cells[i, j] = Instantiate(cellPrefab, board.transform);
                cells[i, j].Initialize(new Vector2(i, j), gm);
                if (i % 2 == 0 && j % 2 == 0)
                {
                    cells[i, j].ChangeCell(CellType.Passable);
                }
                else cells[i, j].ChangeCell(CellType.Impassable);
            }
        gm.board.SetBoard(cells);
        return cells;
    }

    /// <summary>
    /// Добавление соседей в список непосещенных клеток
    /// </summary>
    /// <param name="cell">Клетка соседей которой требуется добавить</param>
    void AddNeighbourToUnvisitedCells(Cell cell)
    {
        List<Cell> neighbours = cell.GetNeighbours(2);
        foreach(var n in neighbours)
        {
            if (!unvisitedCells.Contains(n) && !visitedCells.Contains(n))
                unvisitedCells.Add(n);
        }
    }

    /// <summary>
    /// Возвращает список непосещенных соседей
    /// </summary>
    /// <param name="cell">Клетка для определения соседей</param>
    /// <returns></returns>
    List<Cell> GetUnvisitedNeighbour(Cell cell)
    {
        List<Cell> unvisitedNeighbour = new List<Cell>();
        foreach(var c in cell.GetNeighbours(2))
        {
            if (!visitedCells.Contains(c))
                unvisitedNeighbour.Add(c);
        }
        return unvisitedNeighbour;
    }

}
