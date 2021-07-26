using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// История ходов
/// </summary>
public static class MoveHistory
{

    public static Queue<Cell> history;

    public static void AddToHistory(Cell cell, Board board)
    {
        if (history.Count > board.VisibleRange)
            history.Dequeue();
        history.Enqueue(cell);
    }


}
