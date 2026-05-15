using UnityEngine;

public struct BoardPosition
{
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

    public static BoardPosition operator +(BoardPosition a, BoardPosition b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static BoardPosition operator -(BoardPosition a, BoardPosition b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public Vector2Int Value
    {
        get => _value;
        set { _value = new Vector2Int(value.x, value.y); }
    }

    public BoardPosition(int x, int y)
        : this()
    {
        Value = new Vector2Int(x, y);
    }
}
