using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Класс для отображения клеток
/// </summary>
public static class DrawComponent 
{
    /// <summary>
    /// Отображение истории ходов
    /// </summary>
    /// <param name="mapShow"></param>
    public static void DrawHistoryCells(bool mapShow)
    { 
        // Отображение соседей для каждой клетки в истории ходов
        foreach(Cell c in MoveHistory.history)
        {
            foreach (var neighbour in c.GetNeighbours())
            {
                neighbour.gameObject.SetActive(true);
                foreach (var n in neighbour.GetNeighbours())
                {
                    n.gameObject.GetComponent<Button>().interactable = false;
                        if (!MoveHistory.history.Contains(n) && !mapShow)
                        {
                            n.gameObject.SetActive(false);
                        }
                }
            }
        }
    }
    /// <summary>
    /// Отображение текущей клетки
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="mapShow"></param>
    public static void DrawCurrentCell(Cell cell, bool mapShow)
    {
        cell.gameObject.SetActive(true);
        // Отображение соседей клетки и активация кнопок при необходимости
        foreach (var neighbour in cell.GetNeighbours())
        {
            neighbour.gameObject.SetActive(true);
            if (neighbour.Type != CellType.Impassable)
                neighbour.gameObject.GetComponent<Button>().interactable = true;
            // Для всех соседей соседей: сокрытие если они не в списке истории
            foreach (var n in neighbour.GetNeighbours())
            {

                if (n != cell && !MoveHistory.history.Contains(n))
                {
                    n.gameObject.GetComponent<Button>().interactable = false;
                    if (!mapShow)
                        n.gameObject.SetActive(false);
                }
                // Если в клетке соседа проходимого соседа есть охотник, то происходит его активация и отображение на игровом поле  
                if (neighbour.Type != CellType.Impassable && n.Type!= CellType.Impassable && n.GetComponentInChildren<Hunter>() != null)
                {
                    n.gameObject.SetActive(true);
                    Hunter hunter = n.GetComponentInChildren<Hunter>();
                    if (hunter != null && !hunter.activated)
                        hunter.ActivateHunter();
                    
                }
            }
        }
    }

    /// <summary>
    /// Метод для вращения игрока в зависимости от направления движения
    /// </summary>
    /// <param name="player">Ссылка на игрока</param>
    /// <param name="current">Текущая клетка</param>
    /// <param name="target">Целевая клетка</param>
    public static void RotatePlayer(Player player, Cell current, Cell target)
    {
        RectTransform transform = player.GetComponent<RectTransform>();
        if (current.coord.X > target.coord.X)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (current.coord.X < target.coord.X)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
            transform.localScale = new Vector3(1, -1, 1);
        }
        else if (current.coord.Y < target.coord.Y)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (current.coord.Y > target.coord.Y)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1, -1, 1);
        }
    }
}
