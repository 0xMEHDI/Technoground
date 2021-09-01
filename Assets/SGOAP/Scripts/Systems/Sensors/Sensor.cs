using UnityEngine;

namespace SGoap
{
    /// <summary>
    /// A sensor is a communication point between actions and the agent.
    /// </summary>
    public abstract class Sensor : MonoBehaviour, IDataBind<AgentBasicData>, IDataBind<Agent>
    {
        protected AgentBasicData AgentData;
        protected Agent Agent { get; private set; }

        public void Bind(AgentBasicData data)
        {
            AgentData = data;
        }

        public void Bind(Agent agent)
        {
            Agent = agent;
            OnAwake();
        }

        public abstract void OnAwake();
    }
}