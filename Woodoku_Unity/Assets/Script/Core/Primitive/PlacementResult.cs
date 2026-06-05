using System;
using System.Collections.Generic;

namespace Script.Core.Primitive
{
    public readonly struct PlacementResult
    {
        private readonly IReadOnlyList<BoardPosition> _clearedCells;

        public bool IsSuccess { get; }
        public BlockShape BlockShape { get; }
        public int NClearedTimes { get; }

        public IReadOnlyList<BoardPosition> ClearedCells =>
            _clearedCells ?? Array.Empty<BoardPosition>();

        public PlacementResult(
            bool isSuccess,
            BlockShape blockshape,
            int nClearedTimes = 0,
            IReadOnlyList<BoardPosition> clearedCells = null
        )
        {
            IsSuccess = isSuccess;
            BlockShape = blockshape;
            NClearedTimes = nClearedTimes;
            _clearedCells = clearedCells;
        }

        public static PlacementResult Failure(BlockShape blockShape = default)
        {
            return new PlacementResult(false, blockShape, 0, Array.Empty<BoardPosition>());
        }
    }
}
