using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnHandler : MonoBehaviour
{
    [SerializeField] EnemyController meleeEnemy;
    [SerializeField] EnemyController rangedEnemy;
    [SerializeField] float transitionDelay = 2f;
    [SerializeField] float campCheckCooldown = 4f;
    [SerializeField] float campCheckDistance = 2f;
    [SerializeField] Wave[] waves;
    
    Wave currentWave;
    int currentWaveIndex;
    int enemiesRemaining;
    int enemiesLeftToSpawn;
    float timeLeftToSpawn;
    
    PlayerController player;
    Vector3 previousPlayerPosition;
    float timeToCampCheck;
    bool camping;
    bool dead;

    MapGenerator map;

    public AudioClip levelSuccessClip;

    public event System.Action<int> OnWaveStart;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        timeToCampCheck = campCheckCooldown + Time.time;
        previousPlayerPosition = player.transform.position;
        player.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();

        StartCoroutine(StartWave());
    }

    void Update()
    {
        if (!dead)
        {
            if (Time.time > timeToCampCheck)
            {
                timeToCampCheck = campCheckCooldown + Time.time;

                camping = (Vector3.Distance(player.transform.position, previousPlayerPosition) < campCheckDistance);

                previousPlayerPosition = player.transform.position;
            }

            if (enemiesLeftToSpawn > 0 && Time.time > timeLeftToSpawn)
            {
                enemiesLeftToSpawn--;
                timeLeftToSpawn = Time.time + currentWave.enemySpawnDelay;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    void OnPlayerDeath()
    {
        dead = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemaining--;

        if (enemiesRemaining == 0)
        {
            AudioManager.instance.PlaySound(levelSuccessClip, player.transform.position);

            if (currentWaveIndex == waves.Length - 1)
            {
                currentWaveIndex = 0;
            }

            StartCoroutine(StartWave());
        }
            
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(transitionDelay);

        currentWaveIndex++;

        if (currentWaveIndex - 1 < waves.Length)
        {
            currentWave = waves[currentWaveIndex - 1];

            enemiesLeftToSpawn = currentWave.enemyCount;
            enemiesRemaining = enemiesLeftToSpawn;
        }

        OnWaveStart?.Invoke(currentWaveIndex);

        player.transform.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up;
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;
        float spawnSpeed = 4f;

        Transform tile = map.GetRandomOpenTile();

        if (camping)
            tile = map.GetTileFromPosition(player.transform.position);

        Material material = tile.GetComponent<MeshRenderer>().material;

        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(spawnTimer * spawnSpeed, 1));;

            spawnTimer += Time.deltaTime;

            yield return null;
        }

        if (tile != null)
        {
            EnemyController spawnedEnemy;

            float random = UnityEngine.Random.Range(0, 2);
            if (random == 0)
                spawnedEnemy = Instantiate(meleeEnemy, tile.position + Vector3.up, Quaternion.identity);
            else
                spawnedEnemy = Instantiate(rangedEnemy, tile.position + Vector3.up, Quaternion.identity);

            spawnedEnemy.UpdateStats(currentWave.enemyHealth, currentWave.enemySpeed, currentWave.enemyColor);
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }   
    }

    [Serializable]
    public class Wave
    {
        public float enemyHealth;
        public float enemySpeed;
        public Color enemyColor;

        public int enemyCount;
        public float enemySpawnDelay;
    }
}
