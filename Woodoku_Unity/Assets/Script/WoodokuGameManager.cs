using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WoodokuGameManager : MonoBehaviour
{
    [SerializeField]
    private BoardUI boardUI;

    [SerializeField]
    private HandUI handUI;

    [SerializeField]
    private GameSetting gameSetting;

    private BoardData boardData;
    private HandManager handManager;

    public const int NHandSlots = 3;

    void Start()
    {
        Initialize();
        handManager.Begin();
    }

    private void Initialize()
    {
        boardData = new BoardData(gameSetting.GridSize);
        boardUI.Initialize(boardData);

        int randomSeed = 1234; // test

        BlockData[] blockDatas = Resources.LoadAll<BlockData>("");
        List<BlockShape> blockShapes = new();
        foreach (BlockData data in blockDatas)
        {
            BlockShape shape = data.ToShape();
            blockShapes.Add(shape);
        }

        handManager = new(blockShapes, NHandSlots, randomSeed);
        handManager.HandSettled += CheckForGameOver;

        handUI.Initialize(HandleDropRequest, boardUI.CellSize, handManager);

        boardData.CellUpdate += boardUI.BoardData_OnCellUpdate;
    }

    private void CheckForGameOver()
    {
        if (IsGameOver())
        {
            Debug.Log("Game Over");
        }
    }

    private bool IsGameOver()
    {
        foreach (BlockShape? blockData in handManager.CurrentHand)
        {
            if (!blockData.HasValue)
            {
                continue;
            }
            if (boardData.CanPlaceBlockInBoard(blockData.Value))
            {
                return false;
            }
        }
        return true;
    }

    private bool HandleDropRequest(PointerEventData eventData, BlockShape blockShape)
    {
        if (
            boardUI.TryScreenPointToBoardPosition(
                eventData.position,
                eventData.pressEventCamera,
                blockShape.Center(),
                out BoardPosition blockBaseBoardPosition
            )
        )
        {
            PlacementResult result = boardData.TryPlaceBlock(blockShape, blockBaseBoardPosition);
            // scoreManager.update(result);
            return result.IsSuccess;
        }
        return false;
    }
}
