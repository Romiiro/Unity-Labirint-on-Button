using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Клетка с указанием пути
/// </summary>
public class CellWithPath : MonoBehaviour
{
    Cell targetCell;
    public List<Cell> path;
    Board _board;

    public void SetTargetPoint(Board board)
    {
        targetCell = board.PointsOfInterest[Random.Range(0, board.PointsOfInterest.Count - 1)];
        path = PathFindingCommand.GetPath(gameObject.GetComponent<Cell>(), targetCell);
    }
}
