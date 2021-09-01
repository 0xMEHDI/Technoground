using UnityEngine;
using SGoap;

public class RangeSensor : Sensor
{
	public TargetSensor TargetSensor;

	[HideInInspector] public float AttackRange;
	
	public Vector3 AgentPosition => Agent.transform.position;
	public Vector3 TargetPosition => TargetSensor.Target.transform.position;
	public float DistanceToTarget => Vector3.Distance(AgentPosition, TargetPosition);
	public bool InRange => DistanceToTarget <= AttackRange;

	public override void OnAwake(){}

    private void Update()
	{
		//Debug.Log("Is In Range: " + InRange);

		if (InRange)
			Agent.States.SetState("InAttackRange", 1);
		else
			Agent.States.RemoveState("InAttackRange");
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, AttackRange);
	}
}
