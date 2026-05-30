using System.Collections.Generic;

public sealed class WoodokuEnv
{
    private readonly GameSession session;

    public WoodokuEnv(int gridSize, IEnumerable<BlockShape> shapes, int nSlots)
    {
        session = new(gridSize, shapes, nSlots);
    }

    public Observation GetObservation() => new(session.Board, session.Hands);

    public Observation Reset(int seed)
    {
        session.Begin(seed);
        return GetObservation();
    }

    public StepResult Step(AgentAction a)
    {
        int oldScore = session.Score.Score;
        PlacementResult result = session.TryPlaceBlock(a);
        int newScore = session.Score.Score;

        int reward = newScore - oldScore;
        bool done = session.State == GameState.GameOver;
        return new(GetObservation(), reward, done);
    }

    public IEnumerable<AgentAction> LegalActions => session.GetLegalActions();
}
