using UnityEngine;

public class HandBlock : MonoBehaviour
{
    [SerializeField]
    private BlockPiece blockPiecePrefab;

    [SerializeField]
    private Transform blockPiecesParent;

    [SerializeField]
    private float inSlotCellSizeScale = 0.5f;
    private BlockPiece[] blockPieces;

    public BlockShape BlockShape { get; private set; }

    public void Initialize(BlockShape blockShape, float cellSize)
    {
        BlockShape = blockShape;
        blockPieces = new BlockPiece[blockShape.NBlocks];

        if (cellSize <= 0)
        {
            Debug.Log("Cell size <= 0");
            cellSize = 1f;
        }

        for (int i = 0; i < blockShape.NBlocks; i++)
        {
            BlockOffset blockOffset = blockShape.Blocks[i];
            BlockPiece newBlockPiece = Instantiate(blockPiecePrefab, blockPiecesParent);
            RectTransform newBlockPieceRectTransform = newBlockPiece.GetComponent<RectTransform>();

            newBlockPieceRectTransform.anchoredPosition =
                ((Vector2)blockOffset - blockShape.Center()) * cellSize;
            newBlockPieceRectTransform.sizeDelta = Vector2.one * cellSize;

            blockPieces[i] = newBlockPiece;
        }

        blockPiecesParent.localScale = inSlotCellSizeScale * Vector3.one;
    }

    public void SetScale(float scale)
    {
        blockPiecesParent.localScale = scale * Vector3.one;
    }

    public void ResetScale()
    {
        blockPiecesParent.localScale = inSlotCellSizeScale * Vector3.one;
    }
}
