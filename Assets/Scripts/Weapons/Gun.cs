using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    enum FireMode { Auto, Burst, Single }

    [Header("Stats")]
    public float rateOfFire = 10f;
    public float bulletSpeed = 10f;
    public float reloadSpeed = 2f;
    public float bulletSpread = 10f;
    public int bulletsPerMagazine = 50;

    [Header("Fire Mode")]
    [SerializeField] FireMode fireMode = FireMode.Auto;
    public int shotsPerBurst = 3;

    [Header("Parts")]
    public Shell shell;
    public Transform muzzle;
    public Transform extractor;
    public Bullet[] bullets;

    [Header("Audio")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
 
    [HideInInspector] public bool reloading;
    [HideInInspector] public bool triggerPressed;
    [HideInInspector] public float timeTillShot;
    [HideInInspector] public int bulletsRemaining;
    [HideInInspector] public int shotsPerBurstRemaining;
    [HideInInspector] public MuzzleFlash muzzleFlash;

    void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    void Start()
    {
        shotsPerBurstRemaining = shotsPerBurst;
        bulletsRemaining = bulletsPerMagazine;

        StaticStatTracker.UpdatePlayerMaxAmmoCount(bulletsRemaining);
    }

    void Update()
    {
        StaticStatTracker.UpdatePlayerAmmoCount(bulletsRemaining);

        if (!reloading && bulletsRemaining <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
            StartCoroutine(Reload());
        } 
    }

    public void OnTriggerPress()
    {
        Fire();
        triggerPressed = true;
    }

    public void OnTriggerRelease()
    {
        shotsPerBurstRemaining = shotsPerBurst;
        triggerPressed = false;
    }

    void Fire()
    {
        if (Time.time > timeTillShot && bulletsRemaining > 0 && !reloading)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsPerBurstRemaining == 0)
                    return;

                shotsPerBurstRemaining--;
            }

            if (fireMode == FireMode.Single)
            {
                if (triggerPressed)
                    return;
            }

            if (bulletsRemaining != 0)
            {
                for (int i = 0; i < bullets.Length; i++)
                {
                    float randomBulletSpread = Random.Range(-bulletSpread, bulletSpread);
                    Quaternion randomRotation = Quaternion.Euler(randomBulletSpread, randomBulletSpread, randomBulletSpread);

                    timeTillShot = Time.time + 100 / rateOfFire;
                    Bullet newBullet = Instantiate(bullets[i], muzzle.position, muzzle.rotation * randomRotation);
                    newBullet.SetSpeed(bulletSpeed);
                    bulletsRemaining--;
                }

                Instantiate(shell, extractor.position, extractor.rotation * Quaternion.Euler(0, 180, 0));
                muzzleFlash.Activate();

                StaticStatTracker.UpdatePlayerAmmoCount(bulletsRemaining);
                AudioManager.instance.PlaySound(shootAudio, transform.position, 0.75f);
            }
        }
    }

    IEnumerator Reload()
    {
        reloading = true;

        yield return new WaitForSeconds(0.25f);

        float percent = 0;
        Vector3 rotation = transform.eulerAngles;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolation = 4 * (-Mathf.Pow(percent, 2) + percent);
            float reloadAngle = Mathf.Lerp(0, 30, interpolation);
            transform.eulerAngles = rotation + Vector3.right * reloadAngle;

            yield return null;
        }

        reloading = false;
        bulletsRemaining = bulletsPerMagazine;
    }
}