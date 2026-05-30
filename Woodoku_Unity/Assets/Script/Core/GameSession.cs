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

    public GameSession(
        int gridSize,
        IEnumerable<BlockShape> blockShapes,
        int nHandSlots,
        int randomSeed
    )
    {
        boardData = new(gridSize);
        handManager = new(blockShapes, nHandSlots, randomSeed);
        scoreManager = new();

        handManager.HandSettled += CheckForGameOver;
    }

    public void Begin()
    {
        boardData.Reset();
        handManager.Reset();
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

        PlacementResult result = boardData.TryPlaceBlock(
            new PlacementAction(blockBaseBoardPosition, blockShape.Value)
        );

        if (result.IsSuccess)
        {
            scoreManager.ApplyPlacement(result);
            handManager.CommitPlacement(slotIndex);
        }

        return result;
    }
}
