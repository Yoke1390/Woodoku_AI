using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core
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
            return this.IsInBoard(boardPosition) ? _board[boardPosition.x, boardPosition.y] : CellState.OutOfBoard;
        }

        public CellState GetCell(int x, int y)
        {
            return GetCell(new BoardPosition(x, y));
        }

        public IEnumerable<PlacementAction> EnumerateLegalActions(BlockShape blockShape)
        {
            for (int x = 0; x < BoardSize - blockShape.MaxX; x++)
            for (int y = 0; y < BoardSize - blockShape.MaxY; y++)
            {
                BoardPosition blockBaseBoardPosition = new(x, y);
                PlacementAction action = new(blockBaseBoardPosition, blockShape);
                if (CanPlaceBlock(action)) yield return action;
            }
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

        public void SetCell(BoardPosition boardPosition, CellState state = CellState.Filled)
        {
            int x = boardPosition.x;
            int y = boardPosition.y;
            if (this.IsInBoard(boardPosition))
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

        public void SetCell(int x, int y, CellState state = CellState.Filled)
        {
            BoardPosition boardPosition = new(x, y);
            SetCell(boardPosition, state);
        }

        public bool CanPlaceBlockInBoard(BlockShape blockShape)
        {
            return EnumerateLegalActions(blockShape).Any();
        }

        public bool CanPlaceBlock(PlacementAction action)
        {
            foreach (BlockOffset cellOffset in action.Shape.Blocks)
            {
                BoardPosition targetPos = action.BasePosition + cellOffset;
                if (GetCell(targetPos) != CellState.Empty)
                    // OutOfBoard or Filled
                    return false;
            }

            return true;
        }

        public PlacementResult TryPlaceBlock(PlacementAction action)
        {
            bool canPlace = CanPlaceBlock(action);
            if (canPlace)
            {
                PlacementResult result = PlaceBlockAndClear(action);
                return result;
            }

            return PlacementResult.Failure(action.Shape);
        }

        private PlacementResult PlaceBlockAndClear(PlacementAction action)
        {
            foreach (BlockOffset blockOffset in action.Shape.Blocks)
            {
                // called after validation
                BoardPosition pos = action.BasePosition + blockOffset;
                SetCell(pos);
            }

            (int nClearedTimes, List<BoardPosition> cellsToClearList) = GetCellsToClear();

            ClearCells(cellsToClearList);

            return new PlacementResult(true, action.Shape, nClearedTimes, cellsToClearList);
        }

        private void ClearCells(IEnumerable<BoardPosition> cellsToClear)
        {
            foreach (BoardPosition pos in cellsToClear) SetCell(pos, CellState.Empty);
        }

        private (int nClearedTimes, List<BoardPosition> cellsToClearList) GetCellsToClear()
        {
            int nClearedTimes = 0;
            HashSet<BoardPosition> cellsToClearSet = new();

            void AddClearCellsAndCount(IReadOnlyCollection<BoardPosition> newSet)
            {
                if (newSet.Count > 0)
                {
                    nClearedTimes++;
                    cellsToClearSet.UnionWith(newSet);
                }
            }

            for (int x = 0; x < BoardSize; x++) AddClearCellsAndCount(GetCellsToClearWithX(x));
            for (int y = 0; y < BoardSize; y++) AddClearCellsAndCount(GetCellsToClearWithY(y));
            for (int gridX = 0; gridX < NGrids; gridX++)
            for (int gridY = 0; gridY < NGrids; gridY++)
                AddClearCellsAndCount(GetCellsToClearWithGrid(gridX, gridY));

            List<BoardPosition> cellsToClearList = cellsToClearSet.ToList();

            return (nClearedTimes, cellsToClearList);
        }

        private IReadOnlyCollection<BoardPosition> GetCellsToClearWithX(int x)
        {
            if (x < 0 || BoardSize <= x) return Array.Empty<BoardPosition>();

            for (int y = 0; y < BoardSize; y++)
                if (GetCell(x, y) != CellState.Filled)
                    return Array.Empty<BoardPosition>();

            HashSet<BoardPosition> cellsToBeCleared = new();
            for (int y = 0; y < BoardSize; y++) cellsToBeCleared.Add(new BoardPosition(x, y));
            return cellsToBeCleared;
        }

        private IReadOnlyCollection<BoardPosition> GetCellsToClearWithY(int y)
        {
            if (y < 0 || BoardSize <= y) return Array.Empty<BoardPosition>();

            for (int x = 0; x < BoardSize; x++)
                if (GetCell(x, y) != CellState.Filled)
                    return Array.Empty<BoardPosition>();

            HashSet<BoardPosition> cellsToBeCleared = new();
            for (int x = 0; x < BoardSize; x++) cellsToBeCleared.Add(new BoardPosition(x, y));

            return cellsToBeCleared;
        }

        private IReadOnlyCollection<BoardPosition> GetCellsToClearWithGrid(int gridX, int gridY)
        {
            if (gridX < 0 || NGrids <= gridX || gridY < 0 || NGrids <= gridY) return Array.Empty<BoardPosition>();

            for (int offsetX = 0; offsetX < GridSize; offsetX++)
            for (int offsetY = 0; offsetY < GridSize; offsetY++)
            {
                int targetX = GridSize * gridX + offsetX;
                int targetY = GridSize * gridY + offsetY;
                if (GetCell(targetX, targetY) != CellState.Filled) return Array.Empty<BoardPosition>();
            }

            HashSet<BoardPosition> cellsToBeCleared = new();
            for (int offsetX = 0; offsetX < GridSize; offsetX++)
            for (int offsetY = 0; offsetY < GridSize; offsetY++)
            {
                int targetX = GridSize * gridX + offsetX;
                int targetY = GridSize * gridY + offsetY;
                cellsToBeCleared.Add(new BoardPosition(targetX, targetY));
            }

            return cellsToBeCleared;
        }

        public bool TryOffset(
            BoardPosition boardPosition,
            BlockOffset blockOffset,
            out BoardPosition newBoardPosition
        )
        {
            int x = boardPosition.x + blockOffset.x;
            int y = boardPosition.y + blockOffset.y;
            newBoardPosition = new BoardPosition(x, y);
            if (this.IsInBoard(newBoardPosition)) return true;

            newBoardPosition = default;
            return false;
        }
    }
}