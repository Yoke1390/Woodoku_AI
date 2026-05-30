using System.Collections.Generic;

public interface IWoodokuAgent
{
    AgentAction SelectAction(Observation obs, IEnumerable<AgentAction> legalActions);
}
