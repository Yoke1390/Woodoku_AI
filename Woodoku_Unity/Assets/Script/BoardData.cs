using System;
using System.Collections.Generic;

public class BoardData
{
    public int GridSize { get; }
    public int BoardSize { get; }

    // for performance, sbyte can be considered
    public enum CellState
    {
        Empty = 0,
        Filled = 1,
        OutOfBoard = -1,
    }

    private CellState[,] board;

    public event EventHandler<CellUpdateData> CellUpdate;

    public BoardData(int gridSize)
    {
        GridSize = gridSize;
        BoardSize = GridSize * GridSize;
        Reset();
    }

    public void Reset()
    {
        board = new CellState[BoardSize, BoardSize];
    }

    public CellState GetCell(BoardPosition boardPosition)
    {
        if (IsInBoard(boardPosition))
        {
            return board[boardPosition.x, boardPosition.y];
        }
        else
        {
            return CellState.OutOfBoard;
        }
    }

    public CellState GetCell(int x, int y)
    {
        return GetCell(new BoardPosition(x, y));
    }

    public void SetCell(BoardPosition boardPosition, CellState state = CellState.Filled)
    {
        int x = boardPosition.x;
        int y = boardPosition.y;
        if (IsInBoard(boardPosition))
        {
            board[x, y] = state;
            CellUpdate?.Invoke(this, new CellUpdateData(x, y, state));
        }
        else
        {
            throw new IndexOutOfRangeException($"Invalid board index: {x}, {y}");
        }
    }

    public void SetCell(int x, int y, CellState state = CellState.Filled)
    {
        BoardPosition boardPosition = new(x, y);
        SetCell(boardPosition, state);
    }

    public bool CanPlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        foreach (BlockOffset cellOffset in blockData.BlockCells)
        {
            BoardPosition targetPos = blockBaseBoardPosition + cellOffset;
            if (GetCell(targetPos) != CellState.Empty)
            {
                // OutOfBoard or Filled
                return false;
            }
        }
        return true;
    }

    public bool TryPlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        bool canPlace = CanPlaceBlock(blockData, blockBaseBoardPosition);
        if (canPlace)
        {
            PlaceBlockAndClear(blockData, blockBaseBoardPosition);
        }
        return canPlace;
    }

    private void PlaceBlockAndClear(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        HashSet<BoardPosition> cellsToBeCleared = new();
        foreach (BlockOffset blockOffset in blockData.BlockCells)
        {
            // called after validation
            BoardPosition pos = blockBaseBoardPosition + blockOffset;
            SetCell(pos, CellState.Filled);
        }

        ClearBlocks();
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
        if (IsInBoard(newBoardPosition))
        {
            return true;
        }
        else
        {
            newBoardPosition = default;
            return false;
        }
    }

    public bool IsInBoard(BoardPosition boardPosition)
    {
        int x = boardPosition.x;
        int y = boardPosition.y;

        return 0 <= x && x < BoardSize && 0 <= y && y < BoardSize;
    }

    private void ClearBlocks()
    {
        HashSet<BoardPosition> cellsToClear = new();

        void AddClearCellsAndCount(HashSet<BoardPosition> newSet)
        {
            if (newSet.Count > 0)
            {
                cellsToClear.UnionWith(newSet);
                // call score count
            }
        }

        for (int x = 0; x < BoardSize; x++)
        {
            AddClearCellsAndCount(GetCellsToClearWithX(x));
        }
        for (int y = 0; y < BoardSize; y++)
        {
            AddClearCellsAndCount(GetCellsToClearWithY(y));
        }
        for (int gridX = 0; gridX < GridSize; gridX++)
        {
            for (int gridY = 0; gridY < GridSize; gridY++)
            {
                AddClearCellsAndCount(GetCellsToClearWithGrid(gridX, gridY));
            }
        }

        foreach (BoardPosition pos in cellsToClear)
        {
            SetCell(pos, CellState.Empty);
        }
    }

    private HashSet<BoardPosition> GetCellsToClearWithX(int x)
    {
        if (x < 0 || BoardSize <= x)
        {
            return new HashSet<BoardPosition>();
        }

        HashSet<BoardPosition> cellsToBeCleared = new();

        for (int y = 0; y < BoardSize; y++)
        {
            if (GetCell(x, y) == CellState.Empty)
            {
                return new HashSet<BoardPosition>();
            }
        }

        for (int y = 0; y < BoardSize; y++)
        {
            cellsToBeCleared.Add(new BoardPosition(x, y));
        }
        return cellsToBeCleared;
    }

    private HashSet<BoardPosition> GetCellsToClearWithY(int y)
    {
        if (y < 0 || BoardSize <= y)
        {
            return new HashSet<BoardPosition>();
        }

        HashSet<BoardPosition> cellsToBeCleared = new();

        for (int x = 0; x < BoardSize; x++)
        {
            if (GetCell(x, y) == CellState.Empty)
            {
                return new HashSet<BoardPosition>();
            }
        }

        for (int x = 0; x < BoardSize; x++)
        {
            cellsToBeCleared.Add(new BoardPosition(x, y));
        }

        return cellsToBeCleared;
    }

    private HashSet<BoardPosition> GetCellsToClearWithGrid(int gridX, int gridY)
    {
        if (gridX < 0 || GridSize <= gridX || gridY < 0 || GridSize <= gridY)
        {
            return new HashSet<BoardPosition>();
        }
        HashSet<BoardPosition> cellsToBeCleared = new HashSet<BoardPosition>();

        for (int offsetX = 0; offsetX < GridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < GridSize; offsetY++)
            {
                int targetX = GridSize * gridX + offsetX;
                int targetY = GridSize * gridY + offsetY;
                if (GetCell(targetX, targetY) != CellState.Filled)
                {
                    return new HashSet<BoardPosition>();
                }
            }
        }

        for (int offsetX = 0; offsetX < GridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < GridSize; offsetY++)
            {
                int targetX = GridSize * gridX + offsetX;
                int targetY = GridSize * gridY + offsetY;
                cellsToBeCleared.Add(new BoardPosition(targetX, targetY));
            }
        }

        return cellsToBeCleared;
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
}
