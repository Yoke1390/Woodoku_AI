using System.Collections.Generic;
using UnityEngine;

public class WoodokuGameManager : MonoBehaviour
{
    public const int NHandSlots = 3;

    private Camera _uiCamera;

    [SerializeField] private BoardUI boardUI;

    [SerializeField] private GameOverUI gameOverUI;

    private GameSession gameSession;

    [SerializeField] private GameSetting gameSetting;

    [SerializeField] private HandUI handUI;

    [SerializeField] private ScoreUI scoreUI;

    private void Start()
    {
        Initialize();
        gameSession.Begin();
    }

    private void Initialize()
    {
        _uiCamera = boardUI.GetComponentInParent<Canvas>().rootCanvas.worldCamera;

        var blockDatas = Resources.LoadAll<BlockData>("");
        List<BlockShape> blockShapes = new();
        foreach (var data in blockDatas)
        {
            var shape = data.ToShape();
            blockShapes.Add(shape);
        }

        gameSession = new GameSession(gameSetting.GridSize, blockShapes, NHandSlots);

        boardUI.Initialize(gameSession.Board);
        handUI.Initialize(
            HandleEndBlockMoveRequest,
            boardUI.CellSize,
            gameSession.Hands,
            gameSetting.BlockControlMode
        );

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

    private bool HandleEndBlockMoveRequest(Vector2 screenPoint, int slotIndex)
    {
        var blockShape = gameSession.Hands.CurrentHand[slotIndex];

        if (blockShape is not BlockShape shape) return false;
        if (
            boardUI.TryScreenPointToBoardPosition(
                screenPoint,
                _uiCamera,
                shape.Center(),
                out var blockBaseBoardPosition
            )
        )
        {
            var result = gameSession.TryPlaceBlock(slotIndex, blockBaseBoardPosition);
            return result.IsSuccess;
        }

        return false;
    }
}
