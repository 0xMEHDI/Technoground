using System.Collections;
using UnityEngine;
using SGoap;

public class LungeAction : BasicAction
{
    public float speed = 2f;
    public float cooldown = 2f;
    public override float CooldownTime => cooldown;

    MeleeEnemy Agent;
    TargetSensor TargetSensor;
    RangeSensor RangeSensor;
    FOVSensor FOVSensor;

    Material material;
    public AudioClip soundEffect;

    private void Awake()
    {
        Agent = GetComponentInParent<MeleeEnemy>();
        material = Agent.GetComponent<MeshRenderer>().material;
        TargetSensor = Agent.TargetSensor;
        RangeSensor = Agent.RangeSensor;
        FOVSensor = Agent.FOVSensor;
    }

    public override EActionStatus Perform()
    {
        if (TargetSensor.HasTarget && RangeSensor.InRange && FOVSensor.IsVisible)
        {
            Agent.transform.LookAt(TargetSensor.Target.transform.position);
            StartCoroutine(StartAttack());

            return EActionStatus.Success;
        }
        else
        {
            return EActionStatus.Failed;
        }
    }

    IEnumerator StartAttack()
    {
        //Debug.Log("Lunging");

        AudioManager.instance.PlaySound(soundEffect, Agent.transform.position);

        Color materialColor = material.color;
        material.color = Color.red;

        Vector3 startingPosition = Agent.transform.position;
        Vector3 targetPosition = TargetSensor.Target.transform.position;

        float attackProgress = 0f;

        while (attackProgress <= 1f)
        {
            attackProgress += Time.deltaTime * speed;
            float interpolation = 4 * (-attackProgress * attackProgress + attackProgress);
            Agent.transform.position = Vector3.Lerp(startingPosition, targetPosition, interpolation);

            yield return null;
        }

        material.color = materialColor;
    }
}