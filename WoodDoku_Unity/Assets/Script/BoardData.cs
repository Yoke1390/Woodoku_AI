using System;

public class BoardData
{
    private int[,] board;

    public BoardData()
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

    public void SetCell(int x, int y, int value = 1)
    {
        if (0 <= x && x <= 8 && 0 <= y && y <= 8)
        {
            board[x, y] = value;
        }
        else
        {
            throw new IndexOutOfRangeException($"Invalid board index: {x}, {y}");
        }
    }
}
