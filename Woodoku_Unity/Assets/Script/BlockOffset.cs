using UnityEngine;

public readonly struct BlockOffset
{
    private readonly Vector2Int _value;
    public int x => _value.x;
    public int y => _value.y;

    public static explicit operator BlockOffset(Vector2Int vector)
    {
        return new BlockOffset(vector.x, vector.y);
    }

    public static explicit operator Vector2(BlockOffset blockOffset)
    {
        return new Vector2(blockOffset.x, blockOffset.y);
    }

    public BlockOffset(int x, int y)
        : this()
    {
        _value = new Vector2Int(x, y);
    }
}
