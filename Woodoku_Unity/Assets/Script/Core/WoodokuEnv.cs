using System.Collections.Generic;
using Script.Core.Primitive;

namespace Script.Core
{
    public sealed class WoodokuEnv
    {
        private readonly GameSession _session;

        public WoodokuEnv(int gridSize, IEnumerable<BlockShape> shapes, int nSlots)
        {
            _session = new GameSession(gridSize, shapes, nSlots);
        }

        public IEnumerable<AgentAction> LegalActions => _session.GetLegalActions();

        public Observation GetObservation()
        {
            return new Observation(_session.Board, _session.Hands);
        }

        public Observation Reset(int seed)
        {
            _session.Begin(seed);
            return GetObservation();
        }

        public StepResult Step(AgentAction a)
        {
            var oldScore = _session.Score.Score;
            var result = _session.TryPlaceBlock(a);
            var newScore = _session.Score.Score;

            var reward = newScore - oldScore;
            var done = _session.State == GameState.GameOver;
            return new StepResult(GetObservation(), reward, done);
        }
    }
}
