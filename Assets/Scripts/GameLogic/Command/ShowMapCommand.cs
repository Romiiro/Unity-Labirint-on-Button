using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для реализации команды отображения карты
/// </summary>
public class ShowMapCommand : Command
{
    Board _board;
    bool _showAll;

    public ShowMapCommand(Board board, bool showAll = false)
    {
        _board = board;
        _showAll = showAll;
    }

    public override void Execute()
    {
        if (_showAll)
            ShowAllCells();
        else 
            ShowCellsInRange();
    }
    /// <summary>
    /// Метод для отображения клеток вокруг игрока
    /// </summary>
    private void ShowCellsInRange()
    {
        Cell currentCell = _board.currentCell;
        int range = _board.MapRange;

        for (int i = currentCell.coord.X - range; i <= currentCell.coord.X + range; i++)
            for (int j = currentCell.coord.Y - range; j < currentCell.coord.Y + range; j++)
            {
                if( i >= 0 && j >=0 && i < _board.SizeX && j < _board.SizeY)
                    _board.GetCell(new Coord(i, j)).SetActive(true);
            }
    }
    /// <summary>
    /// Метод для отображения всей карты
    /// </summary>
    private void ShowAllCells()
    {
        for(int i = 0; i < _board.SizeX; i++)
            for(int j = 0; j <_board.SizeY; j++)
            {
                _board.GetCell(new Coord(i, j)).SetActive(true);
            }
    }
}
