using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Класс для реализации команды запуска игры
/// </summary>
public class StartGameCommand : Command
{
    GameManager gm;
    int _sizeX;
    int _sizeY;

    /// <summary>
    /// Конструктор класса запуску игры
    /// </summary>
    /// <param name="gm">GameManager</param>
    /// <param name="sizeX">Размер по горизонтали, по умолчанию 13</param>
    /// <param name="sizeY">Размер по вертикали, п0 умолчанию 13</param>
    public StartGameCommand(GameManager gm, int sizeX = 13, int sizeY = 13)
    {
        this.gm = gm;
        _sizeX = sizeX;
        _sizeY = sizeY;
    }
    
    public override void Execute() 
    {
        gm.board.SizeX = _sizeX;
        gm.board.SizeY = _sizeY;
        gm.board.Initialize();
        LabGenerator lg = gm.GetComponentInChildren<LabGenerator>();
        lg.Init(gm);
        lg.Generate();
        
        gm.board.SetCurrentCell(gm.board.SizeX - 1, 0);
        gm.player.Initialise(gm.board);
        
        Debug.Log("Game is started");
    }
}
