using System;
using Script.Core.Primitive;

namespace Script.Core
{
    public class ScoreManager : IReadOnlyScore
    {
        private const int SingleClearScore = 18;
        private const int ComboBonus = 10;
        private const int StreakBonus = 10;

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
            var diff = 0;
            diff += result.BlockShape.NBlocks;

            if (result.NClearedTimes > 0)
            {
                diff += result.NClearedTimes * SingleClearScore;
                diff += (result.NClearedTimes - 1) * ComboBonus;
                diff += _streak * StreakBonus;

                _streak++;
            }
            else
            {
                _streak = 0;
            }

            Score += diff;
            ScoreUpdate?.Invoke(Score);
        }
    }
}
