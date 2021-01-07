/// <summary>
/// Базовый класс скина фигуры
/// </summary>
public class FigureSkin
{
    // ячейки фигуры, соответствующие различным углам поворота
    protected int[,] cells0;
    protected int[,] cells90;
    protected int[,] cells180;
    protected int[,] cells270;

    // текущие ячейки фигуры 
    public int[,] cells;
    public void SetRotation(int angle)
    {
        switch (angle)
        {
            case 0:
                cells = cells0; break;
            case 90:
                cells = cells90; break;
            case 180:
                cells = cells180; break;
            case 270:
                cells = cells270; break;
        }
    }
}

// основные фигуры -------------------------------------------------------------------------------------
public class FigureJ : FigureSkin
{
    public FigureJ()
    {
        cells0 = new int[4, 4] { { 1, 0, 0, 0 }, { 2, 1, 2, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 0, 2, 1, 0 }, { 0, 1, 0, 0 }, { 0, 2, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 0, 0, 0, 0 }, { 2, 1, 2, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 0, 2, 0, 0 }, { 0, 1, 0, 0 }, { 1, 2, 0, 0 }, { 0, 0, 0, 0 } };
    }
}


public class FigureI : FigureSkin
{
    public FigureI()
    {
        cells0 = new int[4, 4] { { 0, 0, 0, 0 }, { 2, 1, 2, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 0, 2, 0, 0 }, { 0, 1, 0, 0 }, { 0, 2, 0, 0 }, { 0, 1, 0, 0 } };
        cells180 = new int[4, 4] { { 0, 0, 0, 0 }, { 1, 2, 1, 2 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 0, 1, 0, 0 }, { 0, 2, 0, 0 }, { 0, 1, 0, 0 }, { 0, 2, 0, 0 } };
    }
}

public class FigureO : FigureSkin
{
    public FigureO()
    {
        cells0 = new int[4, 4] { { 1, 2, 0, 0 }, { 2, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 2, 1, 0, 0 }, { 1, 2, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = cells0;
        cells270 = cells90;
    }
}

public class FigureZ : FigureSkin
{
    public FigureZ()
    {
        cells0 = new int[4, 4] { { 2, 1, 0, 0 }, { 0, 2, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 0, 2, 0, 0 }, { 2, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 1, 2, 0, 0 }, { 0, 1, 2, 0 }, { 0, 0, 0, 0 } , { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 0, 1, 0, 0 }, { 1, 2, 0, 0 }, { 2, 0, 0, 0 }, { 0, 0, 0, 0 } };

    }
}


public class FigureS : FigureSkin
{
    public FigureS()
    {
        cells0 = new int[4, 4] { { 0, 1, 2, 0 }, { 1, 2, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 1, 0, 0, 0 }, { 2, 1, 0, 0 }, { 0, 2, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 0, 2, 1, 0 }, { 2, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 2, 0, 0, 0 }, { 1, 2, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } };
    }
}

public class FigureL : FigureSkin
{
    public FigureL()
    {
        cells0 = new int[4, 4] { { 0, 0, 1, 0 }, { 2, 1, 2, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 0, 2, 0, 0 }, { 0, 1, 0, 0 }, { 0, 2, 1, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 0, 0, 0, 0 }, { 2, 1, 2, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 1, 2, 0, 0 }, { 0, 1, 0, 0 }, { 0, 2, 0, 0 }, { 0, 0, 0, 0 } };
    }
}

public class FigureT : FigureSkin
{
    public FigureT()
    {
        cells0 = new int[4, 4] { { 0, 1, 0, 0 }, { 1, 2, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 0, 1, 0, 0 }, { 0, 2, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 1, 2, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 0, 1, 0, 0 }, { 1, 2, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } };
    }
}


// дополнительне фигуры -------------------------------------------------------------------------------------
public class FigureX : FigureSkin
{
    public FigureX()
    {
        cells0 = new int[4, 4] { { 0, 1, 0, 0 }, { 1, 2, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = cells0;
        cells180 = cells0;
        cells270 = cells0;
    }
}

public class FigureN : FigureSkin
{
    public FigureN()
    {
        cells0 = new int[4, 4] { { 1, 2, 1, 0 }, { 2, 0, 2, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 2, 1, 0, 0 }, { 0, 2, 0, 0 }, { 2, 1, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 2, 0, 2, 0 }, { 1, 2, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 1, 2, 0, 0 }, { 2, 0, 0, 0 }, { 1, 2, 0, 0 }, { 0, 0, 0, 0 } };
    }
}

public class FigureW : FigureSkin
{
    public FigureW()
    {
        cells0 = new int[4, 4] { { 2, 0, 0, 0 }, { 1, 2, 0, 0 }, { 0, 1, 2, 0 }, { 0, 0, 0, 0 } };
        cells90 = new int[4, 4] { { 0, 1, 2, 0 }, { 1, 2, 0, 0 }, { 2, 0, 0, 0 }, { 0, 0, 0, 0 } };
        cells180 = new int[4, 4] { { 2, 1, 0, 0 }, { 0, 2, 1, 0 }, { 0, 0, 2, 0 }, { 0, 0, 0, 0 } };
        cells270 = new int[4, 4] { { 0, 0, 2, 0 }, { 0, 2, 1, 0 }, { 2, 1, 0, 0 }, { 0, 0, 0, 0 } };
    }
}
