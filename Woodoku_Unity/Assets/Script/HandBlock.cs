using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBlock : MonoBehaviour
{
    [SerializeField]
    private BlockPiece blockPiecePrefab;

    [SerializeField]
    private float inSlotCellSizeScale = 0.5f;
    private BlockPiece[] blockPieces;

    public void Initialize(BlockData blockData)
    {
        blockPieces = new BlockPiece[blockData.N_Blocks];

        float cellSize = BoardUI.Instance.CellSize * inSlotCellSizeScale;
        if (cellSize <= 0)
        {
            Debug.Log("Cell size <= 0");
            cellSize = 1f;
        }

        Vector2 centerOffset = blockData.Center;

        for (int i = 0; i < blockData.N_Blocks; i++)
        {
            Vector2 blockPosition = blockData.BlockCells[i];
            BlockPiece newBlockPiece = Instantiate(blockPiecePrefab, gameObject.transform);
            RectTransform newBlockPieceRectTransform = newBlockPiece.GetComponent<RectTransform>();

            newBlockPieceRectTransform.anchoredPosition = (blockPosition - centerOffset) * cellSize;
            newBlockPieceRectTransform.sizeDelta = Vector2.one * cellSize;

            blockPieces[i] = newBlockPiece;
        }
    }
}
