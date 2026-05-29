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
    private HandManager handManager;

    [SerializeField]
    private GameSetting gameSetting;

    private BoardData boardData;

    void Start()
    {
        boardData = new BoardData(gameSetting.GridSize);
        boardUI.Initialize(boardData);

        uint randomSeed = 1234; // test
        handManager.Initialize(HandleDropRequest, boardUI.CellSize, randomSeed);
        handManager.BlockPlaced += CheckForGameOver;
        handManager.HandBlockGenerated += CheckForGameOver;

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
        foreach (BlockData blockData in handManager.CurrentHandBlockDatas)
        {
            if (blockData == null)
            {
                continue;
            }
            if (boardData.CanPlaceBlockInBoard(blockData))
            {
                return false;
            }
        }
        return true;
    }

    private bool HandleDropRequest(PointerEventData eventData, BlockData blockData)
    {
        if (
            boardUI.TryScreenPointToBoardPosition(
                eventData.position,
                eventData.pressEventCamera,
                blockData.Center,
                out BoardPosition blockBaseBoardPosition
            )
        )
        {
            PlacementResult result = boardData.TryPlaceBlock(blockData, blockBaseBoardPosition);
            // scoreManager.update(result);
            return result.IsSuccess;
        }
        return false;
    }
}
