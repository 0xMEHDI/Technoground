using UnityEngine;
using UnityEngine.AI;
using SGoap;

public class GoToAction : BasicAction
{
    public float cooldown = 1f;
    public override float CooldownTime => cooldown;

    EnemyController Agent;
    NavMeshAgent NavMeshAgent;
    TargetSensor TargetSensor;
    RangeSensor RangeSensor;

    private void Awake()
    {
        Agent = GetComponentInParent<EnemyController>();
        NavMeshAgent = Agent.GetComponent<NavMeshAgent>();
        TargetSensor = Agent.TargetSensor;
        RangeSensor = Agent.RangeSensor;
    }

    public override EActionStatus Perform()
    {
        if (TargetSensor.HasTarget)
        {
            if (!RangeSensor.InRange)
            {
                //Debug.Log("Moving");

                Vector3 targetPosition = TargetSensor.Target.transform.position;

                NavMesh.SamplePosition(targetPosition, out NavMeshHit navHit, RangeSensor.AttackRange, -1);
                NavMeshAgent.SetDestination(navHit.position);

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