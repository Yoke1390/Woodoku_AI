using System;
using System.Collections.Generic;
using Script.Core.Primitive;

namespace Script.Core
{
    public interface IReadOnlyHands
    {
        IReadOnlyList<BlockShape?> CurrentHand { get; }
        int NSlots { get; }
        event Action<int> HandBlockConsumed;
        event Action<int, BlockShape> HandBlockGenerated;
    }

    public interface IReadOnlyBoard
    {
        int GridSize { get; }
        int BoardSize { get; }
        int NGrids { get; }

        CellState GetCell(BoardPosition boardPosition);
        IEnumerable<PlacementAction> EnumerateLegalActions(BlockShape shape);
        event EventHandler<CellUpdateData> CellUpdate;
    }

    public interface IReadOnlyScore
    {
        int Score { get; }
        event Action<int> ScoreUpdate;
    }
}