using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private HandBlock handBlockPrefab;
    [SerializeField] private RectTransform[] handSlots;
    private float _cellSize;
    private BlockControlMode _controlMode;
    private EndBlockMoveHandler _endBlockMoveHandler;

    private HandBlock[] _handBlocks;

    private IReadOnlyHands _hands;

    public void Initialize(
        EndBlockMoveHandler endBlockMoveHandler,
        float cellSize,
        IReadOnlyHands hands,
        BlockControlMode controlMode = BlockControlMode.Drag
    )
    {
        _endBlockMoveHandler = endBlockMoveHandler;
        _cellSize = cellSize;
        _hands = hands;
        _controlMode = controlMode;

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
        var newHandBlock = Instantiate(handBlockPrefab, handSlots[slotIndex]);
        newHandBlock.Initialize(blockShape, _cellSize);
        _handBlocks[slotIndex] = newHandBlock;

        newHandBlock.GetComponent<BlockManipulator>().Initialize(slotIndex, _endBlockMoveHandler);
        newHandBlock.gameObject.AddComponent(GameSetting.GetBlockControlInputType(_controlMode));
    }
}
