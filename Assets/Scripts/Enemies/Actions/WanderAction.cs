using UnityEngine;
using UnityEngine.AI;
using SGoap;

public class WanderAction : BasicAction
{
    public float cooldown = 5f;
    public override float CooldownTime => cooldown;

    EnemyController Agent;
    NavMeshAgent NavMeshAgent;
    TargetSensor TargetSensor;
    RangeSensor RangeSensor;
    FOVSensor FOVSensor;

    private void Awake()
    {
        Agent = GetComponentInParent<EnemyController>();
        NavMeshAgent = Agent.GetComponent<NavMeshAgent>();
        RangeSensor = Agent.RangeSensor;
        TargetSensor = Agent.TargetSensor;
        FOVSensor = Agent.FOVSensor;
    }

    public override EActionStatus Perform()
    {
        if (TargetSensor.HasTarget)
        {
            if (!FOVSensor.IsVisible)
            {
                //Debug.Log("Wandering");

                Vector3 randomPosition = Random.insideUnitCircle * 10f;
                Vector3 targetPosition = Agent.transform.position + randomPosition;

                NavMesh.SamplePosition(targetPosition, out NavMeshHit navHit, RangeSensor.DistanceToTarget, -1);
                //NavMeshAgent.SetDestination(navHit.position);

                return EActionStatus.Running;
            }
            else
            {
                return EActionStatus.Success;
            }
        }
        else
        {
            return EActionStatus.Failed;
        }
    }
}
