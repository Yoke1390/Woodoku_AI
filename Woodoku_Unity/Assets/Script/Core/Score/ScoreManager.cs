using System;
using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core.Score
{
    public class ScoreManager : IReadOnlyScore
    {
        private int _streak;
        public int Score { get; private set; }

        public event Action<int> ScoreUpdate;

        public void Reset()
        {
            Score = 0;
            _streak = 0;

            ScoreUpdate?.Invoke(Score);
        }

        public void ApplyPlacement(PlacementResult result)
        {
            Score += ScoreRule.ScoreDiff(result, _streak);
            _streak = ScoreRule.NextStreak(result, _streak);

            ScoreUpdate?.Invoke(Score);
        }
    }
}
