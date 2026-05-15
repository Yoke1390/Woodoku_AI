using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodokuGameManager : MonoBehaviour
{
    public static WoodokuGameManager Instance = null;

    [SerializeField]
    private BoardUI boardUI;

    [SerializeField]
    private HandManager handManager;

    private BoardData boardData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    void Start()
    {
        boardData = new BoardData();

        boardUI.Initialize();
        handManager.Initialize();

        boardData.CellUpdate += boardUI.BoradData_OnCellUpdate;

        // test
        boardData.SetCell(0, 0, 1);
        boardData.SetCell(2, 7, 1);
    }

    public bool CanPlaceBlock(BlockData blockData, BoardPosition? blockBaseBoardPosition)
    {
        if (blockBaseBoardPosition.HasValue)
        {
            return boardData.CanPlaceBlock(blockData, blockBaseBoardPosition.Value);
        }
        return false;
    }
}
