using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для команды перезапуска игры
/// </summary>
public class RestartGameCommand : Command
{
    Board _board;
    Menu _menu;
    
    public RestartGameCommand(Board board, Menu menu)
    {
        _board = board;
        _menu = menu;
    }
    public override void Execute()
    {
        _board.Clear();
        _board.gameObject.SetActive(false);
        _menu.StartGame(_board.SizeX, _board.SizeY);
    }


}
