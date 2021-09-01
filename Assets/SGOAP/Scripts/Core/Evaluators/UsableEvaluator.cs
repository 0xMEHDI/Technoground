using UnityEngine;

namespace SGoap
{
    /// <summary>
    /// Evaluating true will allow an action to be achievable and considered for the plan.
    /// </summary>
    public abstract class UsableEvaluator : MonoBehaviour, IDataBind<AgentBasicData>
    {
        [TextArea]
        public string Notes;

        protected AgentBasicData AgentData;

        public abstract bool Evaluate(IContext context);

        public void Bind(AgentBasicData data)
        {
            AgentData = data;
        }
    }
}