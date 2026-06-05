using Script.Core.Primitive;
using UnityEngine;

namespace Script.Unity.Hand
{
    public class HandBlock : MonoBehaviour
    {
        private const int BackgroundSize = 3;
        [SerializeField] private BlockPiece blockPiecePrefab;

        [SerializeField] private Transform blockPiecesParent;

        [SerializeField] private RectTransform clickTargetBackground;

        private readonly float _inSlotCellSizeScale = 0.5f;

        private BlockPiece[] _blockPieces;
        private float _cellSize;

        public BlockShape BlockShape { get; private set; }

        public void Initialize(BlockShape blockShape, float cellSize)
        {
            BlockShape = blockShape;
            _blockPieces = new BlockPiece[blockShape.NBlocks];

            if (cellSize <= 0)
            {
                Debug.Log("Cell size <= 0");
                cellSize = 1f;
            }

            _cellSize = cellSize;

            for (int i = 0; i < blockShape.NBlocks; i++)
            {
                BlockOffset blockOffset = blockShape.Blocks[i];
                BlockPiece newBlockPiece = Instantiate(blockPiecePrefab, blockPiecesParent);
                RectTransform newBlockPieceRectTransform = newBlockPiece.GetComponent<RectTransform>();

                newBlockPieceRectTransform.anchoredPosition =
                    (blockOffset.ToVector2() - blockShape.Center()) * _cellSize;
                newBlockPieceRectTransform.sizeDelta = Vector2.one * _cellSize;

                _blockPieces[i] = newBlockPiece;
            }

            ResetScale();
        }

        public void ResetScale()
        {
            SetScale(_inSlotCellSizeScale);
        }

        public void SetScale(float scale)
        {
            clickTargetBackground.sizeDelta = scale * _cellSize * BackgroundSize * Vector2.one;
            blockPiecesParent.localScale = scale * Vector3.one;
        }
    }
}
