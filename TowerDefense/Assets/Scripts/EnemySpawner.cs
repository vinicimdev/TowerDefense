using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private TextMeshProUGUI waveCounter;

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float spawnRate = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.5f;
    [SerializeField] private float enemiesPerSecondCap = 15f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    public static EnemySpawner main;
    public int currentWave = 1;
    private float timeSinceLastSpawn;
    private float eps; // enemies per second
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private void Awake()
    {
        main = this;
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        StartCoroutine(StartWave());    
    }

    private void Update()
    {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn >= 1f / eps && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if(enemiesAlive == 0 && enemiesLeftToSpawn == 0)
        {
            EndWave();
        }
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        eps = EnemiesPerSecond();
    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        waveCounter.text = currentWave.ToString();
        StartCoroutine(StartWave());
    }

    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = GetEnemyPrefab();
        GameObject newEnemy = Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);

        Health health = newEnemy.GetComponent<Health>();
        if (health != null)
        {
            float healthMultiplier = 1f + (currentWave - 1) * 0.2f;
            float rewardMultiplier = 1f + (currentWave - 1) * 0.15f;

            int scaledHealth = Mathf.RoundToInt(health.hitPoints * healthMultiplier);
            int scaledReward = Mathf.RoundToInt(health.goldWorth * rewardMultiplier);

            health.Initialize(scaledHealth, scaledReward);
        }
    }
    private GameObject GetEnemyPrefab()
    {
        float tankChance = 0f;

        if (currentWave >= 5) // por exemplo, tank aparece só da wave 5 em diante
        {
            tankChance = Mathf.Clamp01((currentWave - 4) * 0.1f);
            // Ex: wave 5 = 10%, wave 6 = 20%, etc. Máximo 100%.
        }

        float roll = Random.value;

        foreach (GameObject prefab in enemyPrefabs)
        {
            Health health = prefab.GetComponent<Health>();
            if (health != null)
            {
                if (roll < tankChance && health.type == EnemyType.Tank)
                    return prefab;

                if (roll >= tankChance && health.type == EnemyType.Normal)
                    return prefab;
            }
        }

        // fallback
        return enemyPrefabs[0];
    }


    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(spawnRate * Mathf.Pow(currentWave, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }
}
