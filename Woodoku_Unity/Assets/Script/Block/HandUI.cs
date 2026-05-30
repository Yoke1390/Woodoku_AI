using System;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] handSlots;

    [SerializeField]
    private HandBlock handBlockPrefab;

    private HandManager _handmanager;
    private DropHandler _dropHandler;
    private float _cellSize;

    public void Initialize(DropHandler dropHandler, float cellSize, HandManager handManager)
    {
        _dropHandler = dropHandler;
        _cellSize = cellSize;
        _handmanager = handManager;

        _handmanager.HandBlockGenerated += GenerateHandBlock;
    }

    private void GenerateHandBlock(int slotIndex, BlockShape blockShape)
    {
        HandBlock newHandBlock = Instantiate(handBlockPrefab, handSlots[slotIndex]);
        newHandBlock.Initialize(blockShape, _cellSize);

        var draggableBlock = newHandBlock.GetComponent<DraggableBlock>();
        draggableBlock.SetDropHandler(_dropHandler);
        draggableBlock.BlockPlaced += () => _handmanager.OnBlockPlaced(slotIndex);
    }
}
