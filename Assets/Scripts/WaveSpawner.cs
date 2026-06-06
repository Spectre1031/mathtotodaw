using UnityEngine;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab Setup")]
    public GameObject monsterPrefab; // Drag Monster_Placeholder here

    [Header("Wave Timing Configurations")]
    private float spawnInterval = 5f;  // Spawn every 5 seconds
    private int monstersPerInterval = 3;
    private float spawnTimer;
    private bool isWaveActive = false;

    [Header("Off-Screen Boundaries")]
    // Just slightly wider than your orb boundaries so they spawn off-screen
    public float spawnRadius = 9.5f;
    void OnEnable()
    {
        TimeTracker.OnTimeOut += EndWave;
    }
    void Start()
    {
        Debug.Log($"Starting new wave: isWaveActive: {isWaveActive}");
        StartWave();
    }

    void Update()
    {
        if (!isWaveActive) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnMonsterGroup();
            spawnTimer = 0f;
        }
    }

    void StartWave()
    {
        Debug.Log("Initializing Wave 1... Get ready to survive!");
        // waveTimer = waveDuration;
        spawnTimer = 0f;
        isWaveActive = true;
        Debug.Log("Wave 1 Started! Survival clock ticking...");
    }

    void SpawnMonsterGroup()
    {
        if(!isWaveActive || GameOverController.isPaused) return;
        Debug.Log("Spawning a new group of monsters...");
        for (int i = 0; i < monstersPerInterval; i++)
        {
            // Pick a completely random direction angle
            float randomAngle = Random.Range(0f, Mathf.PI * 2f);

            // Project the coordinate out onto the off-screen radius circle ring
            float spawnX = Mathf.Cos(randomAngle) * spawnRadius;
            float spawnY = Mathf.Sin(randomAngle) * spawnRadius;
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

            // Spawn the physical monster enemy
            Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        }
        Debug.Log($"Spawned a wave segment of {monstersPerInterval} enemies!");
    }

    void EndWave()
    {
        isWaveActive = false;
        Debug.Log("Closed wave spawner!");
    }
}