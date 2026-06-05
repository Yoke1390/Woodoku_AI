using System.Collections.Generic;
using Script.Core.Primitive;

namespace Script.Core.Agents
{
    public interface IWoodokuAgent
    {
        AgentAction SelectAction(Observation obs, IEnumerable<AgentAction> legalActions);
    }
}