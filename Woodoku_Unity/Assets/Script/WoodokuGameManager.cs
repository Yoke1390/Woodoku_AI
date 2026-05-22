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
        handManager.Initialize(HandleDropRequest, boardUI.CellSize);

        boardData.CellUpdate += boardUI.BoradData_OnCellUpdate;

        // test
        boardData.SetCell(2, 0);
        boardData.SetCell(2, 1);
        boardData.SetCell(2, 2);
        boardData.SetCell(2, 3);
        boardData.SetCell(2, 4);
        boardData.SetCell(2, 6);
        boardData.SetCell(2, 7);
        boardData.SetCell(2, 8);
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
            return boardData.TryPlaceBlock(blockData, blockBaseBoardPosition);
        }
        return false;
    }
}
