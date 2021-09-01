namespace SGoap
{
    /// <summary>
    /// Implement this to get access to AgentBasicData.
    /// </summary>
    public abstract class BasicCostEvaluator : CostEvaluator, IDataBind<AgentBasicData>
    {
        protected AgentBasicData AgentData;

        public void Bind(AgentBasicData data)
        {
            AgentData = data;
        }
    }
}
