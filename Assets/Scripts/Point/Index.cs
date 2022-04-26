public struct Index
{
    public int x;
    public int y;

    public Index(int y, int x)
    {
        this.y = y;
        this.x = x;
    }

    public bool IsNegative()
    {
        return y < 0 || x < 0;
    }

    public bool IsControl()
    {
        return y < 0 && x > 0;
    }

    public bool IsEqual(Index i)
    {
        return y == i.y && x == i.x;
    }

    public static Index negative
    {
        get
        {
            return new Index(-1, -1);
        }
    }
}