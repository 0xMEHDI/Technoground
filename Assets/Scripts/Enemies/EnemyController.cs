using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Base Stats")]
    public float health = 5f;
    public float movementSpeed = 5f;
    public float targetingRange = 10f;
    public float attackRange = 4f;

    [Header("Audio & Effects")]
    public AudioClip deathClip1;
    public AudioClip deathClip2;
    public ParticleSystem deathFX;

    [Header("Sensors")]
    public TargetSensor TargetSensor;
    public RangeSensor RangeSensor;
    public FOVSensor FOVSensor;

    protected float startingHealth;
    protected Material material;
    protected NavMeshAgent navMeshAgent;
    [HideInInspector] public PlayerController Target;

    public event Action OnDeath;

    Vector3 randomPosition;
    float nextRandomPosition;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;
        Target = FindObjectOfType<PlayerController>();
    }

    protected virtual void Start()
    {
        startingHealth = health;
        navMeshAgent.speed = movementSpeed;
        navMeshAgent.stoppingDistance = 1f;
        TargetSensor.Target = Target;
        TargetSensor.TargetingRange = targetingRange;
        RangeSensor.AttackRange = attackRange;

        randomPosition = UnityEngine.Random.insideUnitCircle * 10f;
    }

    protected virtual void Update()
    {
        if (Target != null && !FOVSensor.IsVisible)
        {
            if (Time.time > nextRandomPosition)
            {
                randomPosition = UnityEngine.Random.insideUnitCircle * 10f;
                Vector3 targetPosition = Target.transform.position + randomPosition;

                NavMesh.SamplePosition(targetPosition, out NavMeshHit navHit, RangeSensor.DistanceToTarget, -1);
                navMeshAgent.SetDestination(navHit.position);

                nextRandomPosition = Time.time + 2f;
            }
        }

        CheckHealth();
    }

    void CheckHealth()
    {
        if (health <= 0)
        {
            System.Random rand = new System.Random();
            if (rand.Next(0, 2) == 0)
                AudioManager.instance.PlaySound(deathClip1, transform.position);
            else
                AudioManager.instance.PlaySound(deathClip2, transform.position);

            StaticStatTracker.UpdatePlayerScore((int)startingHealth);

            ParticleSystem newDeathFX = Instantiate(deathFX, transform.position, Target.transform.rotation);

            OnDeath?.Invoke();

            Destroy(gameObject);
            Destroy(newDeathFX.gameObject, 2f);
        }
    }

    public void UpdateStats(float health, float speed, Color color)
    {
        this.health = health;
        navMeshAgent.speed = speed;
        material.color = color;
        deathFX.GetComponent<ParticleSystemRenderer>().sharedMaterial.color = color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
            health--;
    }
}
