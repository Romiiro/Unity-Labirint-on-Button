using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс команды для постановки и снятия игры с паузы
/// </summary>
public class PauseGameCommand : Command
{
    GameManager _gm;
    public override void Execute()
    {
        _gm = GameObject.FindObjectOfType<GameManager>();
        if (!_gm.Pause)
            _gm.Pause = true;
        else
            _gm.Pause = false;
    }
}
