using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] fruitPrefabs;
    [SerializeField] private int minSpawnCount = 1;
    [SerializeField] private int maxSpawnCount = 3;
    [SerializeField] private float timeBetweenWaves = 0.5f;
    [SerializeField] private float minDistanceBetweenFruits = 2f;

    private Camera mainCamera;
    private readonly List<FruitClick> activeFruits = new List<FruitClick>();
    private GameManager gameManager;

    private void Awake()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void NotifyFruitEaten(FruitClick eatenFruit)
    {
        if (activeFruits.Contains(eatenFruit))
        {
            activeFruits.Remove(eatenFruit);
        }

        Destroy(eatenFruit.gameObject);

        if (activeFruits.Count == 0)
        {
            StartCoroutine(SpawnNextWave());
        }
    }

    private IEnumerator SpawnNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        if (gameManager != null && !gameManager.IsGameRunning())
        {
            yield break;
        }

        SpawnWave();
    }

    public void SpawnWave()
    {
        ClearMissingReferences();

        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
        {
            Debug.LogWarning("No fruit prefabs assigned to FruitSpawner.");
            return;
        }

        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInsideScreen(usedPositions);
            usedPositions.Add(spawnPosition);

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
            GameObject fruitObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

            FruitClick fruitClick = fruitObject.GetComponent<FruitClick>();

            if (fruitClick != null)
            {
                activeFruits.Add(fruitClick);
            }
        }
    }

    private void ClearMissingReferences()
    {
        activeFruits.RemoveAll(fruit => fruit == null);
    }

    private Vector3 GetRandomPositionInsideScreen(List<Vector3> usedPositions)
    {
        if (mainCamera == null)
        {
            return Vector3.zero;
        }

        float distanceFromCamera = 0f - mainCamera.transform.position.z;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceFromCamera));

        float padding = 1f;

        float minX = bottomLeft.x + padding;
        float maxX = topRight.x - padding;
        float minY = bottomLeft.y + padding;
        float maxY = topRight.y - padding;

        for (int attempt = 0; attempt < 30; attempt++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            Vector3 candidate = new Vector3(randomX, randomY, 0f);

            bool tooClose = false;

            for (int i = 0; i < usedPositions.Count; i++)
            {
                if (Vector3.Distance(candidate, usedPositions[i]) < minDistanceBetweenFruits)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                return candidate;
            }
        }

        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
    }
}