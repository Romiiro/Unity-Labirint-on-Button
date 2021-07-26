using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Следы
/// </summary>
public class Step : MonoBehaviour
{
    Board _board;
    bool _permanent = false;

    /// <summary>
    /// Инициализация следов
    /// </summary>
    /// <param name="cell">Текущая клетка</param>
    /// <param name="nextCell">Следующая клетка</param>
    /// <param name="board">Доска</param>
    public void Initialize(Cell cell, Cell nextCell,Board board)
    {
        _board = board;
        _board.steps.Add(this);
        // Указание позиции и размеров следа
        RectTransform transform = GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0.5f, 0.5f);
        transform.anchorMax = new Vector2(0.5f, 0.5f);
        transform.anchoredPosition = new Vector2(0, 0);
        transform.sizeDelta = new Vector2(80 * board.Multiply, 80 * board.Multiply);
        transform.SetAsLastSibling();
        
        // Вращение следов в зависимости от расположения слудающей клетки
        if(cell.coord.X > nextCell.coord.X)
        {
            transform.localScale = new Vector3(-1, 1,1);
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else if(cell.coord.Y < nextCell.coord.Y)
        {
           transform.eulerAngles = new Vector3(0,0,90);
                
        }
        else if (cell.coord.Y > nextCell.coord.Y)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }

    public void SetIsPermanent()
    {
        _permanent = true;
    }

    public bool IsPermanent()
    {
        return _permanent;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
