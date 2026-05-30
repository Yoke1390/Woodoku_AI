using System;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] handSlots;

    [SerializeField]
    private HandBlock handBlockPrefab;

    private IReadOnlyHands _hands;
    private DropHandler _dropHandler;
    private float _cellSize;

    public void Initialize(DropHandler dropHandler, float cellSize, IReadOnlyHands hands)
    {
        _dropHandler = dropHandler;
        _cellSize = cellSize;
        _hands = hands;

        _hands.HandBlockGenerated += GenerateHandBlock;
    }

    private void GenerateHandBlock(int slotIndex, BlockShape blockShape)
    {
        HandBlock newHandBlock = Instantiate(handBlockPrefab, handSlots[slotIndex]);
        newHandBlock.Initialize(blockShape, _cellSize);

        var draggableBlock = newHandBlock.GetComponent<DraggableBlock>();
        draggableBlock.Initialize(slotIndex, _dropHandler);
    }
}
