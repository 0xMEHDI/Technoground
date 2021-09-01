using SGoap;
using UnityEngine;

namespace SGoap
{
    public class AgentBasicData
    {
        public Transform Target { get; set; }
        public Animator Animator { get; set; }
        public Agent Agent { get; set; }
        public EffectController EffectsController { get; set; }
        public IInventory Inventory { get; set; }
        public CoolDown Cooldown { get; set; }

        public Vector3 DirectionToTarget => (Target.position - Agent.transform.position).normalized;
        public float DistanceToTarget => Vector3.Distance(Target.position, Agent.transform.position);

        public Vector3 Position
        {
            get => Agent.transform.position;
            set => Agent.transform.position = value;
        }

        public void LookAt(Vector3 position)
        {
            Agent.transform.LookAt(position);
        }
    }
}