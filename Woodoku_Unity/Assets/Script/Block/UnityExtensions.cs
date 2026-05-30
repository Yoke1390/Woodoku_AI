using UnityEngine;

public static class UnityExtensions
{
    public static Vector2 Center(this BlockShape shape) => new(shape.MaxX / 2f, shape.MaxY / 2f);

    public static Vector2 ToVector2(this BlockOffset offset) => new(offset.x, offset.y);

    public static BlockOffset ToBlockOffset(this Vector2Int vector)
    {
        return new BlockOffset(vector.x, vector.y);
    }
}
