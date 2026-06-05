using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core
{
    public enum GameState
    {
        Playing,
        GameOver
    }

    public class GameSession
    {
        private const int TestSeed = 1234;
        private readonly BoardData _boardData;
        private readonly HandManager _handManager;
        private readonly ScoreManager _scoreManager;

        public GameSession(
            int gridSize,
            IEnumerable<BlockShape> blockShapes,
            int nHandSlots,
            int seed = TestSeed
        )
        {
            _boardData = new BoardData(gridSize);
            _handManager = new HandManager(blockShapes, nHandSlots, seed);
            _scoreManager = new ScoreManager();

            _handManager.HandSettled += CheckForGameOver;
        }

        public GameState State { get; private set; }
        public IReadOnlyBoard Board => _boardData;
        public IBoardEventPublisher BoardEvent => _boardData;
        public IReadOnlyHands Hands => _handManager;
        public IReadOnlyScore Score => _scoreManager;
        public event Action GameOver;

        public void Begin(int? seed = null)
        {
            _boardData.Reset();
            _handManager.Reset(seed);
            _scoreManager.Reset();
            State = GameState.Playing;
        }

        private void CheckForGameOver()
        {
            if (IsGameOver())
            {
                State = GameState.GameOver;
                GameOver?.Invoke();
            }
        }

        private bool IsGameOver()
        {
            return !GetLegalActions().Any();
        }

        public IEnumerable<AgentAction> GetLegalActions()
        {
            for (int slot = 0; slot < Hands.CurrentHand.Count; slot++)
            {
                BlockShape? shape = Hands.CurrentHand[slot];
                if (!shape.HasValue)
                    continue;
                foreach (PlacementAction p in _boardData.EnumerateLegalActions(shape.Value))
                    yield return new AgentAction(slot, p.BasePosition);
            }
        }

        public PlacementResult TryPlaceBlock(int slotIndex, BoardPosition blockBaseBoardPosition)
        {
            if (State == GameState.GameOver)
                return PlacementResult.Failure();

            if (slotIndex < 0 || slotIndex >= _handManager.CurrentHand.Count)
                return PlacementResult.Failure();

            BlockShape? blockShape = _handManager.CurrentHand[slotIndex];

            if (!blockShape.HasValue)
                return PlacementResult.Failure();

            PlacementAction action = new(blockBaseBoardPosition, blockShape.Value);
            PlacementResult result = _boardData.TryPlaceBlock(action);

            if (result.IsSuccess)
            {
                _scoreManager.ApplyPlacement(result);
                _handManager.CommitPlacement(slotIndex);
            }

            return result;
        }

        public PlacementResult TryPlaceBlock(AgentAction a)
        {
            return TryPlaceBlock(a.SlotIndex, a.BasePosition);
        }
    }
}
