using Script.Core.Primitive;

namespace Script.Core.Score
{
    public static class ScoreRule
    {
        private const int SingleClearScore = 18;
        private const int ComboBonus = 10;
        private const int StreakBonus = 10;

        public static int ScoreDiff(PlacementResult result, int streak)
        {
            int diff = 0;
            diff += result.BlockShape.NBlocks;

            if (result.NClearedTimes > 0)
            {
                diff += result.NClearedTimes * SingleClearScore;
                diff += (result.NClearedTimes - 1) * ComboBonus;
                diff += streak * StreakBonus;
            }

            return diff;
        }

        public static int NextStreak(PlacementResult result, int streak)
        {
            if (result.NClearedTimes > 0)
                streak++;
            else
                streak = 0;

            return streak;
        }
    }
}
