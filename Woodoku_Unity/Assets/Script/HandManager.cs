using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] handSlots;

    [SerializeField]
    private HandBlock handBlockPrefab;

    private BlockData[] blockDatas;
    private DropHandler _dropHandler;
    private float _cellSize;

    void Awake()
    {
        blockDatas = Resources.LoadAll<BlockData>("");

        Debug.Log($"{blockDatas.Length} Block Data was found");
    }

    public void Initialize(DropHandler dropHandler, float cellSize)
    {
        _dropHandler = dropHandler;
        _cellSize = cellSize;
        for (int i = 0; i < handSlots.Length; i++)
        {
            BlockData blockData = GetBlockData();
            GenerateHandBlock(i, blockData);
        }
    }

    private BlockData GetBlockData()
    {
        // tmp
        return blockDatas[3];
    }

    private void GenerateHandBlock(int slotIndex, BlockData blockData)
    {
        HandBlock newHandBlock = Instantiate(handBlockPrefab, handSlots[slotIndex]);
        newHandBlock.Initialize(blockData, _cellSize);

        var draggableBlock = newHandBlock.GetComponent<DraggableBlock>();
        draggableBlock.SetDropHandler(_dropHandler);
    }
}
