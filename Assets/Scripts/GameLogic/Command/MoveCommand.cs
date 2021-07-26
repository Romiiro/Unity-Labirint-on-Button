using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс команды для перемещения
/// </summary>
public class MoveCommand : Command
{

    Cell _curentCell;
    Cell _targetCell;
    Board _board;
    Menu _menu;
    GameManager _gm;
    int _moves;

    /// <summary>
    /// Конструктор класса, принимает аргументы необходимые для реализации движения
    /// </summary>
    /// <param name="currentCell">Текущая клетка</param>
    /// <param name="targetCell">Целевая клетка</param>
    /// <param name="board">Игровая доска</param>
    /// <param name="gm">GameManager</param>
    public MoveCommand(Cell currentCell, Cell targetCell, Board board, GameManager gm)
    {
        _curentCell = currentCell;
        _targetCell = targetCell;
        _board = board;
        _menu =  GameObject.FindObjectOfType<Menu>();
        _moves = board.MovesWithDisplayedSteps;
        _gm = gm;
    }
    public override void Execute()
    {
        // Вначале движения скрываем игровые сообщения
        _menu.GameTextPanel.SetActive(false);

        DrawComponent.DrawHistoryCells(_board.viewAllMap);
        DrawComponent.DrawCurrentCell(_targetCell, _board.viewAllMap);
        DrawComponent.RotatePlayer(_board.Player, _curentCell, _targetCell);

        // Перемещаем игрока в целевую клетку
        _board.Player.GetComponent<RectTransform>().anchoredPosition = _targetCell.GetComponent<RectTransform>().anchoredPosition;

        // Делаем целевую клетку текущей
        _board.currentCell = _targetCell;


        // Проверяем не является ли целевая клетка уникальной, и выполняем необходимые действия
        // В случае если клетка победная, выводим меню победы.
        if (_targetCell.Type == CellType.Win)
        {
            _menu.menuIsOpen = true;
            _menu.winPanel.SetActive(true);
            _gm.Pause = true;
        }
        // Если целевая клетка содержит карту, выводим соответсвующий текст и вызываем команду показа карты
        else if (_targetCell.Type == CellType.Map)
        {
            Command com = new ShowGameText(_menu, "Хорошая полянка, отсюда можно оглядеться.");
            com.Execute();
            com = new ShowMapCommand(_board);
            com.Execute();
        }
        // Если предыдущая клетка не была клеткой с картой и не включен показ всей карты, вызываем команду сокрытия карты
        else if (_curentCell.Type == CellType.Map && !_board.viewAllMap)
        {
            Command com = new HideMapCommand(_board);
            com.Execute();
        }
        // Если целевая клетка содержит нору, выводим соответствующий текст и заставялем всех охотников потерять игрока
        else if (_targetCell.Type == CellType.Hole)
        {
            Command com = new ShowGameText(_menu, "Здесь можно будет спрятаться в случае опасности.");
            com.Execute();
            foreach (var h in _board.ActiveHunters)
            {
                h.LosePlayer();
            }
        }
        //Если целевая клетка содержит следы, выводим соответствующий текст и вызываем команду показа пути
        else if ( _targetCell.Type == CellType.Path)
        {
            Command com = new ShowGameText(_menu, "Ура я напал на куриный след!");
            com.Execute();
            com = new ShowPathCommand(_board, _targetCell.GetComponent<CellWithPath>());
            _targetCell.ChangeCell(CellType.Passable);
            com.Execute();
        }

        // Если целевая клетка содержит следы, помечаем эти следы посещенные для постоянного отображения. 
        if(_targetCell.GetComponentInChildren<Step>()!= null)
        {
            _targetCell.GetComponentInChildren<Step>().SetIsPermanent();
        }

        //Перебираем всех охотников на карте, если охотник активирован вызываем метод движения.
        foreach (var h in _board.Hunters)
        {
            //_board.currentCell.ChangeCell(CellType.Passable);
            if (h.activated)
                h.DoMove();
        }

        // Если в целевой клетке есть Охотник, выводим меню поражения 
        if (_targetCell.GetComponentInChildren<Hunter>() != null)
        {
            _menu.losePanel.SetActive(true);
            _gm.Pause = true;
        }

        // Если на карте есть отображаемые следы
        if (_board.steps.Count > 0)
        {
            // Увеличиваем счетчик ходов с отображаемыми следами
            _board.MovesWithDisplayedSteps++;
            
            // Если количество ходов больше чем количество отображаемых следов выполняем действия  
            if (_board.MovesWithDisplayedSteps >= _board.MaxVisibleSteps)
            {
                // Если игрок шел по следу, выводим сообщение
                if (_targetCell.GetComponentInChildren<Step>() != null)
                {
                    Command com = new ShowGameText(_menu, "След обрывается, возможно она не далеко ушла.");
                    com.Execute();
                }
                // Уничтожаем следы если они не были посещены
                foreach (var s in _board.steps)
                {
                    if (s.IsPermanent())
                        _board.PermanentSteps.Add(s);
                    else s.Destroy();
                }
                // Очищаем отображаемые следы
                _board.steps.Clear();
                // Сбрасываем счетчик ходов
                _board.MovesWithDisplayedSteps = 0;
            }
        }

        MoveHistory.AddToHistory(_targetCell, _board);

    }

    /*void CellDisplay(Cell cell)
    {
        
        foreach (var neighbour in cell.GetNeighbours())
        {
            neighbour.gameObject.SetActive(true);
            if (neighbour.Type != CellType.Impassable && cell == _targetCell)
                neighbour.gameObject.GetComponent<Button>().interactable = true;
            foreach (var n in neighbour.GetNeighbours())
            {

                if (n != _targetCell)
                {
                    n.gameObject.GetComponent<Button>().interactable = false;
                    if (!MoveHistory.history.Contains(n))
                    {
                        if (!_board.viewAllMap)
                            n.gameObject.SetActive(false);
                        if (n.Type == CellType.Lose && neighbour.Type != CellType.Impassable)
                        {
                            n.gameObject.SetActive(true);
                            Hunter hunter = n.GetComponentInChildren<Hunter>();
                            if (hunter != null && !hunter.activated)
                                hunter.ActivateHunter();
                        }
                    }
                }
            }
        }
    }*/
}
