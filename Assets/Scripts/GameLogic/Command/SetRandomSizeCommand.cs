using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для команды рандомной генерации размера поля
/// </summary>
public class SetRandomSizeCommand : Command
{
    Menu _menu;

    public SetRandomSizeCommand(Menu menu)
    {
        _menu = menu;
    }
    public override void Execute()
    {
        SetRandomValue();
    }
    /// <summary>
    /// Метод для установки случайных размеров от 11 до 31
    /// </summary>
    private void SetRandomValue()
    {
        int randX;
        do
        { randX = Random.Range(11, 32); }
        while (randX % 2 != 1);
        int randY;
        do
        {
            randY = Random.Range(11, 32);
        } 
        while (randY % 2 != 1);

        _menu.inputFieldX.text = randX.ToString();
        if(!_menu.BoardIsSquare())
            _menu.inputFieldY.text = randY.ToString();
    }
}
