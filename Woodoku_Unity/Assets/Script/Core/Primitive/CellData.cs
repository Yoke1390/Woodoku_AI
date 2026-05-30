// for performance, sbyte can be considered
public enum CellState
{
    Empty = 0,
    Filled = 1,
    OutOfBoard = -1,
}

public readonly struct CellUpdateData
{
    public int X { get; }
    public int Y { get; }
    public CellState State { get; }

    public CellUpdateData(int x, int y, CellState state)
    {
        X = x;
        Y = y;
        State = state;
    }
}
