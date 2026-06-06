using UnityEngine;
using System.Collections.Generic;

public class OrbSpawner : MonoBehaviour
{
    public GameObject orbPrefab;
    public Transform playerTransform; // Drag Toto here in the inspector

    [Header("Spawn Distance Rules")]
    public float minDistanceFromPlayer = 2.0f;
    public float minDistanceBetweenOrbs = 1.2f;
    public int maxSpawnAttempts = 15;

    [Header("Arena Boundaries")]
    public float minX = -16f;
    public float maxX = 16f;
    public float minY = -12f;
    public float maxY = 12f;

    private string[] possibleStats = { "Attack", "Max health", "Movement", "Defense", "Heal" };

    public void SpawnWaveOrbs(int totalOrbsToSpawn)
    {
        List<Vector2> spawnedPositions = new List<Vector2>();
        OperationGenerator generator = GetComponent<OperationGenerator>();

        if (generator == null)
        {
            Debug.LogError("OrbSpawner cannot find OperationGenerator on the GameManager!");
            return;
        }

        for (int i = 0; i < totalOrbsToSpawn; i++)
        {
            Vector2 finalSpawnPosition = Vector2.zero;
            bool isValidPosition = false;

            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);
                Vector2 potentialPos = new Vector2(randomX, randomY);

                if (playerTransform != null && Vector2.Distance(potentialPos, playerTransform.position) < minDistanceFromPlayer)
                    continue;

                bool tooCloseToAnotherOrb = false;
                foreach (Vector2 existingPos in spawnedPositions)
                {
                    if (Vector2.Distance(potentialPos, existingPos) < minDistanceBetweenOrbs)
                    {
                        tooCloseToAnotherOrb = true;
                        break;
                    }
                }

                if (tooCloseToAnotherOrb) continue;

                finalSpawnPosition = potentialPos;
                isValidPosition = true;
                break;
            }

            // Fallback coordinate backup if position checks failed
            if (!isValidPosition)
            {
                finalSpawnPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            }

            // Instantiation Phase
            GameObject newOrb = Instantiate(orbPrefab, finalSpawnPosition, Quaternion.identity);
            spawnedPositions.Add(finalSpawnPosition);

            // Pass data down into your exact MathOrb parameters!
            MathOrb orbScript = newOrb.GetComponent<MathOrb>();
            if (orbScript != null)
            {
                // Generate question data
                MathOperation data = generator.GenerateQuestion();

                // Randomly generate GDD Boost details
                string randomStat = possibleStats[Random.Range(0, possibleStats.Length)];
                int randomBoostValue = Random.Range(2, 6); // Generates a random boost value between 2 and 5

                // Trigger your exact Initialization Method!
                orbScript.InitializeOrb(data.Op1, data.Op2, data.Op, data.Res, randomStat, randomBoostValue);
            }
        }
    }
}