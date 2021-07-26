using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Игрок
/// </summary>
public class Player : MonoBehaviour
{

    Board _board;
    GameManager _gm;
    

    // Обработка движений игрока с клавиатуры
    // ESC - меню
    // F7 - дебаг режим с отображением всей карты
    void Update()
    {
        
        if ((!_gm.Pause || _gm.MenuIsOpen ) && Input.GetKeyDown(KeyCode.Escape))
        {
            _gm.OpenGameMenu();
        }
        
        if (!_gm.Pause)
        {

             if (Input.GetKeyDown(KeyCode.UpArrow))
             {
                 if (_board.currentCell.coord.Y < _board.SizeY - 1)
                 {
                     Coord newCoord = new Coord(_board.currentCell.coord.X, _board.currentCell.coord.Y + 1);
                     if (_board.GetCell(newCoord).Type != CellType.Impassable)
                     {
                         Command move = new MoveCommand(_board.currentCell, _board.GetCell(newCoord), _board, _gm);
                         move.Execute();
                     }
                 }
             }
             else if (Input.GetKeyDown(KeyCode.DownArrow))
             {
                 if (_board.currentCell.coord.Y > 0)
                 {
                     Coord newCoord = new Coord(_board.currentCell.coord.X, _board.currentCell.coord.Y - 1);
                     if (_board.GetCell(newCoord).Type != CellType.Impassable)
                     {
                         Command move = new MoveCommand(_board.currentCell, _board.GetCell(newCoord), _board, _gm);
                         move.Execute();
                     }
                 }
             }
             else if (Input.GetKeyDown(KeyCode.RightArrow))
             {
                 if (_board.currentCell.coord.X < _board.SizeX - 1)
                 {
                     Coord newCoord = new Coord(_board.currentCell.coord.X + 1, _board.currentCell.coord.Y);
                     if (_board.GetCell(newCoord).Type != CellType.Impassable)
                     {
                         Command move = new MoveCommand(_board.currentCell, _board.GetCell(newCoord), _board, _gm);
                         move.Execute();
                     }
                 }
             }
             else if (Input.GetKeyDown(KeyCode.LeftArrow))
             {
                 if (_board.currentCell.coord.X > 0)
                 {
                     Coord newCoord = new Coord(_board.currentCell.coord.X - 1, _board.currentCell.coord.Y);
                     if (_board.GetCell(newCoord).Type != CellType.Impassable)
                     {
                         Command move = new MoveCommand(_board.currentCell, _board.GetCell(newCoord), _board, _gm);
                         move.Execute();
                     }
                 }
             }
             else if (Input.GetKeyDown(KeyCode.F7))
             {
                 if (!_board.viewAllMap)
                 {
                     _board.viewAllMap = true;
                     Command com = new ShowMapCommand(_board, true);
                     com.Execute();
                 }
                 else
                 {
                     _board.viewAllMap = false;
                     Command com = new HideMapCommand(_board);
                     com.Execute();
                 }

             }
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape2");
            }
        }
    }

    /// <summary>
    /// Инициализация игрока
    /// </summary>
    /// <param name="board"></param>
    public void Initialise(Board board)
    {
        _board = board;
        _gm = FindObjectOfType<GameManager>();

        RectTransform transform = gameObject.GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(0, 0);
        transform.anchoredPosition = board.currentCell.GetComponent<RectTransform>().anchoredPosition;
        transform.sizeDelta = new Vector2(80 * board.Multiply, 80 * board.Multiply); 
        transform.SetAsLastSibling();
    }


}
