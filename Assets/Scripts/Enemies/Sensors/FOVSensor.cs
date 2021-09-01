using UnityEngine;
using SGoap;

public class FOVSensor : Sensor
{
    public TargetSensor TargetSensor;
    RaycastHit hit;

    public Vector3 AgentPosition => Agent.transform.position;
    public Vector3 TargetPosition => TargetSensor.Target.transform.position;
    public bool IsVisible => IsHitValid();

    public override void OnAwake(){}

    private void Update()
    {
        //Debug.Log("Is Visible: " + IsVisible);

        if (IsVisible)
            Agent.States.SetState("IsVisible", 1);
        else
            Agent.States.RemoveState("IsVisible");
    }

    bool IsHitValid()
    {
        if (Physics.Raycast(AgentPosition, TargetPosition - AgentPosition, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Enemy"))
            {
                return false;
            }
        }
            
        return true;
    }
}
