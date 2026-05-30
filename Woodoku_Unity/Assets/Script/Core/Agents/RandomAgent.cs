using System;
using System.Collections.Generic;
using System.Linq;

public class RandomAgent : IWoodokuAgent
{
    private Random random;

    public RandomAgent(int seed = 1234)
    {
        random = new(seed);
    }

    public AgentAction SelectAction(Observation obs, IEnumerable<AgentAction> legalActions)
    {
        AgentAction[] actions = legalActions.ToArray();
        return actions[random.Next(actions.Length)];
    }
}
