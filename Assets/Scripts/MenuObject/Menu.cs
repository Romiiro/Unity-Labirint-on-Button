using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс описывающий логику меню
/// </summary>
public class Menu : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject GameMenuPanel;
    public GameObject GameMenuButton;
    public GameObject GameTextPanel;
    public Text GameTextPanelText;

    public InputField inputFieldX;
    public InputField inputFieldY;

    public Toggle squareBoardToggle;
    public Toggle randomSizeToggle;

    public Button repeatSizeGenerationButton;
    public Button startGameButton;

    public Sprite[] menuAnimation;
    public Image background;
    private int currentFrame;
    private int timeFromChangeFrame;

    public GameManager gm;

    public bool menuIsOpen;


    /// <summary>
    /// Возвращает "истина" если поставлена соответсвующая галочка в меню
    /// </summary>
    /// <returns></returns>
    public bool BoardIsSquare()
    {
        return squareBoardToggle.isOn;
    }

    public void Start()
    {
        // Загружаем кадры анимации из папки
        menuAnimation = Resources.LoadAll<Sprite>("Steps");
        
        ButtonPress bp = new ButtonPress(gm, this);
        
        // Добавляем контроль изменения значений полей и переключателей в меню
        squareBoardToggle.onValueChanged.AddListener(SetSquareBoard);
        inputFieldX.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateInput(addedChar); };
        inputFieldX.onValueChanged.AddListener(CopyInputValue);
        inputFieldX.onEndEdit.AddListener(delegate (string text) { ValidateEndEdit(text, inputFieldX); });
        inputFieldY.onEndEdit.AddListener(delegate (string text) { ValidateEndEdit(text, inputFieldY); });
        inputFieldY.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateInput(addedChar); };

        randomSizeToggle.onValueChanged.AddListener(SetRandomSize);
        repeatSizeGenerationButton.onClick.AddListener(GenerateRandomSize);
        startGameButton.onClick.AddListener(StartGame);

        MenuBackgroundAnimation();
    }

    /// <summary>
    /// Функция проверки корректности ввода в текстовые поля
    /// </summary>
    /// <param name="s">Введенная строка</param>
    /// <param name="input">Ссылка на поле ввода</param>
    private void ValidateEndEdit(string s, InputField input)
    {
        int num;
        // Если введено не число, устанавлием размер поля по умолчанию 13.
        if (!int.TryParse(s, out num)) input.text = "13"; ;
        // Если значение не входит в диапазон допустимых значений, заменяем на ближайшее допустимое значение
        if (num > 31)
            input.text = "31";
        else if(num < 11)
            input.text = "11";
        // Если введено положительное число, заменяем его на нечетное с округлением в большую сторону
        else input.text = num % 2 == 0 ? (num + 1).ToString() : s;
    }

    /// <summary>
    /// Метод генерации случайного размера
    /// </summary>
    private void GenerateRandomSize()
    {
        var com = new SetRandomSizeCommand(this);
        com.Execute();
    }

    /// <summary>
    /// Метод для устанковки случайного размера при выборе данной опции
    /// </summary>
    /// <param name="isOn"></param>
    void SetRandomSize(bool isOn)
    {
        if (isOn)
            GenerateRandomSize();
    }
    /// <summary>
    /// Функция проверки введенного символа. Если введенный символ не число, то он не вводится.
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    private char ValidateInput(char ch)
    {
        return char.IsDigit(ch) ? ch : '\0';
    }

    /// <summary>
    /// Метод для копирования размера, если выбрано квадратное поле
    /// </summary>
    /// <param name="value"></param>
    void CopyInputValue(string value)
    {
        if (squareBoardToggle.isOn)
            inputFieldY.text = inputFieldX.text;
    }

    /// <summary>
    /// Метод отключающий ввод вертикального размера, если выбрано квадратное поле 
    /// </summary>
    /// <param name="isOn"></param>
    void SetSquareBoard(bool isOn)
    {
        if(!isOn)
        {
            inputFieldY.interactable = true;
        }
        else
        {
            inputFieldY.interactable = false;
            inputFieldY.text = inputFieldX.text;
        }
    }

    /// <summary>
    /// Метод для запуска игры
    /// </summary>
    void StartGame()
    {
        StopMenuBackgroundAnimation();
        gm.progress.SetActive(true);
        ButtonPress bp = new ButtonPress(gm, this);
        int x;
        int y;
        // Если поля размера не заполнены, заполняем их случаными размерами
        if (inputFieldX.text == "" || inputFieldY.text == "") GenerateRandomSize();
        // Если в полях размера, все-таки оказались данные которые нельзя перевести в число, заполняем случайными размерам
        if (!int.TryParse(inputFieldX.text, out x) || !int.TryParse(inputFieldY.text, out y)) return;
        bp.Com = new StartGameCommand(gm, x, y);
        bp.Execute();
    }
    /// <summary>
    /// Метод вызываемый нажатием кнопок  с размерами карт в меню
    /// </summary>
    /// <param name="size"></param>
    public void StartGame(int size)
    {
        StopMenuBackgroundAnimation();
        gm.progress.SetActive(true);
        //gm.progress.gameObject.SetActive(true);
        ButtonPress bp = new ButtonPress(gm, this);
        bp.Com = new StartGameCommand(gm, size, size);
        bp.Execute();
    }
    public void StartGame(int sizeX, int sizeY)
    {
        // Останавливем анимацию фона
        StopMenuBackgroundAnimation();

        gm.progress.SetActive(true);
        
        ButtonPress bp = new ButtonPress(gm, this);
        bp.Com = new StartGameCommand(gm, sizeX, sizeY);
        bp.Execute();
    }

    /// <summary>
    /// Метод для остановки анимации меню, устанавливает статично первый кадр амнимации
    /// </summary>
    public void StopMenuBackgroundAnimation()
    {
        background.sprite = menuAnimation[0];
        CancelInvoke();
    }

    /// <summary>
    /// Метод для запуска анимации фона с 1 кадра
    /// </summary>
    public void MenuBackgroundAnimation()
    {
        background.sprite = menuAnimation[0];
        currentFrame = 1;
        InvokeRepeating("ChangeFrame", 5f, 0.03f);
    }

    // Метод для смены кадры
    private void ChangeFrame()
    {
        // Счетчик времени между кадрами увеличиваем каждый кадр на 3мс
        timeFromChangeFrame += 3; 

        background.sprite = menuAnimation[currentFrame];
        // Название файла содержит информацию о задержке перед его сменой, вытаскиваем эту информацию
        string delayString = menuAnimation[currentFrame].name.Substring(18, 2);
        int timeToNextFrame = Int32.Parse(delayString);
        
        // Если время между кадрами больше, задержки кадра переходим на следующий кадр и сбрасываем счетчик 
        if (timeFromChangeFrame >= timeToNextFrame)
        {
            currentFrame++;
            timeFromChangeFrame = 0;
        }
        // Если номер кадра после увеличения равен количеству кадров, то переходим на первый кадр 
        if (currentFrame == menuAnimation.Length)
            currentFrame = 0;
    }

}
