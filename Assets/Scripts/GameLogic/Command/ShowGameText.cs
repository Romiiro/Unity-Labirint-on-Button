using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс для реализации команды вывода игрового текста
/// </summary>
public class ShowGameText : Command
{
    private string _message;
    private Text _text;
    private GameObject _textPanel;

    public ShowGameText(Menu menu, string str)
    {
        _text = menu.GameTextPanelText;
        _textPanel = menu.GameTextPanel;
        _message = str;
    }

    public override void Execute()
    {
        _textPanel.SetActive(true);
        _text.text = _message;
    }
}
