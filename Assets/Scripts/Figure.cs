/// <summary>
/// Интерфейс, обеспечивающий доступ к модели игрового поля
/// </summary>
public interface ILevelCanvas
{
    int Width();                            // возвращает ширину игрового поля
    int Height();                           // возвращает высоту игрового поля
    int GetPixel(int x, int y);             // возвращает значение цвета в ячейке игрового поля, заданной координатами x, y
    void SetPixel(int x, int y, int value); // устанавливает значение цвета в ячейке игрового поля, заданной координатами x, y
}

/// <summary>
/// Базовый класс фигуры
/// </summary>
public class Figure
{
    private ILevelCanvas levelCanvas = null;            // указатель на интерфейс игрового уровня
    private int rotation;                               // угол поворота град.
    private int left;                                   // координата X верхнего левого угла фигуры
    private int top;                                    // координата Y верхнего левого угла фигуры
    private FigureSkin skin;                            // внешний вид фигуры

    /// <summary>
    /// Тип делегата, выполняиемого при перечислении ячеек фигуры
    /// </summary>
    /// <param name="x">координата X ячейки фигуры</param>
    /// <param name="y">координата X ячейки фигуры</param>
    /// <param name="sx">координата X ячейки игрового поля</param>
    /// <param name="sy">координата Y ячейки игрового поля</param>
    private delegate void CellProgram(int x, int y, int sx, int sy);

    public Figure()
    {
        levelCanvas = null;
        skin = null;
        rotation = 0;
    }

    // Возвращают координаты верхнего левого угла фигуры в игровом поле
    public int Left { get { return left; } }    // координата X 
    public int Top { get { return top; } }    // координата Y

    /// <summary>
    /// Формирует исходное состояние фигуры
    /// </summary>
    /// <returns>
    /// true - если фигура не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    public void Init(ILevelCanvas canvas, FigureSkin s)
    {
        levelCanvas = canvas;
        skin = s;
        
        if (skin != null)
        {
            SetRotation(0);
            left = levelCanvas.Width() / 2;
            top = 0;
            if (!HasCollisions())
            {
                Draw();
            }
            else
            {
                Fix();
            }
        }
    }

    public void Clear()
    {
        skin = null;
    }

    public bool IsEmpty() 
    { 
        return skin == null; 
    }

    /// <summary>
    /// Выполняет попытку поворота фигуры на 90 град. против часовой стрелки
    /// </summary>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    public bool RotateCCW()
    {
        if (skin != null)
        {
            return Rotate(-90);
        }
        return false;
    }

    /// <summary>
    /// Выполняет попытку поворота фигуры на 90 град. по часовой стрелке
    /// </summary>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    public bool RotateCW()
    {
        if (skin != null)
        {
            return Rotate(90);
        }
        return false;
    }

    /// <summary>
    /// Выполняет попытку смещения фигуры на одну линию вниз.
    /// </summary>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    public bool MoveDown()
    {
        if (skin != null)
        {
            if (SetPosition(left, top + 1))
            {
                return true;
            }

            // нарисовать фигуру в новом состоянии
            Fix();
        }
        return false;
    }

    /// <summary>
    /// Выполняет попытку смещения фигуры влево
    /// </summary>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    public bool MoveLeft()
    {
        if (skin != null)
        {
            return SetPosition(left - 1, top);
        } 
        return false;
    }

    /// <summary>
    /// Выполняет попытку смещения фигуры вправо
    /// </summary>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    public bool MoveRight()
    {
        if (skin != null)
        {
            return SetPosition(left + 1, top);
        }
        return false;
    }

    /// <summary>
    /// Выполняет проверку пересечения фигуры с занятыми ячейками игрового поля.
    /// </summary>
    /// <returns>
    /// true - если фигура пересекается занятыми ячейками игрового поля.
    /// false - в противном случае.
    /// </returns>
    private bool HasCollisions()
    {
        if (skin != null)
        {
            for (int y = 0, sy = top; y < 4; y++, sy++)
            {
                for (int x = 0, sx = left; x < 4; x++, sx++)
                {
                    // проходить только по блокам, относящимся к фигуре
                    if (skin.cells[y, x] != 0)
                    {
                        // блоки стен имеют значение 1000,
                        // занятые блоки имеют значение цвета > 0
                        // блоки, занятые текущей фигурой имеют отрицательное значение цвета
                        // свободные блоки имеют значение 0
                        if (levelCanvas.GetPixel(sx, sy) > 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Устанавливает фигуру в положение с новыми координатами
    /// </summary>
    /// <param name="x">координата x</param>
    /// <param name="y">координата y</param>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля,
    /// false - в противном случае
    /// </returns>
    private bool SetPosition(int x, int y)
    {
        // стереть фигуру в текущем состоянии
        Erase();
        int saveX = left;
        int saveY = top;
        left = x;
        top = y;

        bool collisions = HasCollisions();
        if (collisions)
        {
            left = saveX;
            top = saveY;
        }
        // нарисовать фигуру в новом состоянии
        Draw();
        return !collisions;
    }

    /// <summary>
    /// Производит поворот фигуры на указанный угол
    /// </summary>
    /// <returns>
    /// true - если фигура в новом положении не имеет пересечений с занятыми ячейками игрового поля.
    /// false - в противном случае
    /// </returns>
    private bool Rotate(int delta)
    {
        // стереть фигуру в текущем состоянии
        Erase();

        int saveRotation = rotation;
        SetRotation(rotation + delta);
        bool collisions = HasCollisions();
        if (collisions)
        {
            SetRotation(saveRotation);
        }
        // нарисовать фигуру в новом состоянии
        Draw();
        return !collisions;
    }

    /// <summary>
    /// Меняет скин фигуры в соответствии с заданным углом поворота
    /// </summary>
    private void SetRotation(int value)
    {
        if (value < 0)
            rotation = 270;
        else
        if (value > 270)
            rotation = 0;
        else
            rotation = value;

        skin.SetRotation(rotation);
    }

    /// <summary>
    /// Стирает фигуру в текщей позиции
    /// </summary>
    private void Erase()
    {
        if (skin != null)
        {
            ForEachCells(delegate (int x, int y, int sx, int sy)
            {
                levelCanvas.SetPixel(sx, sy, 0);
            });
        }
    }

    /// <summary>
    /// Рисует фигуру в текущей позиции отрицательным цветом для исключения самонаезда
    /// </summary>
    private void Draw()
    {
        if (skin != null)
        {
            ForEachCells(delegate (int x, int y, int sx, int sy)
            {
                levelCanvas.SetPixel(sx, sy, -skin.cells[y, x]);
            });
        }
    }

    // зафиксировать упавшую фмгуру

    /// <summary>
    /// Рисует фигуру в текущей позиции
    /// </summary>
    private void Fix()
    {
        if (skin != null)
        {
            ForEachCells(delegate (int x, int y, int sx, int sy)
            {
                levelCanvas.SetPixel(sx, sy, skin.cells[y, x]);
            });
            skin = null;
        }
    }

    /// <summary>
    /// Выполняет перебор ячеек фигуры
    /// </summary>
    /// <param name="program">Вызываемый делегат</param>
    private void ForEachCells(CellProgram program)
    {
        for (int y = 0, sy = top; y < 4; y++, sy++)
        {
            for (int x = 0, sx = left; x < 4; x++, sx++)
            {
                // проходить только по блокам, относящимся к фигуре
                if (skin.cells[y, x] != 0)
                {
                    program(x, y, sx, sy);
                }
            }
        }
    }
}
