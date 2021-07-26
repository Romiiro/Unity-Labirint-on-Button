using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс для обработки нажатия кнопок
/// </summary>
public class ButtonPress
{
    GameManager gm;
    Menu menu;

    //Команды вызываемая при нажатии кнопки
    public Command Com { get; set; } 


    public ButtonPress(GameManager gm, Menu menu)
    {

    }
    public void Execute()
    {
        Com.Execute();
    }
}
