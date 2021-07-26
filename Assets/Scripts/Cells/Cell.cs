using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Клетка
/// </summary>
public class Cell : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager gameManager;
    public Board board;
    public Coord coord;
    public List<Cell> NeighbourList { get; set; }
    public Vector2 Position { get; set; }
    public CellType Type { get; set; }

    private Hunter _hunter;

    private Vector2 _defaultCellSize = new Vector2 (100, 100);
    private Vector2 _defaultImpassableCellSize = new Vector2(110, 110);
    private float _defaultIntercellurarSpacing = 5;

    private GameObject parent;
    private List<Cell> neighbourList = new List<Cell>();
    private Vector2 size;
  //  private CellType type;
    private Vector2 position;
    private Vector2 coords;

    /*public void Destoy()
    {
        DestroyImmediate(this);
    }
    */

    /// <summary>
    /// Инициализация клетки
    /// </summary>
    /// <param name="pos">Позиция</param>
    /// <param name="gm">GameManager</param>
    public void Initialize(Vector2 pos, GameManager gm)
    {
        board = gm.board;
        float multiply = board.GetMultiply();
        var intercellurarSpacing = _defaultIntercellurarSpacing * multiply;
        coord = new Coord((int)pos.x, (int)pos.y);
        gameManager = gm;

        // Указание имени клетки
        gameObject.name = "Cell_" + coord.X + "_" + coord.Y;
        
        // Вычисление размера клетки
        RectTransform transform = gameObject.GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(0, 0);
        transform.sizeDelta = _defaultCellSize * multiply;
        
        float indent = board.GetIndent();
        // Вычисление позиции клетки
        transform.anchoredPosition = coords = new Vector2((transform.sizeDelta.x + intercellurarSpacing) * coord.X, (transform.sizeDelta.y + intercellurarSpacing) * coord.Y) + new Vector2(indent, indent);

        // Если клетка находится в радиусе старта игры делаем ее активной, иначе скрываем ее
        if ((pos.x == board.SizeX - 2 && pos.y == 0) || (pos.x == board.SizeX - 1 && pos.y == 0) || (pos.x == board.SizeX - 1 && pos.y == 1))
        {
            gameObject.SetActive(true);
            GetComponent<Button>().interactable = true;
            if (pos.x == board.SizeX - 1 && pos.y == 0)
            {
                GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            gameObject.SetActive(false);
            DisableButton();
        }
    }
        
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(ActivateCell);
    }
    /// <summary>
    /// Активация клетки при нажатии
    /// </summary>
    void ActivateCell()
    {
        Command com = new MoveCommand(board.currentCell, this, board, gameManager);
        com.Execute();

    }

    // Отключение кнопки
    public void DisableButton()
    {
        gameObject.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// Смена типа клетки
    /// </summary>
    /// <param name="type">Новый тип клетки</param>
    public void ChangeCell(CellType type)
    {
        this.Type = type;
        switch (type)
        {
            case CellType.Passable:
                gameObject.GetComponent<Image>().sprite = gameManager.passebleCellSprite;
                break;
            case CellType.Impassable:
                gameObject.GetComponent<Image>().sprite = gameManager.impassebleCellSprite;
                break;
            case CellType.Win:
                gameObject.GetComponent<Image>().sprite = gameManager.winCellSprite;
                board.PointsOfInterest.Add(this);
                break;
            case CellType.Lose:
                gameObject.GetComponent<Image>().sprite = gameManager.passebleCellSprite;
                board.PointsOfInterest.Add(this);
                break;
            case CellType.Map:
                gameObject.GetComponent<Image>().sprite = gameManager.mapCellSprite;
                board.PointsOfInterest.Add(this);
                break;
            case CellType.Path:
                gameObject.GetComponent<Image>().sprite = gameManager.pathCellSprite;
                gameObject.AddComponent<CellWithPath>();
                board.CellsWithPath.Add(GetComponent<CellWithPath>());
                break;
            case CellType.Hole:
                gameObject.GetComponent<Image>().sprite = gameManager.holeCellSprite;
                board.PointsOfInterest.Add(this);
                break;
        }
        RectTransform transform = gameObject.GetComponent<RectTransform>();

        if (type == CellType.Impassable)
        {
            transform.sizeDelta = _defaultImpassableCellSize * board.GetMultiply();
            //GetComponent<Button>().interactable = false;
        }
        else
        { 
            transform.sizeDelta = _defaultCellSize * board.GetMultiply();
        }
    }

    /// <summary>
    /// Возвращает список проходимых соседей
    /// </summary>
    /// <param name="excludeCells">Спискок клеток для исключения</param>
    /// <param name="step">Шаг</param>
    /// <returns></returns>
    public List<Cell> GetPassableNeighbours(List<Cell> excludeCells, int step = 1)
    {
        List<Cell> neighbours = new List<Cell>();
        // Получает список всех соседей
        List<Cell> allNeighbour = GetNeighbours(step);
        // Исключаем клетки из списка исключений
        if(excludeCells != null)
            allNeighbour = allNeighbour.Except(excludeCells).ToList();
        // Проверяем каждого соседа и если он проходимый добавляем в итоговый список
        foreach (var c in allNeighbour)
        {
            if(c.Type != CellType.Impassable)
                neighbours.Add(c);
        }
        return neighbours;
    }

    /// <summary>
    /// Список всех соседних клеток, со смещением. Смещение используется когда ищется со сосед через n клеток
    /// </summary>
    /// <param name="step">Смещение поиска соседей, по умолчанию 1</param>
    /// <returns></returns>
    public List<Cell> GetNeighbours(int step = 1)
    {
        List<Cell> neighbours = new List<Cell>();
        List<Coord> neighbourCoords = coord.GetNeighboursCoods(board.SizeX, board.SizeY, step);
        foreach(var c in neighbourCoords)
        {
            neighbours.Add(board.GetCell(c));
        }
        return neighbours;
    }

    /// <summary>
    /// Смена отображения клетки
    /// </summary>
    /// <param name="active"></param>
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
