using System;
using System.Collections.Generic;
using System.Linq;

public class BoardData : IReadOnlyBoard
{
    public int GridSize { get; }
    public int BoardSize { get; }
    public int NGrids { get; }

    private CellState[,] board;

    public event EventHandler<CellUpdateData> CellUpdate;

    public BoardData(int gridSize)
    {
        GridSize = gridSize;
        BoardSize = GridSize * GridSize;
        NGrids = BoardSize / GridSize;
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

    public bool CanPlaceBlockInBoard(BlockShape blockShape)
    {
        for (int x = 0; x < BoardSize - blockShape.MaxX; x++)
        {
            for (int y = 0; y < BoardSize - blockShape.MaxY; y++)
            {
                BoardPosition blockBaseBoardPosition = new(x, y);
                if (CanPlaceBlock(blockShape, blockBaseBoardPosition))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanPlaceBlock(BlockShape blockShape, BoardPosition blockBaseBoardPosition)
    {
        foreach (BlockOffset cellOffset in blockShape.Blocks)
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

    public PlacementResult TryPlaceBlock(
        BlockShape blockShape,
        BoardPosition blockBaseBoardPosition
    )
    {
        bool canPlace = CanPlaceBlock(blockShape, blockBaseBoardPosition);
        if (canPlace)
        {
            PlacementResult result = PlaceBlockAndClear(blockShape, blockBaseBoardPosition);
            return result;
        }
        return PlacementResult.Failure(blockShape);
    }

    private PlacementResult PlaceBlockAndClear(
        BlockShape blockShape,
        BoardPosition blockBaseBoardPosition
    )
    {
        foreach (BlockOffset blockOffset in blockShape.Blocks)
        {
            // called after validation
            BoardPosition pos = blockBaseBoardPosition + blockOffset;
            SetCell(pos, CellState.Filled);
        }

        (int nClearedTimes, List<BoardPosition> cellsToClearList) = GetCellsToClear();

        ClearCells(cellsToClearList);

        return new PlacementResult(true, blockShape, nClearedTimes, cellsToClearList);
    }

    private void ClearCells(IEnumerable<BoardPosition> cellsToClear)
    {
        foreach (BoardPosition pos in cellsToClear)
        {
            SetCell(pos, CellState.Empty);
        }
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

        for (int x = 0; x < BoardSize; x++)
        {
            AddClearCellsAndCount(GetCellsToClearWithX(x));
        }
        for (int y = 0; y < BoardSize; y++)
        {
            AddClearCellsAndCount(GetCellsToClearWithY(y));
        }
        for (int gridX = 0; gridX < NGrids; gridX++)
        {
            for (int gridY = 0; gridY < NGrids; gridY++)
            {
                AddClearCellsAndCount(GetCellsToClearWithGrid(gridX, gridY));
            }
        }

        List<BoardPosition> cellsToClearList = cellsToClearSet.ToList();

        return (nClearedTimes, cellsToClearList);
    }

    private IReadOnlyCollection<BoardPosition> GetCellsToClearWithX(int x)
    {
        if (x < 0 || BoardSize <= x)
        {
            return Array.Empty<BoardPosition>();
        }

        for (int y = 0; y < BoardSize; y++)
        {
            if (GetCell(x, y) != CellState.Filled)
            {
                return Array.Empty<BoardPosition>();
            }
        }

        HashSet<BoardPosition> cellsToBeCleared = new();
        for (int y = 0; y < BoardSize; y++)
        {
            cellsToBeCleared.Add(new BoardPosition(x, y));
        }
        return cellsToBeCleared;
    }

    private IReadOnlyCollection<BoardPosition> GetCellsToClearWithY(int y)
    {
        if (y < 0 || BoardSize <= y)
        {
            return Array.Empty<BoardPosition>();
        }

        for (int x = 0; x < BoardSize; x++)
        {
            if (GetCell(x, y) != CellState.Filled)
            {
                return Array.Empty<BoardPosition>();
            }
        }

        HashSet<BoardPosition> cellsToBeCleared = new();
        for (int x = 0; x < BoardSize; x++)
        {
            cellsToBeCleared.Add(new BoardPosition(x, y));
        }

        return cellsToBeCleared;
    }

    private IReadOnlyCollection<BoardPosition> GetCellsToClearWithGrid(int gridX, int gridY)
    {
        if (gridX < 0 || NGrids <= gridX || gridY < 0 || NGrids <= gridY)
        {
            return Array.Empty<BoardPosition>();
        }

        for (int offsetX = 0; offsetX < GridSize; offsetX++)
        {
            for (int offsetY = 0; offsetY < GridSize; offsetY++)
            {
                int targetX = GridSize * gridX + offsetX;
                int targetY = GridSize * gridY + offsetY;
                if (GetCell(targetX, targetY) != CellState.Filled)
                {
                    return Array.Empty<BoardPosition>();
                }
            }
        }

        HashSet<BoardPosition> cellsToBeCleared = new();
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

    private bool IsInBoard(BoardPosition boardPosition)
    {
        int x = boardPosition.x;
        int y = boardPosition.y;
        return 0 <= x && x < BoardSize && 0 <= y && y < BoardSize;
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
}

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
