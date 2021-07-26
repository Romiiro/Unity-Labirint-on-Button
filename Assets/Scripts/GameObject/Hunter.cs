using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Охотник
/// </summary>
public class Hunter : MonoBehaviour
{
    bool playerViewed = false;
    Cell startCell;
    Board _board;
    public bool activated = false;
    bool firstMove = true;
    List<Cell> pathToStartPosition;
    private Menu _menu;

    Cell currentCell;

    public Cell GetStartCell()
    { 
        return startCell;
    }

    /// <summary>
    /// Инициализация охотника
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="board"></param>
    public void Initialize(Cell cell, Board board)
    {
        // Сохранаяем текущую клетку как стартовую
        startCell = currentCell = cell;
        _board = board;
        _board.Hunters.Add(this);
        // Задаем размеры изображения охотника
        RectTransform transform = gameObject.GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0.5f, 0.5f);
        transform.anchorMax = new Vector2(0.5f, 0.5f);
        transform.anchoredPosition = new Vector2(0, 0);
        transform.sizeDelta = new Vector2(80 * board.Multiply, 80 * board.Multiply);
        transform.SetAsLastSibling();
        _menu = FindObjectOfType<Menu>();
    }

    // Активация охотника
    public void ActivateHunter()
    {
        // Вывод текста
        Command com = new ShowGameText(_menu, "О нет, меня заметил охотник. Нужно куда-нибудь спрятаться!");
        com.Execute();
        
        playerViewed = true;
        _board.ActiveHunters.Add(this);
        activated = true;
    }
    /// <summary>
    /// Движение охотника
    /// </summary>
    public void DoMove()
    {
        // Пропускаем первый ход
        if (firstMove)
        { 
            firstMove = false;
            return;
        }
        List<Cell> path;
        // Если видим игрока, то преслудем его рассчитывая маршрут до него
        if (playerViewed)
        {
            path = PathFindingCommand.GetPath(currentCell, _board.currentCell);
            currentCell = path[0];
            currentCell.SetActive(true);
        }
        // Если игрок спрятался и путь до стартовой клетки не рассчитан, рассчитываем путь
        else if (pathToStartPosition == null)
            pathToStartPosition = PathFindingCommand.GetPath(currentCell, startCell);
        // Если путь рассчитан, двигаемся по рассчитаному пути
        else
        {
            currentCell = pathToStartPosition[0];
            pathToStartPosition.RemoveAt(0);
        }
        
        // Переходим в соседнюю клетку делая ее родительской
        transform.SetParent(currentCell.transform);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        
        // Если дошли до стартовой клетки дезактивируемся
        if(currentCell == startCell)
        {
            activated = false;
            firstMove = true;
            _board.ActiveHunters.Remove(this);
            pathToStartPosition = null;
        }
        //currentCell.ChangeCell(CellType.Lose);
    }

    /// <summary>
    /// Метод для потери игрока, с выводом сообщения
    /// </summary>
    public void LosePlayer()
    {
        if (playerViewed)
        {
            Command com = new ShowGameText(_menu, "Фух, успел. Надеюсь он не видел куда я спрятался?");
            com.Execute();
        }

        playerViewed = false;
    }


}
