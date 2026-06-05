using System;
using Script.Core.Primitive;

namespace Script.Core.Interfaces
{
    public interface IReadOnlyBoard
    {
        int GridSize { get; }
        int BoardSize { get; }
        int NGrids { get; }

        CellState GetCell(BoardPosition boardPosition);
        CellState GetCell(int x, int y);
    }

    public interface IBoardEventPublisher
    {
        event EventHandler<CellUpdateData> CellUpdate;
    }
}
