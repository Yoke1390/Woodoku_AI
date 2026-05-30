using UnityEngine;

public static class RenderExtensions
{
    public static Vector2 Center(this BlockShape shape) => new(shape.MaxX / 2f, shape.MaxY / 2f);
}
