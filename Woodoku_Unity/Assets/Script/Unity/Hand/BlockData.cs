using System;
using System.Linq;
using Script.Core.Primitive;
using UnityEngine;

namespace Script.Unity.Hand
{
    [CreateAssetMenu(menuName = "ScriptableObject/BlockData")]
    public class BlockData : ScriptableObject
    {
        [SerializeField] private Vector2Int[] blockCells;

        public int NBlocks => blockCells.Length;
        public BlockOffset[] BlockCells { get; private set; }

        private void OnEnable()
        {
            ResetCache();
        }

        // インスペクターの値が変更されたら呼ばれる
        private void OnValidate()
        {
            // x=0,y=0が起点になるようにオフセット
            OffsetToZero();

            ResetCache();
        }

        public BlockShape ToShape()
        {
            return new BlockShape(BlockCells);
        }

        private void OffsetToZero()
        {
            if (blockCells == null || blockCells.Length == 0)
                return;

            var minX = blockCells.Min(cell => cell.x);
            if (minX != 0)
                Offset(-minX, 0);

            var minY = blockCells.Min(cell => cell.y);
            if (minY != 0)
                Offset(0, -minY);
        }

        private void Offset(int offsetX, int offsetY)
        {
            for (var i = 0; i < NBlocks; i++)
            {
                var vec = blockCells[i];
                var newX = vec.x + offsetX;
                var newY = vec.y + offsetY;
                blockCells[i] = new Vector2Int(newX, newY);
            }
        }

        private void ResetCache()
        {
            if (blockCells == null || blockCells.Length == 0)
            {
                BlockCells = Array.Empty<BlockOffset>();
                return;
            }

            BlockCells = new BlockOffset[NBlocks];
            for (var i = 0; i < NBlocks; i++) BlockCells[i] = blockCells[i].ToBlockOffset();
        }
    }
}
