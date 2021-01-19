using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Snake[] enemyPrefabs;
    public float delayBetweenSpawns = 10f;
    public int maxNumberOfActiveSnakes = 2;

    float nextSpawnTime;
    int numberOfActiveSnakes;

    public Blink blinkPrefab;

    bool canSpawn;

    void Awake()
    {
        GameEvents.snakeDeath += DecreaseActiveSnakes;
        GameEvents.gameStart += StartSpawning;

        numberOfActiveSnakes = FindObjectsOfType<EnemySnake>().Length;
        UpdateNextSpawnTime();
    }

    void OnDestroy()
    {
        GameEvents.snakeDeath -= DecreaseActiveSnakes;
        GameEvents.gameStart -= StartSpawning;
    }

    public void StartSpawning()
    {
        canSpawn = true;
    }

    void Update()
    {
        if (canSpawn == false)
        {
            UpdateNextSpawnTime();
            return;
        }

        if (numberOfActiveSnakes >= maxNumberOfActiveSnakes)
        {
            UpdateNextSpawnTime();
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            UpdateNextSpawnTime();
            SpawnNewSnake();
        }
    }

    void UpdateNextSpawnTime()
    {
        nextSpawnTime = Time.time + delayBetweenSpawns;
    }

    void DecreaseActiveSnakes(Snake deadSnake)
    {
        if(deadSnake is EnemySnake)
            numberOfActiveSnakes -= 1;
    }

    void SpawnNewSnake()
    {
        Vector3 spawnPosition = Level.GetRandomPosition();

        Blink blinker = Instantiate(blinkPrefab, spawnPosition, Quaternion.identity);

        blinker.StarBlinking(OnFinish : () => {

            spawnPosition.y = spawnPosition.y + 1f;
            Snake newSnake = Instantiate(RandomPrefab(), spawnPosition, Quaternion.identity);
            numberOfActiveSnakes += 1;

            Destroy(blinker.gameObject);
        });

    }

    Snake RandomPrefab()
    {
        int randIndex = Random.Range(0, enemyPrefabs.Length);
        return enemyPrefabs[randIndex];

    }

}
