using System;
using System.Collections.Generic;
using Script.Core.Primitive;

namespace Script.Core
{
    public static class GameRules
    {
        public static (GameStateData next, PlacementResult result)
            ApplyPlacement(in GameStateData s, AgentAction a)
        {
            // 1. slotからshape解決。空 or 範囲外 → (s, PlacementResult.Failure())
            BlockShape? shape = s.Hand.CurrentHand[a.SlotIndex];


            // 2. PlacementAction構築 → BoardRule.CanPlaceBlock不可なら (s, Failure)
            // 3. BoardSimulator.SimulatePlaceAndClear → (newBoard, result)
            // 4. score = s.Score + ScoreRule.ScoreDiff(result, s.Streak)
            //    streak = ScoreRule.NextStreak(result, s.Streak)
            // 5. hand = s.Hand.Consume(a.SlotIndex)
            // 6. Statusは据え置き（補充前なので確定させない）

            return default;
        }

        public static GameStateData RefillIfNeeded(in GameStateData s, RuleSet rules)
        {
            // hand.IsEmpty のときだけ:
            //   rng を NHandSlots 回 Next(ShapePool.Count) で引いて shapes 生成
            //   hand = hand.Refilled(shapes), rng = 進めたrng
            // 最後に Status = IsGameOver(state) ? GameOver : Playing を確定
            return default;
        }

        public static IEnumerable<AgentAction> LegalActions(in GameStateData s) // 旧GetLegalActionsを純粋化
        {
            return Array.Empty<AgentAction>();
        }

        public static bool IsGameOver(in GameStateData s) // !LegalActions().Any()
        {
            return false;
        }
    }
}
