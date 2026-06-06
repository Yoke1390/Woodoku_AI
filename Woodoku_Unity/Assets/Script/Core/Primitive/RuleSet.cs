using System.Collections.Generic;

namespace Script.Core.Primitive
{
    public readonly struct RuleSet
    {
        public int GridSize { get; }
        public int NHandSlots { get; }
        public IReadOnlyList<BlockShape> ShapePool { get; }

        public RuleSet(int gridSize, int nHandSlots, IReadOnlyList<BlockShape> shapePool)
        {
            GridSize = gridSize;
            NHandSlots = nHandSlots;
            ShapePool = shapePool;
        }
    }
}
