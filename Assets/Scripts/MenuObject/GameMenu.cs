using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс для игрового меню, с возможностью перезапуска игры
/// </summary>
public class GameMenu : MonoBehaviour
{
    public Board board;
    public Menu menu;

    public Button restartGameButton;

    void Start()
    {
        restartGameButton.onClick.AddListener(RestartGame);
    }

    void RestartGame()
    {
        Command com = new RestartGameCommand(board, menu);
        com.Execute();
    }

}
