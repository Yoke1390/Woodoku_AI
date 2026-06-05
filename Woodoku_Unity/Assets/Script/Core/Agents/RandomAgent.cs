using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Primitive;

namespace Script.Core.Agents
{
    public class RandomAgent : IWoodokuAgent
    {
        private readonly Random _random;

        public RandomAgent(int seed = 1234)
        {
            _random = new Random(seed);
        }

        public AgentAction SelectAction(Observation obs, IEnumerable<AgentAction> legalActions)
        {
            AgentAction[] actions = legalActions.ToArray();
            return actions[_random.Next(actions.Length)];
        }
    }
}
