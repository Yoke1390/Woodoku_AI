using System;
using System.Collections.Generic;
using System.Linq;

public enum GameState
{
    Playing,
    GameOver,
}

public class GameSession
{
    private readonly BoardData boardData;
    private readonly HandManager handManager;
    private readonly ScoreManager scoreManager;

    public GameState State { get; private set; }
    public IReadOnlyBoard Board => boardData;
    public IReadOnlyHands Hands => handManager;
    public IReadOnlyScore Score => scoreManager;
    public event Action GameOver;

    private const int TestSeed = 1234;

    public GameSession(
        int gridSize,
        IEnumerable<BlockShape> blockShapes,
        int nHandSlots,
        int randomSeed = TestSeed
    )
    {
        boardData = new(gridSize);
        handManager = new(blockShapes, nHandSlots, randomSeed);
        scoreManager = new();

        handManager.HandSettled += CheckForGameOver;
    }

    public void Begin(int? seed = null)
    {
        boardData.Reset();
        handManager.Reset(seed);
        scoreManager.Reset();
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

    private bool IsGameOver() => !GetLegalActions().Any();

    public IEnumerable<AgentAction> GetLegalActions()
    {
        for (int slot = 0; slot < Hands.CurrentHand.Count; slot++)
        {
            BlockShape? shape = Hands.CurrentHand[slot];
            if (!shape.HasValue)
                continue;
            foreach (PlacementAction p in boardData.EnumerateLegalActions(shape.Value))
                yield return new AgentAction(slot, p.BasePosition);
        }
    }

    public PlacementResult TryPlaceBlock(int slotIndex, BoardPosition blockBaseBoardPosition)
    {
        if (State == GameState.GameOver)
            return PlacementResult.Failure();

        if (slotIndex < 0 || slotIndex >= handManager.CurrentHand.Count)
            return PlacementResult.Failure();

        BlockShape? blockShape = handManager.CurrentHand[slotIndex];

        if (!blockShape.HasValue)
            return PlacementResult.Failure();

        PlacementAction action = new(blockBaseBoardPosition, blockShape.Value);
        PlacementResult result = boardData.TryPlaceBlock(action);

        if (result.IsSuccess)
        {
            scoreManager.ApplyPlacement(result);
            handManager.CommitPlacement(slotIndex);
        }

        return result;
    }

    public PlacementResult TryPlaceBlock(AgentAction a)
    {
        return TryPlaceBlock(a.SlotIndex, a.BasePosition);
    }
}
