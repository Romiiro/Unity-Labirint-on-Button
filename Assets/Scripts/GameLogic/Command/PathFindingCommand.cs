using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Класс для выполнения команд поиска пути
/// </summary>
public static class PathFindingCommand
{
    private static Cell startCell;
    private static Cell finishCell;
    private static int radius;
    

    /// <summary>
    /// Универсальный метод для получения пути 
    /// </summary>
    /// <param name="startCell">Клетка начала пути</param>
    /// <param name="targetCell">Клетка конца пути(необязательный параметр)</param>
    /// <param name="radius">Радиус поиска пути(необязательный параметр)</param>
    /// <returns>Возвращает список клеток</returns>
    public static List<Cell> GetPath(Cell startCell, Cell targetCell = null, int radius = 0)
    {
        List<Cell> path = new List<Cell>();
        // Если не выбрана конечная клетка, вызываем метод, возращающий достижимые ячейки в радиусе
        // Иначе вызываем метод, возращающий путь до целевой клетки
        if (targetCell == null) path = GetCellsInAchievableRadius(startCell, radius);
        else path = GetCalculatedPath(startCell, targetCell);
        return path;
    }

    
   /* public static List<Cell> GetEndsCells(Cell startCell, Cell targetCell = null, int radius = 0)
    {
        List<Cell> path = new List<Cell>();
        if (targetCell == null) path = GetEndCellsInAchievableRadius(startCell, radius);
        else path = GetCalculatedPath(startCell, targetCell);
        return path;
    }*/

    /// <summary>
    /// Метод поиска клеток доступных в радиусе. Возвращает список клеток.
    /// </summary>
    /// <param name="startCell">Стартовая клетка</param>
    /// <param name="radius">Радиус поиска</param>
    /// <returns></returns>
    private static List<Cell> GetCellsInAchievableRadius(Cell startCell, int radius)
    {
        Debug.Log("PathStart");
        List<Cell> path = new List<Cell>();
        
        Cell currentCell = startCell;

        List<Cell> visitedCells = new List<Cell>();
        List<Cell> unvisitedCells = new List<Cell>();
        path.Add(startCell);
        int steps = 0;
        
        do
        {
            // Получаем список доступных для хода соседей
            var currentNeighbours = currentCell.GetPassableNeighbours(visitedCells);        
            // Удаляем из списка непосещенных клеток текущую клетку
            unvisitedCells.Remove(currentCell);

            /*
             * Если нет доступных для хода соседей или количество ходов достигло радиуса, возвращаемся в предыдущую ячейку.
             * Для этого находим предыдущую ячейку, добавляем текущую в список посещенных, удаляем предыдущую ячейку из списка пути
             * И уменьшаем счетчик шагов
             */
             
            if (currentNeighbours.Count == 0 || steps >= radius)
            {
                
                var previousCell = path.FindLast(cell => cell.GetType() == typeof(Cell));
                visitedCells.Add(currentCell);
                path.Remove(previousCell);
                currentCell = previousCell;
                steps--;
                continue;
            }

            if (!visitedCells.Contains(currentCell))
                visitedCells.Add(currentCell);
            
            // Добавляем в список непосещенных клеток соседей текущей клетки
            unvisitedCells = UnionTwoLists(unvisitedCells, currentNeighbours);

            // Случайным образом выбираем клетку из списка соседей для следующего шага
            var nextStep = currentNeighbours[Random.Range(0, currentNeighbours.Count)];
            path.Add(currentCell);
            currentCell = nextStep;
            steps++;
        }
        //Повторям пока есть непосещенные клетки
        while (unvisitedCells.Count != 0);
        Debug.Log("PathEnd");
        return visitedCells;
    }

    /// <summary>
    /// Метод возвращающий конечные клетки, которые можно достигнуть за определенное количество ходов
    /// </summary>
    /// <param name="startCell">Стартовая клетка</param>
    /// <param name="radius">Количество ходов</param>
    /// <returns></returns>
    private static List<Cell> GetEndCellsInAchievableRadius(Cell startCell, int radius)
    {
        Debug.Log("PathStart");
        
        List<Cell> path = new List<Cell>();
        List<Cell> returnedCells = new List<Cell>(); // Список возращаемых клеток
        List<Cell> visitedCells = new List<Cell>();
        List<Cell> unvisitedCells = new List<Cell>();
        Cell currentCell = startCell;
        
        path.Add(startCell);
        int steps = 0;
        do
        {
            // Получаем список доступных для хода соседей, кроме уже посещенных
            var currentNeighbours = currentCell.GetPassableNeighbours(visitedCells);
            // Удаляем из списка непосещенных клеток текущую клетку
            unvisitedCells.Remove(currentCell);
            /*
             * Если нет доступных для хода соседей или количество ходов достигло радиуса, сохраняем клетку в итоговый список и возвращаемся в предыдущую ячейку.
             * Для этого находим предыдущую ячейку, добавляем текущую в список посещенных, удаляем предыдущую ячейку из списка пути
             * И уменьшаем счетчик шагов
             */
            if (currentNeighbours.Count == 0 || steps >= radius)
            {

                var previousCell = path.FindLast(cell => cell.GetType() == typeof(Cell));
                returnedCells.Add(currentCell);
                visitedCells.Add(currentCell);
                path.Remove(previousCell);
                currentCell = previousCell;
                steps--;
                continue;
            }

            if (!visitedCells.Contains(currentCell))
                visitedCells.Add(currentCell);

            // Добавляем в список непосещенных клеток соседей текущей клетки
            unvisitedCells = UnionTwoLists(unvisitedCells, currentNeighbours);

            // Случайным образом выбираем клетку из списка соседей для следующего шага
            var nextStep = currentNeighbours[Random.Range(0, currentNeighbours.Count - 1)];
            path.Add(currentCell);
            currentCell = nextStep;
            steps++;
        }
        //Повторям пока есть непосещенные клетки
        while (unvisitedCells.Count != 0);
        Debug.Log("PathEnd");
        return returnedCells;
    }

    /// <summary>
    /// Метод для объединения двух списков
    /// </summary>
    /// <param name="list1">Первый список</param>
    /// <param name="list2">Второй список</param>
    /// <returns></returns>
    private static List<Cell> UnionTwoLists(List<Cell> list1, List<Cell> list2)
    {
        return list1.Union(list2).ToList();
    }

    /// <summary>
    /// Метод возвращающий путь из точки в точку.
    /// </summary>
    /// <param name="startCell">Стартовая клетка</param>
    /// <param name="targetCell">Финальная клетка</param>
    /// <returns></returns>
    private static List<Cell> GetCalculatedPath(Cell startCell, Cell targetCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = startCell;
        List<Cell> visitedCells = new List<Cell>();

        path.Add(startCell);
        
        while (currentCell != targetCell)
        {
            Debug.Log(currentCell);
            //Получаем список доступных для хода соседей, кроме уже посещенных
            var currentNeighbours = currentCell.GetPassableNeighbours(visitedCells);
            
            //Если нет доступных для хода соседей возвращаемся в пердыдущую точку
            if(currentNeighbours.Count == 0)
            {
                var previousCell = path.FindLast(cell => cell.GetType() == typeof(Cell));
                visitedCells.Add(currentCell);
                path.Remove(currentCell);
                currentCell = previousCell;
                continue;
            }
            // Сортируем список соседей по расстоянию до целевой точки
            var sortedNeighbours = currentNeighbours.OrderBy(node => node.coord.GetDistanse(path.Count, node.coord, targetCell.coord));
            Cell nextStep;
            // Если соседей больше одного, выбираем наилучший путь, иначе выбираем доступного соседа.
            if (currentNeighbours.Count > 1)
            {

                var distance0 = sortedNeighbours.ElementAt(0).coord.GetDistanse(path.Count, sortedNeighbours.ElementAt(0).coord, targetCell.coord);
                var distance1 = sortedNeighbours.ElementAt(1).coord.GetDistanse(path.Count, sortedNeighbours.ElementAt(1).coord, targetCell.coord);
                //Если расстояние между координатами от двух первых соседей до целевой точки одинаковое, сравниваем длину фактического пути и выбираем соседа с меньшим значением
                //Иначе выбираем первого соседа
                if(distance0 == distance1)
                {
                    Debug.Log("1");
                    Debug.Log(sortedNeighbours.ElementAt(0));
                    Debug.Log("2");
                    Debug.Log(sortedNeighbours.ElementAt(1));
                    if (GetPathLength(sortedNeighbours.ElementAt(0), targetCell) > GetPathLength(sortedNeighbours.ElementAt(1), targetCell))
                        nextStep = sortedNeighbours.ElementAt(1);
                    else
                        nextStep = sortedNeighbours.ElementAt(0);
                }
                else nextStep = sortedNeighbours.First();
            }
            else nextStep = sortedNeighbours.First();

            Debug.Log("next");
            Debug.Log(nextStep);

            path.Add(nextStep);
            visitedCells.Add(currentCell);
            currentCell = nextStep;
        }
        // Удаляем из пути стартовую клетку.
        path.RemoveAt(0);
        return path;
    }

    /// <summary>
    /// Метод для вычисления длины пути между двумя клетками
    /// </summary>
    /// <param name="startCell">Стартовая клетка</param>
    /// <param name="targetCell">Целевая клетка</param>
    /// <returns></returns>
    private static int GetPathLength(Cell startCell, Cell targetCell) 
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = startCell;
        List<Cell> visitedCells = new List<Cell>();

        path.Add(startCell);
        
        while (currentCell != targetCell)
        {
            Debug.Log(currentCell);
            // Получаем список доступных для хода соседей, кроме уже посещенных
            var currentNeighbours = currentCell.GetPassableNeighbours(visitedCells);
            // Если нет доступных соседей, возращаемся на предыдущую клетку
            if (currentNeighbours.Count == 0)
            {
                var previousCell = path.FindLast(cell => cell.GetType() == typeof(Cell));
                visitedCells.Add(currentCell);
                path.Remove(currentCell);
                currentCell = previousCell;
                continue;
            }

            // Сортируем список соседей по расстоянию между координатами клеток
            var sortedNeighbours = currentNeighbours.OrderBy(node => node.coord.GetDistanse(path.Count, node.coord, targetCell.coord));
            Cell nextStep;
            // Переходим в соседа с наименьшим расстоянием
            nextStep = sortedNeighbours.First();
            Debug.Log("next");
            Debug.Log(nextStep);
            path.Add(nextStep);
            visitedCells.Add(currentCell);
            currentCell = nextStep;
        }
        // Удаляем из пути стартовую клетку
        path.RemoveAt(0);

        return path.Count;
    }

    /*static float GetDistanse(Coord start, Coord finish)
    {
        return Mathf.Abs(finish.X - start.X) + Mathf.Abs(finish.Y - start.Y);
    }
    private static List<Cell> GetUnvisitedNeighbour(Cell cell, List<Cell> visitedCells)
    {
        return cell.GetPassableNeighbours(null).Except(visitedCells).ToList();
    }*/
}
