using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core.Board
{
    public static class BoardRule
    {
        public static IEnumerable<PlacementAction> EnumerateLegalActions(IReadOnlyBoard board, BlockShape blockShape)
        {
            for (int x = 0; x < board.BoardSize - blockShape.MaxX; x++)
            for (int y = 0; y < board.BoardSize - blockShape.MaxY; y++)
            {
                BoardPosition blockBaseBoardPosition = new(x, y);
                PlacementAction action = new(blockBaseBoardPosition, blockShape);
                if (CanPlaceBlock(board, action)) yield return action;
            }
        }

        public static bool CanPlaceBlock(IReadOnlyBoard board, PlacementAction action)
        {
            foreach (BlockOffset cellOffset in action.Shape.Blocks)
            {
                BoardPosition targetPos = action.BasePosition + cellOffset;
                if (board.GetCell(targetPos) != CellState.Empty)
                    // OutOfBoard or Filled
                    return false;
            }

            return true;
        }

        public static (int nClearedTimes, List<BoardPosition> cellsToClearList) GetCellsToClear(IReadOnlyBoard board)
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

            for (int x = 0; x < board.BoardSize; x++) AddClearCellsAndCount(GetCellsToClearWithX(x, board));
            for (int y = 0; y < board.BoardSize; y++) AddClearCellsAndCount(GetCellsToClearWithY(y, board));

            for (int gridX = 0; gridX < board.NGrids; gridX++)
            for (int gridY = 0; gridY < board.NGrids; gridY++)
                AddClearCellsAndCount(GetCellsToClearWithGrid(gridX, gridY, board));

            List<BoardPosition> cellsToClearList = cellsToClearSet.ToList();

            return (nClearedTimes, cellsToClearList);
        }

        private static IReadOnlyCollection<BoardPosition> GetCellsToClearWithX(int x, IReadOnlyBoard board)
        {
            if (x < 0 || board.BoardSize <= x) return Array.Empty<BoardPosition>();

            for (int y = 0; y < board.BoardSize; y++)
                if (board.GetCell(x, y) != CellState.Filled)
                    return Array.Empty<BoardPosition>();

            HashSet<BoardPosition> cellsToBeCleared = new();
            for (int y = 0; y < board.BoardSize; y++) cellsToBeCleared.Add(new BoardPosition(x, y));
            return cellsToBeCleared;
        }

        private static IReadOnlyCollection<BoardPosition> GetCellsToClearWithY(int y, IReadOnlyBoard board)
        {
            if (y < 0 || board.BoardSize <= y) return Array.Empty<BoardPosition>();

            for (int x = 0; x < board.BoardSize; x++)
                if (board.GetCell(x, y) != CellState.Filled)
                    return Array.Empty<BoardPosition>();

            HashSet<BoardPosition> cellsToBeCleared = new();
            for (int x = 0; x < board.BoardSize; x++) cellsToBeCleared.Add(new BoardPosition(x, y));

            return cellsToBeCleared;
        }

        private static IReadOnlyCollection<BoardPosition> GetCellsToClearWithGrid(int gridX, int gridY,
            IReadOnlyBoard board)
        {
            if (gridX < 0 || board.NGrids <= gridX || gridY < 0 || board.NGrids <= gridY)
                return Array.Empty<BoardPosition>();

            for (int offsetX = 0; offsetX < board.GridSize; offsetX++)
            for (int offsetY = 0; offsetY < board.GridSize; offsetY++)
            {
                int targetX = board.GridSize * gridX + offsetX;
                int targetY = board.GridSize * gridY + offsetY;
                if (board.GetCell(targetX, targetY) != CellState.Filled) return Array.Empty<BoardPosition>();
            }

            HashSet<BoardPosition> cellsToBeCleared = new();
            for (int offsetX = 0; offsetX < board.GridSize; offsetX++)
            for (int offsetY = 0; offsetY < board.GridSize; offsetY++)
            {
                int targetX = board.GridSize * gridX + offsetX;
                int targetY = board.GridSize * gridY + offsetY;
                cellsToBeCleared.Add(new BoardPosition(targetX, targetY));
            }

            return cellsToBeCleared;
        }
    }
}
