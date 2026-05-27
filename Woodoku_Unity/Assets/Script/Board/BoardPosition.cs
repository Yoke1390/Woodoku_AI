using System;
using UnityEngine;

public readonly struct BoardPosition
{
    private readonly Vector2Int _value;
    public int x => _value.x;
    public int y => _value.y;

    // without validation
    public static BoardPosition operator +(BoardPosition boardPosition, BlockOffset blockOffset)
    {
        int x = boardPosition.x + blockOffset.x;
        int y = boardPosition.y + blockOffset.y;
        return new BoardPosition(x, y);
    }

    public override bool Equals(object other)
    {
        if ((other is BoardPosition otherPos))
        {
            return this._value == otherPos._value;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public BoardPosition(int x, int y)
        : this()
    {
        _value = new Vector2Int(x, y);
    }
}
