using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPreview : MonoBehaviour
{
    [SerializeField]
    private HandBlock handBlockPrefab;

    [SerializeField]
    private int cellSize = 10;

    [SerializeField]
    private int slotSize = 300;
    private BlockData[] blockDatas;

    private void Start()
    {
        InvokeRepeating(nameof(ShowAllBlocks), 0f, 2f);
    }

    private void ShowAllBlocks()
    {
        ClearAllBlocks();
        blockDatas = Resources.LoadAll<BlockData>("");
        foreach (BlockData blockData in blockDatas)
        {
            ShowHandBlock(blockData);
        }
    }

    private void ClearAllBlocks()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowHandBlock(BlockData blockData)
    {
        GameObject slot = new("Slot", typeof(RectTransform));
        RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
        slotRectTransform.SetParent(transform, false);
        slotRectTransform.sizeDelta = new Vector2(slotSize, slotSize);

        HandBlock newHandBlock = Instantiate(handBlockPrefab, slotRectTransform);
        newHandBlock.Initialize(blockData, cellSize);
    }
}
