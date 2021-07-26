using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Класс описывающий игровую доску
/// </summary>
public class Board : MonoBehaviour
{
    Cell[,] board;
    int sizeX;
    int sizeY;

    // Смещение от краев доски 
    float indent;
    
    // Множитель для вычисления размеров игровых элементов
    float multiply;
    public bool viewAllMap = false;
    /// <summary>
    /// Количество уникальных клеток каждого типа
    /// </summary>
    public int UnusualCellsCount { get; set; }
    public int SizeX { get => sizeX; set => sizeX = value; }
    public int SizeY { get => sizeY; set => sizeY = value; }

    /// <summary>
    /// Множитель для вычисления размеров игровых элементов
    /// </summary>
    public float Multiply { get => multiply; } 

    /// <summary>
    /// Радиус отображения карты вокруг игрока
    /// </summary>
    public int MapRange { get; set; }

    public List<Hunter> Hunters { get; set; }
    public List<Hunter> ActiveHunters { get; set; }
    public Cell currentCell;

    /// <summary>
    /// Количество отображаемых пройденых клеток 
    /// </summary>
    public int VisibleRange { get; set; }

    public List<Cell> PointsOfInterest { get; set; }
    public List<CellWithPath> CellsWithPath {get;set;}

    /// <summary>
    /// Количество отображаемых следов
    /// </summary>
    public int MaxVisibleSteps { get; set; }
    public List<Step> steps { get; set; }
    public List<Step> PermanentSteps { get; set; }

    public int MovesWithDisplayedSteps { get; set; }

    Player player;
    public Player Player { get => player; }

    void Start()
    {
 //       SizeX = 13;
 //       SizeY = 13;

        ActiveHunters = new List<Hunter>();
    }

    /// <summary>
    /// Метод заполняющий доску клетками
    /// </summary>
    /// <param name="cells"></param>
    public void SetBoard(Cell[,] cells)
    {
        board = cells;
    }
 /*   public void ShowBoard()
    {
        this.gameObject.SetActive(true);
    } */

    /// <summary>
    /// Метод для инициализации доски
    /// </summary>
    public void Initialize()
    {
        Hunters = new List<Hunter>();
        steps = new List<Step>();
        PermanentSteps = new List<Step>(); 
        CellsWithPath = new List<CellWithPath>();
        PointsOfInterest = new List<Cell>();
        MoveHistory.history = new Queue<Cell>();

        // Вычисляем множитель в зависимости от количества клеток на поле, так чтобы размер игровых элементов 1 к 1 был при размере в 7 клеток
        multiply = SizeX > SizeY ? 7f / sizeX : 7f / sizeY;
        indent = 50 + 50 * multiply;

        //Устанавливаем размер доски с учетом смещений и размеров игрового поля
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(indent * 2 + (100 + 5) * (sizeX - 1) * multiply, indent * 2 + (100 + 5) * (sizeY - 1) * multiply);
        
        // Вычисляем зависимые от размера игрового поля переменные
        UnusualCellsCount = SizeX > SizeY ? 2 + SizeX / 7 : 2 + SizeY / 7;
        MaxVisibleSteps = 5 * SizeX / 13;
        VisibleRange = SizeX > SizeY ? 1 + SizeX / 10 : 1 + SizeY / 10;
        MapRange = SizeX > SizeY ? 2 + SizeX / 7 : 2 + SizeY / 7;
        player = GetComponentInChildren<Player>();
    }

    public float GetIndent()
    {
        return indent;
    }

    public float GetMultiply()
    {
        return multiply;
    }
    /// <summary>
    /// Возвращает клетку по координатам
    /// </summary>
    /// <param name="coord">Координаты</param>
    /// <returns></returns>
    public Cell GetCell(Coord coord)
    {
        return board[coord.X, coord.Y];
    }

    /// <summary>
    /// Очистка данных поля
    /// </summary>
    public void Clear()
    {
        //Destroy(this);
        foreach (var c in board)
            Destroy(c.gameObject);
        System.Array.Clear(board, 0, board.Length);
        Hunters.Clear();
        ActiveHunters.Clear();
        PointsOfInterest.Clear();
        CellsWithPath.Clear();
        steps.Clear();
        PermanentSteps.Clear();
    }

    /// <summary>
    /// Устанавливает текущую клетку по указанным координатам
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetCurrentCell(int x, int y)
    {
        currentCell = board[x, y];
    }

}
