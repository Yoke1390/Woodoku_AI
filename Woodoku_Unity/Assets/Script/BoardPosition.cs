using UnityEngine;

public struct BoardPosition
{
    private const int BOARD_SIZE = 9;

    private Vector2Int _value;
    public int x => _value.x;
    public int y => _value.y;

    public static implicit operator Vector2Int(BoardPosition position)
    {
        return position.Value;
    }

    public static implicit operator Vector2(BoardPosition position)
    {
        return new Vector2(position.x, position.y);
    }

    public static implicit operator BoardPosition(Vector2Int vector)
    {
        return new BoardPosition(vector.x, vector.y);
    }

    public static Vector2Int operator +(BoardPosition a, BoardPosition b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static Vector2Int operator -(BoardPosition a, BoardPosition b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < BOARD_SIZE && pos.y >= 0 && pos.y < BOARD_SIZE;
    }

    public Vector2Int Value
    {
        get => _value;
        set
        {
            int clampedX = Mathf.Clamp(value.x, 0, BOARD_SIZE - 1);
            int clampedY = Mathf.Clamp(value.y, 0, BOARD_SIZE - 1);
            _value = new Vector2Int(clampedX, clampedY);
        }
    }

    public BoardPosition(int x, int y)
        : this()
    {
        Value = new Vector2Int(x, y);
    }
}
