using System;
using System.Collections.Generic;

public readonly struct PlacementResult
{
    private readonly IReadOnlyList<BoardPosition> _clearedCells;

    public bool IsSuccess { get; }
    public BlockData BlockData { get; }
    public int NClearedTimes { get; }
    public IReadOnlyList<BoardPosition> ClearedCells =>
        _clearedCells ?? Array.Empty<BoardPosition>();

    public PlacementResult(
        bool isSuccess,
        BlockData blockData,
        int nClearedTimes = 0,
        IReadOnlyList<BoardPosition> clearedCells = null
    )
    {
        IsSuccess = isSuccess;
        BlockData = blockData;
        NClearedTimes = nClearedTimes;
        _clearedCells = clearedCells;
    }
}
