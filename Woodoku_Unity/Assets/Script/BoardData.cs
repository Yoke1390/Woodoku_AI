using System;
using UnityEngine;

public class BoardData
{
    private int[,] board;

    public event EventHandler<CellUpdateData> CellUpdate;

    public BoardData()
    {
        Reset();
    }

    public void Reset()
    {
        board = new int[9, 9];
    }

    public int GetCell(int x, int y)
    {
        if (0 <= x && x <= 8 && 0 <= y && y <= 8)
        {
            return board[x, y];
        }
        else
        {
            // Invalid index
            return -1;
        }
    }

    public int GetCell(Vector2Int pos)
    {
        return GetCell(pos.x, pos.y);
    }

    public void SetCell(int x, int y, int value = 1)
    {
        if (0 <= x && x <= 8 && 0 <= y && y <= 8)
        {
            board[x, y] = value;
            CellUpdate?.Invoke(this, new CellUpdateData(x, y, value));
        }
        else
        {
            throw new IndexOutOfRangeException($"Invalid board index: {x}, {y}");
        }
    }

    public void SetCell(Vector2Int pos, int value = 1)
    {
        SetCell(pos.x, pos.y, value);
    }

    public bool CanPlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        foreach (BoardPosition cell in blockData.BlockCells)
        {
            Vector2Int targetPos = cell + blockBaseBoardPosition;
            int cellValue = GetCell(targetPos);
            Debug.Log($"Target Pos: {targetPos.x}, {targetPos.y} value: {cellValue}");
            if (cellValue != 0)
            {
                return false;
            }
        }
        return true;
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
