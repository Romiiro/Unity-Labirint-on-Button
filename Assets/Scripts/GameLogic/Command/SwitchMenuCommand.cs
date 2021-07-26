using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для открытия и закрытия игрового меню
/// </summary>
public class SwitchMenuCommand : Command
{
    Menu _menu;
    GameManager _gm;
    public override void Execute()
    {
        _menu = Object.FindObjectOfType<Menu>();
        _gm = Object.FindObjectOfType<GameManager>();

        if (!_gm.MenuIsOpen)
        {
            _menu.GameMenuPanel.SetActive(true);
            _menu.GameMenuButton.SetActive(false);
            _gm.MenuIsOpen = true;
            Command com = new PauseGameCommand();
            com.Execute();
        }
        else
        {
            _menu.GameMenuPanel.SetActive(false);
            _gm.MenuIsOpen = false;
            _menu.GameMenuButton.SetActive(true);
            Command com = new PauseGameCommand();
            com.Execute();
        }
    }
}
