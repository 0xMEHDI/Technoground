using System;
using UnityEngine;

public class RangedEnemy : EnemyController
{
    [Header("Gun")]
    public Gun startingGun;
    public Transform gunPosition;

    Vector3 smoothTime;
    [HideInInspector] public Gun equippedGun;

    protected override void Start()
    {
        base.Start();

        navMeshAgent.stoppingDistance = RangeSensor.AttackRange;

        if (startingGun != null)
            EquipGun(startingGun);
    }

    protected override void Update()
    {
        if (equippedGun != null)
            equippedGun.transform.SetPositionAndRotation(gunPosition.position, gunPosition.rotation);

        base.Update();
    }

    void FixedUpdate()
    {
        gunPosition.localPosition = Vector3.SmoothDamp(gunPosition.localPosition, new Vector3(0.575f, 0, -0.25f), ref smoothTime, 0.05f);
    }

    void EquipGun(Gun gun)
    {
        if (equippedGun != null)
            Destroy(equippedGun);

        equippedGun = Instantiate(gun, gunPosition.position, gunPosition.rotation);
        equippedGun.transform.parent = gameObject.transform;
    }
}
