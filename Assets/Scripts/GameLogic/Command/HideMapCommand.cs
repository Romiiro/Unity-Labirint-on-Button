using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Комманда для сокрытия карты.
/// </summary>
public class HideMapCommand : Command
{
    Board _board;

    public HideMapCommand(Board board)
    {
        _board = board;
    }
    public override void Execute()
    {
        HideMap();
    }

    void HideMap()
    {
        for(int i=0; i < _board.SizeX; i++)
            for(int j = 0; j < _board.SizeY; j++)
            {
                var cell = _board.GetCell(new Coord(i, j));
                cell.SetActive(false);
            }
        DrawComponent.DrawHistoryCells(false);
        DrawComponent.DrawCurrentCell(_board.currentCell, false);
    }
}
