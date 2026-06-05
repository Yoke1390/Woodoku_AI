using Script.Core.Primitive;
using UnityEngine;

namespace Script.Unity
{
    public static class UnityExtensions
    {
        public static Vector2 Center(this BlockShape shape)
        {
            return new Vector2(shape.MaxX / 2f, shape.MaxY / 2f);
        }

        public static Vector2 ToVector2(this BlockOffset offset)
        {
            return new Vector2(offset.x, offset.y);
        }

        public static BlockOffset ToBlockOffset(this Vector2Int vector)
        {
            return new BlockOffset(vector.x, vector.y);
        }
    }
}