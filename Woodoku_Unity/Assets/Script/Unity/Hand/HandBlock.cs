using UnityEngine;

public class HandBlock : MonoBehaviour
{
    private const int BackgroundSize = 3;

    private BlockPiece[] _blockPieces;
    private float _cellSize;
    [SerializeField] private BlockPiece blockPiecePrefab;

    [SerializeField] private Transform blockPiecesParent;

    [SerializeField] private RectTransform clickTargetBackground;

    [SerializeField] private readonly float inSlotCellSizeScale = 0.5f;

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

        for (var i = 0; i < blockShape.NBlocks; i++)
        {
            var blockOffset = blockShape.Blocks[i];
            var newBlockPiece = Instantiate(blockPiecePrefab, blockPiecesParent);
            var newBlockPieceRectTransform = newBlockPiece.GetComponent<RectTransform>();

            newBlockPieceRectTransform.anchoredPosition =
                (blockOffset.ToVector2() - blockShape.Center()) * _cellSize;
            newBlockPieceRectTransform.sizeDelta = Vector2.one * _cellSize;

            _blockPieces[i] = newBlockPiece;
        }

        ResetScale();
    }

    public void ResetScale()
    {
        SetScale(inSlotCellSizeScale);
    }

    public void SetScale(float scale)
    {
        clickTargetBackground.sizeDelta = scale * _cellSize * BackgroundSize * Vector2.one;
        blockPiecesParent.localScale = scale * Vector3.one;
    }
}