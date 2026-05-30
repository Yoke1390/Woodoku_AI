using System;

public class ScoreManager : IReadOnlyScore
{
    public int Score { get; private set; } = 0;

    private int streak = 0;

    private const int SingleClearScore = 18;
    private const int ComboBonus = 10;
    private const int StreakBonus = 10;

    public event Action<int> ScoreUpdate;

    public void Reset()
    {
        Score = 0;
        streak = 0;

        ScoreUpdate?.Invoke(Score);
    }

    public void ApplyPlacement(PlacementResult result)
    {
        int diff = 0;
        diff += result.BlockShape.NBlocks;

        if (result.NClearedTimes > 0)
        {
            diff += result.NClearedTimes * SingleClearScore;
            diff += (result.NClearedTimes - 1) * ComboBonus;
            diff += streak * StreakBonus;

            streak++;
        }
        else
        {
            streak = 0;
        }

        Score += diff;
        ScoreUpdate?.Invoke(Score);
    }
}
