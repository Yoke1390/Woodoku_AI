using System.Collections.Generic;
using Script.Core;
using Script.Core.Primitive;
using Script.Unity.Board;
using Script.Unity.Hand;
using UnityEngine;

namespace Script.Unity
{
    public class WoodokuGameManager : MonoBehaviour
    {
        private const int NHandSlots = 3;

        [SerializeField] private BoardUI boardUI;

        [SerializeField] private GameOverUI gameOverUI;

        [SerializeField] private GameSetting gameSetting;

        [SerializeField] private HandUI handUI;

        [SerializeField] private ScoreUI scoreUI;

        private GameSession _gameSession;

        private Camera _uiCamera;

        private void Start()
        {
            Initialize();
            _gameSession.Begin();
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

            _gameSession = new GameSession(gameSetting.GridSize, blockShapes, NHandSlots);

            boardUI.Initialize(_gameSession.Board, _gameSession.BoardEvent);
            handUI.Initialize(
                HandleEndBlockMoveRequest,
                boardUI.CellSize,
                _gameSession.Hands,
                gameSetting.BlockControlMode
            );

            _gameSession.Score.ScoreUpdate += scoreUI.UpdateScore;
            _gameSession.GameOver += OnGameOver;
            gameOverUI.Restart += OnRestart;
        }

        private void OnRestart()
        {
            gameOverUI.Hide();
            _gameSession.Begin();
        }

        private void OnGameOver()
        {
            gameOverUI.Show();
        }

        private bool HandleEndBlockMoveRequest(Vector2 screenPoint, int slotIndex)
        {
            var blockShape = _gameSession.Hands.CurrentHand[slotIndex];

            if (!blockShape.HasValue) return false;

            if (!boardUI.TryScreenPointToBoardPosition(
                    screenPoint,
                    _uiCamera,
                    blockShape.Value.Center(),
                    out var blockBaseBoardPosition
                )) return false;
            var result = _gameSession.TryPlaceBlock(slotIndex, blockBaseBoardPosition);
            return result.IsSuccess;
        }
    }
}
