using System.Collections;
using UnityEngine;
using SGoap;

public class ShootAction : BasicAction
{
    public float cooldown = 1f;
    public override float CooldownTime => cooldown;

    RangedEnemy Agent;
    TargetSensor TargetSensor;
    RangeSensor RangeSensor;
    FOVSensor FOVSensor;
    Gun equippedGun;

    private void Awake()
    {
        Agent = GetComponentInParent<RangedEnemy>();
        TargetSensor = Agent.TargetSensor;
        RangeSensor = Agent.RangeSensor;
        FOVSensor = Agent.FOVSensor;
    }

    public override EActionStatus Perform()
    {
        if (equippedGun == null)
        {
            equippedGun = Agent.equippedGun;
            return EActionStatus.Running;
        }

        if (TargetSensor.HasTarget && RangeSensor.InRange && FOVSensor.IsVisible)
        {
            if (!equippedGun.reloading)
            {
                StartCoroutine(Shoot());

                return EActionStatus.Success;
            }
            else
            {
                return EActionStatus.Running;
            }
        }
        else
        {
            return EActionStatus.Failed;
        }
    }

    IEnumerator Shoot()
    {
        //Debug.Log("Shooting");

        for (int i = 0; i < equippedGun.bullets.Length; i++)
        {
            float randomBulletSpread = Random.Range(-equippedGun.bulletSpread, equippedGun.bulletSpread);
            Quaternion randomRotation = Quaternion.Euler(randomBulletSpread, randomBulletSpread, randomBulletSpread);

            Bullet newBullet = Instantiate(equippedGun.bullets[i], equippedGun.muzzle.position, equippedGun.muzzle.rotation * randomRotation);
            newBullet.SetSpeed(equippedGun.bulletSpeed);

            Instantiate(equippedGun.shell, equippedGun.extractor.position, equippedGun.extractor.rotation * Quaternion.Euler(0, 180, 0));
            equippedGun.muzzleFlash.Activate();

            AudioManager.instance.PlaySound(equippedGun.shootAudio, Agent.transform.position, 0.75f);

            yield return new WaitForSeconds(0.1f);
        }

        equippedGun.bulletsRemaining = 0;
    }
}
