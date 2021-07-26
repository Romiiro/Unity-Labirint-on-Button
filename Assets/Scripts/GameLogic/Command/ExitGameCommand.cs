using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Класс команды для завершения игры и открытия меню
/// </summary>
public class ExitGameCommand : Command
{
    Board _board;
    GameManager _gm;
    public ExitGameCommand(GameManager gm)
    {
        _board = gm.board;
        _gm = gm;
    }
    public override void Execute()
    {
        _board.Clear();
        _board.viewAllMap= false;
        _gm.MenuIsOpen = false;
        _board.gameObject.SetActive(false);
    }

}
