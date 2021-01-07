using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Класс для хранения информации об уровне игрв
/// </summary>
public struct LevelInfo
{
    public int width;               // ширина игрового поля
    public int height;              // высота игрового поля
    public string levelClassName;   // имя класса уровня
    public GameLevel level;         // указатель на объект уровня

    public LevelInfo(int w, int h, string c)
    {
        width = w;
        height = h;
        levelClassName = c;
        level = null;
    }
}

/// <summary>
///  Класс CameraScript.
///  Подключается в качестве компонента к камере сцены.
/// </summary>
public class CameraScript : MonoBehaviour, ILevelScene
{
    // данные игровых уровней 
    private static readonly LevelInfo[] levels =
    {
        new LevelInfo(10, 20, "Level1"),
        new LevelInfo(12, 20, "Level2"),
    };

    private int levelWidth;                 // ширина игрового поля
    private int levelHeight;                // высота игрового поля

    public Sprite[] colorSprites;           // массив спрайтов
    public Sprite wallSprite;               // спрайт стенки

    private int levelNumber;                // номер текущего уровня
    private GameLevel level;                // объект, управляющий логикой игрового уровня
    private Figure figure;                  // фигура в игровом поле
    private Tilemap tilemap;
    private Tile[] tiles;                   // массив тайлов для заполнения игрового поля
    private Tile wallTile;                  // тайл блока стены

    private Vector3Int[,] cellPositions;    // массив координат ячеек игрового поля

    private const int startSpeed = 50;      // начальная скорость падения фигур (определяет количество кадров, необходимых для смещения фигуры вниз)
    private const int fastSpeed = 1;        // скорость быстрого падения фигур
    private const int speedupScoreStep = 20; // количество очков, которое необходимо набрать для увеличения скорости

    private int speed;                      // текущая скорость падения фигур
    private int framesCount;                // счетчик кадров
    private int score;                      // сцетчик очков
    private int speedupScore;               // счет, при котором произойдет увеличение скорости падения фигур

    // координаты элеметнов GUI
    private Rect btNextLevelRect;
    private Rect btStartRect;
    private Rect lbScoreRect;
    private Rect lbSpeedRect;

    void Awake()
    {
        tilemap = FindObjectOfType<Tilemap>();
        if (tilemap == null)
            return;

        // подготовить тайлы
        tiles = new Tile[colorSprites.Length];
        for (int i = 0; i < colorSprites.Length; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = colorSprites[i];
            tiles[i] = tile;
        }

        wallTile = ScriptableObject.CreateInstance<Tile>();
        wallTile.sprite = wallSprite;

        // установить координаты кнопок
        btNextLevelRect = new Rect(0, 0, 120, 27);
        btStartRect = new Rect(0, 30, 120, 27);
        int left = Screen.width - 121;
        lbScoreRect = new Rect(left, 0, 120, 27);
        lbSpeedRect = new Rect(left, 30, 120, 27);

        InitLevel(0);
    }

    /// <summary>
    /// Приводит игровой уровень в исходное состояние
    /// </summary>
    private void InitLevel(int ln)
    {
        // выбираем следующий уровень
        if (ln >= 0 && ln < levels.Length)
        {
            levelNumber = ln;
        }
        else
        {
            levelNumber = 0;
        }

        LevelInfo li = levels[levelNumber];
        if (levelWidth != li.width || levelHeight != li.height)
        {
            levelWidth = li.width;
            levelHeight = li.height;
            cellPositions = new Vector3Int[levelHeight, levelWidth];
        }

        // рисуется пустой уровень
        RedrawLevel();

        // если уровень еще не создан, то
        level = levels[levelNumber].level;
        if (level == null)
        {
            // создаем экземпляр
            level = GameLevel.CreateInstance(li.levelClassName);
            if (level == null)
            {
                return;
            }
            // сохраняем 
            levels[levelNumber].level = level;
        }

        // уровень приводится в исходное состояние
        level.Init(levelWidth, levelHeight, this);

        // создается пустая фигура
        if (figure == null)
        {
            figure = new Figure();
        }
        else
        {
            figure.Clear();
        }

        speed = startSpeed;
        score = 0;
        speedupScore = speedupScoreStep;
        framesCount = 0;
    }


    /// <summary>
    /// Рисует уровень в исходном состоянии
    /// </summary>
    private void RedrawLevel()
    {
        tilemap.ClearAllTiles();
        int cameraSize = (int)Camera.main.orthographicSize;
        int w2 = levelWidth / 2;

        int cellY = cameraSize;
        for (int y = 0; y < levelHeight; y++, cellY--)
        {
            // рисуем боковые стенки 
            tilemap.SetTile(new Vector3Int(-w2 - 1, cellY, 0), wallTile);
            tilemap.SetTile(new Vector3Int(-w2 + levelWidth, cellY, 0), wallTile);
            // вычисляются координаты внутренних блоков
            for (int x = 0; x < levelWidth; x++)
            {
                Vector3Int v = new Vector3Int(-w2 + x, cellY, 0);
                cellPositions[y, x] = v;
            }
        }

        // рисуем нижнюю стенку
        for (int x = -1; x < levelWidth + 1; x++)
        {
            tilemap.SetTile(new Vector3Int(-w2 + x, cellY, 0), wallTile);
        }

    }

    /// <summary>
    /// Устанавливает новый внешний вид фигуры в соответствии с правилами текущего уровня
    /// </summary>
    private void CreateNewShape()
    {
        // выбирается случайный скин для фигуры
        FigureSkin skin = level.GetNewFigureSkin();
        if (skin != null)
            figure.Init(level, skin);
    }

    void Update()
    {
        if (!figure.IsEmpty())
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                figure.RotateCW();
            }
            else
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                figure.RotateCCW();
            }
            else
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                figure.MoveLeft();
            }
            else
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                figure.MoveRight();
            }
        }
    }

    void FixedUpdate()
    {
        // если в игровом поле присутствует фигура, то
        if (!figure.IsEmpty())
        {
            // обрабаывается нажатие "пробела" для ускоренного падения фигуры
            int shapeSpeed = Input.GetKey(KeyCode.Space) ? fastSpeed : speed;

            // производится подсчет кадров для перемещения фигуры вниз
            framesCount++;
            if (framesCount > shapeSpeed)
            {
                framesCount = 0;
                // если смещение вниз невозможно
                if (!figure.MoveDown())
                {
                    // удалить заполненные линии
                    level.RemoveRows(figure.Top);
                    // создать новую фигуру
                    CreateNewShape();
                }
            }
        }
    }

    private void OnGUI()
    {
        // кнопка перехода на следующий уровень
        if (GUI.Button(btNextLevelRect, "Next level"))
        {
            InitLevel(levelNumber + 1);
        }

        // кнопка запуска игры
        if (figure.IsEmpty())
        {
            if (GUI.Button(btStartRect, "Start"))
            {
                InitLevel(levelNumber);
                CreateNewShape();
            }
        }

        // отображаем счет
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.Label(lbScoreRect, "Score: " + score.ToString());
        GUI.Label(lbSpeedRect, "Speed: " + (startSpeed - speed + 1).ToString());
    }

    // реализация интерфейса ILevelListener -----------------------------------
    //
    public void UpdateCell(int x, int y, int value)
    {
        if (y >= 0 && y < levelHeight && 
            x >= 0 && x < levelWidth)
        {
            int v = value < tiles.Length ? value : 0;
            tilemap.SetTile(cellPositions[y, x], tiles[Mathf.Abs(v)]);
        }
    }

    public void AddScore(int delta)
    {
        score += delta;

        // При увеличении счета прозводится постепенное увеличение скорости падения фигур
        if (speed > fastSpeed && score >= speedupScore)
        {
            speed--;
            speedupScore += speedupScoreStep;
        }
    }
    //
    // ---------------------------------------------------------------------------
}
