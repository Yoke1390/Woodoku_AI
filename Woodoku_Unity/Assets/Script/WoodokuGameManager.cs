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
        boardData.SetCell(0, 0, 1);
        boardData.SetCell(2, 7, 1);
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
