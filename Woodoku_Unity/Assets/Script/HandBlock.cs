using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBlock : MonoBehaviour
{
    [SerializeField]
    private BlockPiece blockPiecePrefab;
    private List<BlockPiece> blockPieces;

    [SerializeField]
    private BlockData sampleBlockData;

    private void Start()
    {
        Initialize(sampleBlockData);
    }

    public void Initialize(BlockData blockData)
    {
        float cellSize = BoardUI.Instance.CellSize;
        foreach (Vector2Int blockPosition in blockData.BlockCells)
        {
            BlockPiece newBlockPiece = Instantiate(blockPiecePrefab, gameObject.transform);
            RectTransform newBlockPieceRectTransform = newBlockPiece.GetComponent<RectTransform>();

            newBlockPieceRectTransform.anchoredPosition = (Vector2)blockPosition * cellSize;
            newBlockPieceRectTransform.sizeDelta = Vector2.one * cellSize;
        }
    }
}
