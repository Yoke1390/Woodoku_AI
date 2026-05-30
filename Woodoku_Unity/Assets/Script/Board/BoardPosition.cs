using System;

public readonly struct BoardPosition
{
    public int x { get; }
    public int y { get; }

    // without validation
    public static BoardPosition operator +(BoardPosition boardPosition, BlockOffset blockOffset)
    {
        int x = boardPosition.x + blockOffset.x;
        int y = boardPosition.y + blockOffset.y;
        return new BoardPosition(x, y);
    }

    public override bool Equals(object other)
    {
        if (other is BoardPosition otherPos)
        {
            return this.x == otherPos.x && this.y == otherPos.y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public BoardPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
