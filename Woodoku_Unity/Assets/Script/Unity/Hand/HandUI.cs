using System;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] handSlots;

    [SerializeField]
    private HandBlock handBlockPrefab;

    private HandBlock[] _handBlocks;

    private IReadOnlyHands _hands;
    private DropHandler _dropHandler;
    private float _cellSize;

    public void Initialize(DropHandler dropHandler, float cellSize, IReadOnlyHands hands)
    {
        _dropHandler = dropHandler;
        _cellSize = cellSize;
        _hands = hands;

        _handBlocks = new HandBlock[hands.NSlots];

        _hands.HandBlockConsumed += OnHandBlockConsumed;
        _hands.HandBlockGenerated += GenerateHandBlock;
    }

    private void OnHandBlockConsumed(int slotIndex)
    {
        Destroy(_handBlocks[slotIndex].gameObject);
    }

    private void GenerateHandBlock(int slotIndex, BlockShape blockShape)
    {
        HandBlock newHandBlock = Instantiate(handBlockPrefab, handSlots[slotIndex]);
        newHandBlock.Initialize(blockShape, _cellSize);
        _handBlocks[slotIndex] = newHandBlock;

        var draggableBlock = newHandBlock.GetComponent<DraggableBlock>();
        draggableBlock.Initialize(slotIndex, _dropHandler);
    }
}
