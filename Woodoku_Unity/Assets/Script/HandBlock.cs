using System.Collections;
using System.Collections.Generic;
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

    public BlockData BlockData { get; private set; }

    public void Initialize(BlockData blockData, float cellSize)
    {
        BlockData = blockData;
        blockPieces = new BlockPiece[blockData.N_Blocks];

        if (cellSize <= 0)
        {
            Debug.Log("Cell size <= 0");
            cellSize = 1f;
        }

        for (int i = 0; i < blockData.N_Blocks; i++)
        {
            BoardPosition blockPosition = blockData.BlockCells[i];
            BlockPiece newBlockPiece = Instantiate(blockPiecePrefab, blockPiecesParent);
            RectTransform newBlockPieceRectTransform = newBlockPiece.GetComponent<RectTransform>();

            newBlockPieceRectTransform.anchoredPosition =
                (blockPosition - BlockData.Center) * cellSize;
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
