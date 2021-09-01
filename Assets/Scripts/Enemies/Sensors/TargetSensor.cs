using UnityEngine;
using SGoap;

public class TargetSensor : Sensor
{
    [HideInInspector] public PlayerController Target;
    [HideInInspector] public float TargetingRange;

    public float DistanceToTarget => Vector3.Distance(Agent.transform.position, Target.transform.position);
    public bool HasTarget => Target != null && Target.isActiveAndEnabled && DistanceToTarget <= TargetingRange;

    public override void OnAwake(){}

    private void Update()
    {
        if (HasTarget)
        {
            Agent.transform.LookAt(Target.transform.position);
            Agent.States.AddState("HasTarget", 1);
        }
        else
        {
            Agent.States.RemoveState("HasTarget");
        }
    }
}
