using System;
using System.Collections.Generic;
using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core.Board
{
    public class BoardData : IReadOnlyBoard, IBoardEventPublisher
    {
        private readonly CellState[,] _board;

        public BoardData(int gridSize)
        {
            GridSize = gridSize;
            BoardSize = GridSize * GridSize;
            NGrids = BoardSize / GridSize;
            _board = new CellState[BoardSize, BoardSize];
        }

        public event EventHandler<CellUpdateData> CellUpdate;

        public int GridSize { get; }
        public int BoardSize { get; }
        public int NGrids { get; }

        public CellState GetCell(BoardPosition boardPosition)
        {
            return this.Contains(boardPosition) ? _board[boardPosition.x, boardPosition.y] : CellState.OutOfBoard;
        }

        public CellState GetCell(int x, int y)
        {
            return GetCell(new BoardPosition(x, y));
        }

        public BoardSnapShot ToSnapShot()
        {
            return new BoardSnapShot(GridSize, _board);
        }

        public void Reset()
        {
            for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++)
                SetCell(x, y, CellState.Empty);
        }

        private void SetCell(BoardPosition boardPosition, CellState state = CellState.Filled)
        {
            int x = boardPosition.x;
            int y = boardPosition.y;
            if (this.Contains(boardPosition))
            {
                if (_board[x, y] == state)
                    return;

                _board[x, y] = state;
                CellUpdate?.Invoke(this, new CellUpdateData(x, y, state));
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"Invalid board index: {x}, {y}",
                    nameof(boardPosition)
                );
            }
        }

        private void SetCell(int x, int y, CellState state = CellState.Filled)
        {
            BoardPosition boardPosition = new(x, y);
            SetCell(boardPosition, state);
        }

        public PlacementResult TryPlaceBlock(PlacementAction action)
        {
            bool canPlace = BoardRule.CanPlaceBlock(this, action);
            if (canPlace)
            {
                PlacementResult result = PlaceBlockAndClear(action);
                return result;
            }

            return PlacementResult.Failure(action.Shape);
        }

        private PlacementResult PlaceBlockAndClear(PlacementAction action)
        {
            // called after validation
            foreach (BoardPosition pos in action.FilledPositions) SetCell(pos);

            (int nClearedTimes, List<BoardPosition> cellsToClearList) = BoardRule.GetCellsToClear(this);

            ClearCells(cellsToClearList);

            return new PlacementResult(true, action.Shape, nClearedTimes, cellsToClearList);
        }

        private void ClearCells(IEnumerable<BoardPosition> cellsToClear)
        {
            foreach (BoardPosition pos in cellsToClear) SetCell(pos, CellState.Empty);
        }
    }
}
