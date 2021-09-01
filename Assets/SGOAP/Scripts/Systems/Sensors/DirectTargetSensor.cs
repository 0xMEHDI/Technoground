using UnityEngine;

namespace SGoap
{
    /// <summary>
    /// Simply sets the target on the agent.
    /// </summary>
    public class DirectTargetSensor : Sensor
    {
        public Transform Target;

        public override void OnAwake()
        {
            AgentData.Target = Target.transform;
            Agent.States.AddState("hasTarget", 1);
        }
    }
}
