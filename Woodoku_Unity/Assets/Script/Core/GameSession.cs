using System;
using System.Collections.Generic;

public enum GameState
{
    Playing,
    GameOver,
}

public class GameSession
{
    private readonly BoardData boardData;
    private readonly HandManager handManager;

    public GameState State { get; private set; }
    public IReadOnlyBoard Board => boardData;
    public IReadOnlyHands Hands => handManager;
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

        handManager.HandSettled += CheckForGameOver;
    }

    public void Begin()
    {
        handManager.Begin();
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
        foreach (BlockShape? blockShape in handManager.CurrentHand)
        {
            if (!blockShape.HasValue)
            {
                continue;
            }
            if (boardData.CanPlaceBlockInBoard(blockShape.Value))
            {
                return false;
            }
        }
        return true;
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

        PlacementResult result = boardData.TryPlaceBlock(blockShape.Value, blockBaseBoardPosition);

        if (result.IsSuccess)
        {
            handManager.CommitPlacement(slotIndex);
        }

        return result;
    }
}
