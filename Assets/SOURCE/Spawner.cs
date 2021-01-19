using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawnableObjects;

    public float spawnDelay = 2.5f;
    float nextSpawnTime;

    List<GameObject> spawnedObjects = new List<GameObject>();
    public static Spawner instance;

    bool canSpawn;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);

        instance = this;

        GameEvents.gameStart += StartSpawning;
    }

    void OnDestroy()
    {
        GameEvents.gameStart -= StartSpawning;
    }
    
    void StartSpawning()
    {
        canSpawn = true;
    }

    void Update()
    {

        if(canSpawn == false)
        {
            UpdateNextSpawnTime();
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            if (SpawnRandom())
                UpdateNextSpawnTime();
        }

    }

    void UpdateNextSpawnTime()
    {
        nextSpawnTime = Time.time + spawnDelay;
    }
    bool SpawnRandom()
    {
        // Offset the returned position up, so it doesn't spawn the object near the ground
        Vector3 spawnPosition = Level.GetRandomPosition() + Vector3.up * 1.05f;

        if (Physics.CheckSphere(spawnPosition, 1))
            return false;

        GameObject objectToSpawn = GetRandomSpawnableObject();
        Spawn(objectToSpawn, spawnPosition);

        return true;
    }

    GameObject Spawn(GameObject objectToSpawn, Vector3 position)
    {
        GameObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);
        spawnedObjects.Add(spawnedObject);
        return spawnedObject;
    }

    private GameObject GetRandomSpawnableObject()
    {
        if (spawnableObjects.Length <= 0)
            return null;

        int spawnIndex = Random.Range(0, spawnableObjects.Length);
        return spawnableObjects[spawnIndex];
    }

    public GameObject GetRandomSpawnedObject()
    {
        if (spawnedObjects.Count <= 0)
            return null;

        int spawnIndex = Random.Range(0, spawnedObjects.Count);
        return spawnedObjects[spawnIndex];
    }

    public void DestroySpawnedObject(GameObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            spawnedObjects.Remove(obj);
            Destroy(obj.gameObject);
        }
    }

}
