public readonly struct StepResult
{
    public Observation Observation { get; }
    public int Reward { get; }
    public bool Done { get; }

    public StepResult(Observation observation, int reward, bool done)
    {
        Observation = observation;
        Reward = reward;
        Done = done;
    }
}

public readonly struct Observation
{
    public IReadOnlyBoard Board { get; }
    public IReadOnlyHands Hands { get; }

    public Observation(IReadOnlyBoard board, IReadOnlyHands hands)
    {
        Board = board;
        Hands = hands;
    }
}
