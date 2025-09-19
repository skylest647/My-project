using UnityEngine;
using System.Collections.Generic;

public class TrainSpawner : MonoBehaviour
{
    
    public GameObject trainPrefab;      
    public float spawnY = 10f;          
    public float moveSpeed = 5f;        
    public float laneDistance = 2f;     

    private List<GameObject> trains = new List<GameObject>();
    private int maxTrains = 2;

    private float spawnTimer = 0f;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;
    private float nextSpawnTime = 0f;

    private int lastLane = -1; 

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        // Move trains downward
        for (int i = trains.Count - 1; i >= 0; i--)
        {
            GameObject train = trains[i];
            train.transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            if (train.transform.position.y < -10f)
            {
                Destroy(train);
                trains.RemoveAt(i);
            }
        }

        // Spawn new train if under max and timer reached
        if (trains.Count < maxTrains)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= nextSpawnTime)
            {
                SpawnTrain();
                spawnTimer = 0f;
                SetNextSpawnTime();
            }
        }
    }

    void SpawnTrain()
    {
        // Random lane, avoid same as last
        int lane;
        do
        {
            lane = Random.Range(0, 3);
        } while (lane == lastLane && trains.Count > 0);

        lastLane = lane;

        float spawnX = 0f;
        if (lane == 0) spawnX = -laneDistance;
        else if (lane == 2) spawnX = laneDistance;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        GameObject newTrain = Instantiate(trainPrefab, spawnPos, Quaternion.identity);

        trains.Add(newTrain);
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}
