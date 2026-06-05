using System;
using System.Collections.Generic;
using System.Linq;

namespace Script.Core.Primitive
{
    public readonly struct BlockShape
    {
        private readonly IReadOnlyList<BlockOffset> _blocks;
        public IReadOnlyList<BlockOffset> Blocks => _blocks ?? Array.Empty<BlockOffset>();
        public int NBlocks => _blocks.Count;

        public int MaxX { get; }
        public int MaxY { get; }

        public BlockShape(IEnumerable<BlockOffset> blocks)
        {
            _blocks = blocks.ToList();

            if (_blocks.Count == 0) throw new ArgumentException("Number of blocks must be positive.", nameof(_blocks));

            int maxX = _blocks.Max(b => b.x);
            int maxY = _blocks.Max(b => b.y);

            MaxX = maxX;
            MaxY = maxY;
        }
    }
}
