using System;
using System.Collections.Generic;

public interface IReadOnlyHands
{
    IReadOnlyList<BlockShape?> CurrentHand { get; }
    event Action<int, BlockShape> HandBlockGenerated;
}

public interface IReadOnlyBoard
{
    int GridSize { get; }
    int BoardSize { get; }
    int NGrids { get; }

    CellState GetCell(BoardPosition boardPosition);
    event EventHandler<CellUpdateData> CellUpdate;
}
