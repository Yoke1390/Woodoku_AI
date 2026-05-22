using System;
using UnityEngine;

public class BoardData
{
    public int GridSize { get; }
    public int BoardSize { get; }
    private int[,] board;

    public event EventHandler<CellUpdateData> CellUpdate;

    public BoardData(int gridSize)
    {
        GridSize = gridSize;
        BoardSize = GridSize * GridSize;
        Reset();
    }

    public void Reset()
    {
        board = new int[BoardSize, BoardSize];
    }

    public int GetCell(BoardPosition boardPosition)
    {
        if (IsInBoard(boardPosition))
        {
            return board[boardPosition.x, boardPosition.y];
        }
        else
        {
            // Invalid index
            return -1;
        }
    }

    public int GetCell(int x, int y)
    {
        return GetCell(new BoardPosition(x, y));
    }

    public void SetCell(BoardPosition boardPosition, int value = 1)
    {
        int x = boardPosition.x;
        int y = boardPosition.y;
        if (IsInBoard(boardPosition))
        {
            board[x, y] = value;
            CellUpdate?.Invoke(this, new CellUpdateData(x, y, value));
        }
        else
        {
            throw new IndexOutOfRangeException($"Invalid board index: {x}, {y}");
        }
    }

    public void SetCell(int x, int y, int value = 1)
    {
        var boardPosition = new BoardPosition(x, y);
        SetCell(boardPosition, value);
    }

    public bool CanPlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        foreach (BlockOffset cellOffset in blockData.BlockCells)
        {
            if (TryOffset(blockBaseBoardPosition, cellOffset, out BoardPosition targetPos))
            {
                int cellValue = GetCell(targetPos);
                if (cellValue != 0)
                {
                    // target pos is filled
                    return false;
                }
            }
            else
            {
                // base + offset out of board
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
            PlaceBlock(blockData, blockBaseBoardPosition);
        }
        return canPlace;
    }

    private void PlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        foreach (BlockOffset blockOffset in blockData.BlockCells)
        {
            // called after validation
            BoardPosition pos = blockBaseBoardPosition + blockOffset;
            SetCell(pos, 1);
        }
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

    public readonly struct CellUpdateData
    {
        public int X { get; }
        public int Y { get; }
        public int Value { get; }

        public CellUpdateData(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
}
