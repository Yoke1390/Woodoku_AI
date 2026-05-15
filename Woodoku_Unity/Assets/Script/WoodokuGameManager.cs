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

    [SerializeField]
    private GameSetting gameSetting;

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
        boardData = new BoardData(gameSetting.GridSize);
        boardUI.Initialize(boardData);
        handManager.Initialize();

        boardData.CellUpdate += boardUI.BoradData_OnCellUpdate;

        // test
        boardData.SetCell(0, 0, 1);
        boardData.SetCell(2, 7, 1);
    }

    public bool CanPlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        return boardData.CanPlaceBlock(blockData, blockBaseBoardPosition);
    }

    public bool TryPlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        bool canPlace = CanPlaceBlock(blockData, blockBaseBoardPosition);
        if (canPlace)
        {
            PlaceBlock(blockData, blockBaseBoardPosition);
        }
        return canPlace;
    }

    private void PlaceBlock(BlockData blockData, BoardPosition blockBaseBoardPosition)
    {
        foreach (BoardPosition blockPiecePosition in blockData.BlockCells)
        {
            BoardPosition pos = blockBaseBoardPosition + blockPiecePosition;
            boardData.SetCell(pos, 1);
        }
    }
}
