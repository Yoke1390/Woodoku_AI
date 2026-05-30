using System;
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

    [SerializeField]
    private GameOverUI gameOverUI;

    [SerializeField]
    private ScoreUI scoreUI;

    private GameSession gameSession;

    public const int NHandSlots = 3;

    void Start()
    {
        Initialize();
        gameSession.Begin();
    }

    private void Initialize()
    {
        BlockData[] blockDatas = Resources.LoadAll<BlockData>("");
        List<BlockShape> blockShapes = new();
        foreach (BlockData data in blockDatas)
        {
            BlockShape shape = data.ToShape();
            blockShapes.Add(shape);
        }

        gameSession = new(gameSetting.GridSize, blockShapes, NHandSlots);

        boardUI.Initialize(gameSession.Board);
        handUI.Initialize(HandleDropRequest, boardUI.CellSize, gameSession.Hands);

        gameSession.Score.ScoreUpdate += scoreUI.UpdateScore;
        gameSession.GameOver += OnGameOver;
        gameOverUI.Restart += OnRestart;
    }

    private void OnRestart()
    {
        gameOverUI.Hide();
        gameSession.Begin();
    }

    private void OnGameOver()
    {
        gameOverUI.Show();
    }

    private bool HandleDropRequest(PointerEventData eventData, int slotIndex)
    {
        BlockShape? blockShape = gameSession.Hands.CurrentHand[slotIndex];

        if (blockShape is not BlockShape shape)
        {
            return false;
        }
        if (
            boardUI.TryScreenPointToBoardPosition(
                eventData.position,
                eventData.pressEventCamera,
                shape.Center(),
                out BoardPosition blockBaseBoardPosition
            )
        )
        {
            PlacementResult result = gameSession.TryPlaceBlock(slotIndex, blockBaseBoardPosition);
            return result.IsSuccess;
        }
        return false;
    }
}
