using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Интерфейс, скрывающий реализацию визуализации игры
/// </summary>
public interface ILevelScene
{
    /// <summary>
    /// Меняет цвет ячейки игрового поля
    /// </summary>
    /// <param name="x"> Координата X ячейки </param>
    /// <param name="y"> Координата Y ячейки </param>
    /// <param name="value"> Номер цвета ячеки</param>
    void UpdateCell(int x, int y, int value);

    /// <summary>
    /// Увеличивает счет игры
    /// </summary>
    /// <param name="delta">Значение, на которое производится увеличение</param>
    void AddScore(int delta);
}

/// <summary>
/// Базовый класс для создания уровней игры.
/// Определяет логику игрового уровня.
/// </summary>
public abstract class GameLevel : ILevelCanvas
{
    private ILevelScene levelScene;                 // интерфейс сцены
    private readonly Dictionary<FigureSkin, int> availableShapes;  // контейнер с типами допустимых фигур и их вероятностями выпадения
    private Queue<FigureSkin> figures;              // очередь типов выпадаемых фигур
    private List<FigureSkin> figuresList;           // перемешанный список типов фигур
    private System.Random randomizer;

    protected const int WallValue = 1000;           // значение цвета в ячейках, соответствующих краям уровня

    protected int levelWidth;                       // ширина игрового поля
    protected int levelHeight;                      // высота игрового поля
    protected int[,] pixels;                        // массив со значениями цветов игрового поля
    protected List<int> completedRows;              // список заполненных линий уровня

    /// <summary>
    /// Создает экземпляр уровня в соответствии с именем класса уровня
    /// </summary>
    /// <param name="levelClassName">Имя класса уровня</param>
    /// <returns>Указатель на объект уровня</returns>
    public static GameLevel CreateInstance(string levelClassName)
    {
        Type LevelType = Type.GetType(levelClassName);
        if (LevelType != null)
        {
            // определяется конструктор уровня
            System.Reflection.ConstructorInfo ci = LevelType.GetConstructor(new Type[] { });
            if (ci != null)
            {
                // создаем экземпляр уровня
                object obj = ci.Invoke(new object[] { });
                if (obj != null)
                {
                    GameLevel result = obj as GameLevel;
                    return result;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public GameLevel()
    {
        levelScene = null;
        levelWidth = 0;
        levelHeight = 0;
        pixels = null;
        figures = null;
        figuresList = null;
        completedRows = null;
        randomizer = null;
        availableShapes = GetAvailableShapes();
    }

    /// <summary>
    /// Приводит уровень в исходное состояние
    /// </summary>
    /// <param name="width">Ширина игрового поля</param>
    /// <param name="height">Высота игрового поля</param>
    /// <param name="scene">Интерфейс сцены, обеспечивающий отображение уровня</param>
    public void Init(int width, int height, ILevelScene scene)
    {
        levelScene = scene;

        if (width != levelWidth || height != levelHeight)
        {
            levelWidth = width;
            levelHeight = height;
            pixels = new int[height, width];
        }

        // ячейки игрового поля перекрашиваются в цвет фона
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                UpdatePixel(x, y, 0);
            }
        }

        if (randomizer == null)
        {
            randomizer = new System.Random();
        }

        if (figures == null)
        {
            figures = new Queue<FigureSkin>();
        }
        else
        {
            figures.Clear();
        }

        if (figuresList == null)
        {
            figuresList = new List<FigureSkin>();
        }

        if (completedRows == null)
        {
            completedRows = new List<int>();
        }
    }

    /// <summary>
    /// Обновляет цвет ячейки игрового поля в сцене
    /// </summary>
    /// <param name="x">Координата X ячейки</param>
    /// <param name="y">Координата Y ячейки</param>
    /// <param name="value">Индекс цвета</param>
    protected void UpdatePixel(int x, int y, int value)
    {
        pixels[y, x] = value;
        levelScene.UpdateCell(x, y, value);
    }

    /// <summary>
    /// Увеличивает счет сцены на значение delta
    /// </summary>
    protected void AddScore(int delta)
    {
        levelScene.AddScore(delta);
    }

    /// <summary>
    /// Удалает заполненные линии игрового поля, сдвигая вышестоящие ячейки вниз.
    /// </summary>
    /// <param name="rows">Список удаляемых ячеек</param>
    protected void ShiftRowsDown(List<int> rows)
    {
        foreach (int row in rows)
        {
            for (int y = row; y > 1; y--)
            { 
                int upperY = y - 1;
                for (int x = 0; x < levelWidth; x++)
                {
                    UpdatePixel(x, y, pixels[upperY, x]);
                }
            }
        }
    }

    /// <summary>
    /// Выбирает вид новой фигуры.
    /// При небходимости формирует очередь появления фигур.
    /// </summary>
    /// <returns>
    /// Указатель на объект формы фигуры. 
    /// </returns>
    public FigureSkin GetNewFigureSkin()
    {
        // если очередность появления фигур не сформированана,
        // то формируется очередь появления фигур в соответствии с заданной вероятностью их появления
        if (figures.Count == 0)
        {
            figuresList.Clear();
            // фигуры размещаются в соответствии с их вероятностью пояления
            foreach (KeyValuePair<FigureSkin, int> shapeInfo in availableShapes)
            {
                FigureSkin skin = shapeInfo.Key;
                int exp = shapeInfo.Value;
                for (int i = 0; i < exp; i++)
                {
                    figuresList.Add(skin);
                }
            }
            int c = figuresList.Count;

            // фигуры перемешиваются
            for (int k = 0; k < 50; k++)
            {
                for (int i = 0; i < c; i++)
                {
                    int i1 = randomizer.Next(0, c);
                    if (i1 != i)
                    {
                        FigureSkin t = figuresList[i1];
                        figuresList[i1] = figuresList[i];
                        figuresList[i] = t;
                    }
                }
            }
            // формируется очередь 
            foreach (FigureSkin fig in figuresList)
            {
                figures.Enqueue(fig);
            }
        }

        // из очереди извлекается очередная фигура
        return figures.Dequeue();
    }

    /// <summary>
    /// Выполняет поиск заполненных линий в игровом поле. 
    /// Результат поиска заносится в список completedRows.
    /// </summary>
    /// <param name="fromRow">Номер лини, с которого начинается поиск</param>
    /// <param name="rowsCount">Количество проверяемых линий</param>
    protected void FindCompletedRows(int fromRow, int rowsCount)
    {
        completedRows.Clear();
        int maxRow = Mathf.Min(fromRow + rowsCount, levelHeight);

        for (int y = fromRow; y < maxRow; y++)
        {
            int x;
            for (x = 0; x < levelWidth; x++)
            {
                if (pixels[y, x] == 0)
                {
                    break;
                }
            }
            // если все ячейки линии ненулевые, то
            if (x == levelWidth)
            {
                // добавить стрку к списку заполненных линий
                completedRows.Add(y);
            }
        }
    }

    /// <summary>
    /// Возвращает ширину иргового поля
    /// </summary>
    public int Width()
    {
        return levelWidth;
    }

    /// <summary>
    /// Возвращает высоту игрового поля
    /// </summary>
    public int Height()
    {
        return levelHeight;
    }

    // виртуальные методы, требующие переопределения в уровнях ----------------------------------------------------

    /// <summary>
    /// Возвращет индекс цвета яцейки игрового поля
    /// </summary>
    /// <param name="x">Координата X ячейки</param>
    /// <param name="y">Координата Y ячейки</param>
    public abstract int GetPixel(int x, int y);

    /// <summary>
    /// Устанавливает цвет яцейки игрового поля
    /// </summary>
    /// <param name="x">Координата X ячейки</param>
    /// <param name="y">Координата Y ячейки</param>
    /// <param name="value">Индекс цвета</param>
    public abstract void SetPixel(int x, int y, int value);

    /// <summary>
    /// Формирует и возвращает перечень допустимых типов фигур и вероятности их появления
    /// </summary>
    protected abstract Dictionary<FigureSkin, int> GetAvailableShapes();

    /// <summary>
    /// Производит удаление заполненных строк
    /// </summary>
    /// <param name="fromRow">Номер строки, с котрой начинается поиск заполненных строк</param>
    public abstract void RemoveRows(int fromRow);
}

/// <summary>
/// Класс, реализующий 1й уровень игры
/// </summary>
public class Level1 : GameLevel
{
    public Level1() : base() { }

    public override int GetPixel(int x, int y)
    {
        // если координаты ячейки находятся в пределах игрового поля, то
        if (x >= 0 && x < levelWidth &&
            y >= 0 && y < levelHeight)
        {
            // вернуть цвет ячейки
            return pixels[y, x];
        }

        // иначе, вернуть цвет стенки
        return WallValue;
    }

    public override void SetPixel(int x, int y, int value)
    {
        // если координаты ячейки находятся в пределах игрового поля, то
        if (x >= 0 && x < levelWidth &&
            y >= 0 && y < levelHeight)
        {
            // изменить цвет ячейки
            UpdatePixel(x, y, value);
        }
    }

    protected override Dictionary<FigureSkin, int> GetAvailableShapes()
    {
        Dictionary<FigureSkin, int> result = new Dictionary<FigureSkin, int>
        {
            { new FigureO(), 10 },
            { new FigureZ(), 15 },
            { new FigureS(), 15 },
            { new FigureL(), 15 },
            { new FigureJ(), 15 },
            { new FigureI(), 10 },
            { new FigureT(), 20 }
        };
        return result;
    }

    public override void RemoveRows(int fromRow)
    {
        // определяются заполненные линии 
        // от линии fromRow на 4 строки вниз (т.к. максимальная высота фигуры = 4)
        FindCompletedRows(fromRow, 4);
        int count = completedRows.Count;
        // заполненные линии удаляются
        if (count >= 1)
        {
            ShiftRowsDown(completedRows);
            AddScore(count * count);
        }
    }
}

/// <summary>
/// Класс, реализующий 2й уровень игры
/// </summary>
public class Level2 : GameLevel
{

    private readonly List<int> rowsToRemove; // список линий, подлежащих удалению

    public Level2() : base()
    {
        rowsToRemove = new List<int>();
    }

    /// <summary>
    /// Выполняет трансляцию координаты X фигуры при ее выход за границы игрового поля
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private int TranslateX(int x)
    {
        if (x < 0)
        {
            int result = levelWidth - 1 + (x+1) % levelWidth ;
            return result;
        }
        else
        if (x >= levelWidth)
        {
            return x % levelWidth;
        }
        else
        {
            return x;
        }
    }

    public override int GetPixel(int x, int y)
    {
        if (y >= 0 && y < levelHeight)
        {
            return pixels[y, TranslateX(x)];
        }
        else
        {
            return WallValue;
        }
    }

    public override void SetPixel(int x, int y, int value)
    {
        if (y >= 0 && y < levelHeight) 
        {
            UpdatePixel(TranslateX(x), y, value);
        }
    }

    protected override Dictionary<FigureSkin, int> GetAvailableShapes()
    {
        Dictionary<FigureSkin, int> result = new Dictionary<FigureSkin, int>
        {
            { new FigureO(), 10 },
            { new FigureZ(), 15 },
            { new FigureS(), 15 },
            { new FigureL(), 15 },
            { new FigureJ(), 15 },
            { new FigureI(), 10 },
            { new FigureT(), 5 },
            { new FigureX(), 5 },
            { new FigureN(), 5 },
            { new FigureW(), 5 }
        };
        return result;
    }

    public override void RemoveRows(int fromRow)
    {
        // определяются заполненные линии 
        // от линии fromRow на 5 строк вниз (т.к. максимальная высота фигуры = 4, плюс еще одна линия, возможно заполненная ранее)
        FindCompletedRows(fromRow, 5);

        int count = completedRows.Count;
        if (count >= 2)
        {
            int prev = -100;

            // определяются две последовательно расположенные заполненные линии
            foreach (int row in completedRows)
            {
                if (row == prev + 1)
                {
                    rowsToRemove.Clear();
                    rowsToRemove.Add(prev);
                    rowsToRemove.Add(row);
                    ShiftRowsDown(rowsToRemove);
                    AddScore(16);
                    prev = -100;
                }
                else
                {
                    prev = row;
                }
            }
        }
    }
}


