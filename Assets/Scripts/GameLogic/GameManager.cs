using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Игровой менеджер, содержит переменные для игры
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject stepsPrefab;
    public Player player;
    public Hunter hunterPrefab;
    public Sprite impassebleCellSprite;
    public Sprite passebleCellSprite;
    public Sprite winCellSprite;
    public Sprite mapCellSprite;
    public Sprite pathCellSprite;
    public Sprite holeCellSprite;
    public Cell CellPrefab;
    public GameObject progress;
    public Slider slider;
    public Text text;
    public Board board;
    public bool Pause { get; set; } 
    public bool MenuIsOpen { get; set; }




    void Start()
    {
        Pause = true;
    }


    public void ExitGame()
    {
        Command com = new ExitGameCommand(this);
        com.Execute();
    }
    void PlayerInitialize()
    {
        RectTransform transform = player.GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(0, 0);
        transform.anchoredPosition = new Vector2(115 * 4 + 138, 115 * 0 + 138);
        transform.sizeDelta = new Vector2(80, 80);
    }

    public void OpenGameMenu()
    {
        Command menuOpen = new SwitchMenuCommand();
        menuOpen.Execute();
    }
}
