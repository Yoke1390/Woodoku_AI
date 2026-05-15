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
        if (IsValid(boardPosition))
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
        if (IsValid(boardPosition))
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
        foreach (BoardPosition cell in blockData.BlockCells)
        {
            BoardPosition targetPos = cell + blockBaseBoardPosition;
            int cellValue = GetCell(targetPos);
            Debug.Log($"Target Pos: {targetPos.x}, {targetPos.y} value: {cellValue}");
            if (cellValue != 0)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsValid(BoardPosition boardPosition)
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
