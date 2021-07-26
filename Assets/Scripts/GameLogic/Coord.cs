using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Класс описывающий координаты
/// </summary>
public class Coord
    {
    public int X { get; set; }
    public int Y { get; set; }

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }
    /// <summary>
    /// Возвращает список соседних координат
    /// </summary>
    /// <param name="sizeX">Размер поля x</param>
    /// <param name="sizeY">Размер поля y</param>
    /// <returns></returns>
    public List<Coord> GetNeighboursCoods(int sizeX, int sizeY)
    {
        return GetNeighboursCoods(sizeX, sizeY, 1);
    }

    /// <summary>
    /// Возвращает список соседних координат с заданным шагом
    /// </summary>
    /// <param name="sizeX">Размер поля по горизонтали</param>
    /// <param name="sizeY">Размер поля по вертикали</param>
    /// <param name="step">Шаг</param>
    /// <returns></returns>
    public List<Coord> GetNeighboursCoods(int sizeX, int sizeY, int step)
    {
        List<Coord> neighbourCoords = new List<Coord>();
        for(int i = X-step; i <= X + step; i+=step)
            for(int j = Y - step; j <= Y + step; j+=step)
            {
                if(i>=0 && j >=0 && i < sizeX && j < sizeY)
                {
                    if (i == X && j == Y)
                        continue;
                    if (i == X || j == Y)
                        neighbourCoords.Add(new Coord(i, j));
                }
            }
        return neighbourCoords;
    }

    /// <summary>
    /// Возврщает расстояние по прямой между клетками
    /// </summary>
    /// <param name="steps">Количество шагов до стартовой клетки</param>
    /// <param name="start">Стартовая клетка</param>
    /// <param name="finish">Целевая клетка</param>
    /// <returns></returns>
    public float GetDistanse(int steps, Coord start, Coord finish)
    {
        return steps + Mathf.Abs(finish.X - start.X) + Mathf.Abs(finish.Y - start.Y);
    }
}
