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
            int oldScore = _session.Score.Score;
            PlacementResult result = _session.TryPlaceBlock(a);
            int newScore = _session.Score.Score;

            int reward = newScore - oldScore;
            bool done = _session.Status == GameStatus.GameOver;
            return new StepResult(GetObservation(), reward, done);
        }
    }
}