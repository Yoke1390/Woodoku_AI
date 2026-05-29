using System;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] handSlots;

    [SerializeField]
    private HandBlock handBlockPrefab;

    private Unity.Mathematics.Random random;
    private BlockData[] blockDatas;
    private BlockData[] _currentHandBlockDatas;
    public IReadOnlyList<BlockData> CurrentHandBlockDatas => _currentHandBlockDatas;
    private DropHandler _dropHandler;
    private float _cellSize;

    public event Action BlockPlaced;
    public event Action HandBlockGenerated;

    void Awake()
    {
        blockDatas = Resources.LoadAll<BlockData>("");

        Debug.Log($"{blockDatas.Length} Block Data was found");

        _currentHandBlockDatas = new BlockData[handSlots.Length];
    }

    public void Initialize(DropHandler dropHandler, float cellSize, uint randomSeed)
    {
        random = new(randomSeed);
        _dropHandler = dropHandler;
        _cellSize = cellSize;
        GenerateAll();
    }

    private BlockData GetSampleBlockData()
    {
        // tmp
        return blockDatas[13];
    }

    public BlockData GetRandomBlockData()
    {
        return blockDatas[random.NextInt(0, blockDatas.Length)];
    }

    private void GenerateAll()
    {
        for (int i = 0; i < handSlots.Length; i++)
        {
            BlockData blockData = GetRandomBlockData();
            GenerateHandBlock(i, blockData);
        }
        HandBlockGenerated?.Invoke();
    }

    private void GenerateHandBlock(int slotIndex, BlockData blockData)
    {
        _currentHandBlockDatas[slotIndex] = blockData;

        HandBlock newHandBlock = Instantiate(handBlockPrefab, handSlots[slotIndex]);
        newHandBlock.Initialize(blockData, _cellSize);

        var draggableBlock = newHandBlock.GetComponent<DraggableBlock>();
        draggableBlock.SetDropHandler(_dropHandler);
        draggableBlock.BlockPlaced += () => OnBlockPlaced(slotIndex);
    }

    private void OnBlockPlaced(int slotIndex)
    {
        _currentHandBlockDatas[slotIndex] = null;
        if (IsHandEmpty())
        {
            GenerateAll();
        }

        BlockPlaced?.Invoke();
    }

    private bool IsHandEmpty()
    {
        return System.Array.TrueForAll(_currentHandBlockDatas, block => block == null);
    }
}
