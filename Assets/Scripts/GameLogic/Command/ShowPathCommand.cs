using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для реализации команды отображения пути
/// </summary>
public class ShowPathCommand : Command
{
    Board _board;
    CellWithPath _currentCell;

    public ShowPathCommand(Board board, CellWithPath cell)
    {
        _board = board;
        _currentCell = cell;
    }

    public override void Execute()
    {
        ShowPath();
    }

    /// <summary>
    /// Метод для отображения пути
    /// </summary>
    void ShowPath()
    {
        GameManager gm = Object.FindObjectOfType<GameManager>();

        // Случайно выбираем целевую клетку из списка точек интереса 
        var targetCell = _board.PointsOfInterest[Random.Range(0, _board.PointsOfInterest.Count - 1)];
        
        // Размещаем в теущей клетке префаб следов и инициализируем его 
        Step step = Object.Instantiate(gm.stepsPrefab, _currentCell.transform).GetComponent<Step>();
        step.Initialize(_currentCell.GetComponent<Cell>(), _currentCell.path[0], _board);
        // В каждой клетке пути размещаем префаб следов и инициализируем его
        for (int i = 0; i < _currentCell.path.Count-1; i++)
        {   
            step = Object.Instantiate(gm.stepsPrefab, _currentCell.path[i].transform).GetComponent<Step>();
            step.Initialize(_currentCell.path[i], _currentCell.path[i+1],_board);
        }
    }
}
