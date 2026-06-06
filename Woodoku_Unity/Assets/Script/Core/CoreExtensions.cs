using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core
{
    public static class CoreExtensions
    {
        public static bool Contains(this IReadOnlyBoard board, BoardPosition pos)
        {
            return 0 <= pos.x && pos.x < board.BoardSize
                              && 0 <= pos.y && pos.y < board.BoardSize;
        }
    }
}
