using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Класс для реализации меню окончания игры, с возможностью перезапуска игры
public class EndGameMenu : MonoBehaviour
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
