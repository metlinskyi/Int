public abstract class GridItem
{
    public int X { get; set; }
    public int Y { get; set; }

    protected GridItem(int x, int y)
    {
        X = x;
        Y = y;
    }
}