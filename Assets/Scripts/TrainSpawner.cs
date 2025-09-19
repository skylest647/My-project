using UnityEngine;
using System.Collections.Generic;

public class TrainSpawner : MonoBehaviour
{
    public GameObject trainPrefab;     
    public float spawnY = 10f;      
    public float laneDistance = 2f;   
    public int maxTrains = 2;         
    public float minSpawnDelay = 1f;   
    public float maxSpawnDelay = 3f;   
    public float moveSpeed = 5f;        

    private List<GameObject> trains = new List<GameObject>();
    private float spawnTimer = 0f;
    private float nextSpawnTime = 0f;
    private int lastLane = -1;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        // Spawn new train if under maxTrains and timer reached
        if (trains.Count < maxTrains && spawnTimer >= nextSpawnTime)
        {
            SpawnTrain();
            spawnTimer = 0f;
            SetNextSpawnTime();
        }

        // Remove trains that have moved off-screen
        for (int i = trains.Count - 1; i >= 0; i--)
        {
            if (trains[i].transform.position.y < -10f)
            {
                Destroy(trains[i]);
                trains.RemoveAt(i);
            }
        }
    }

    void SpawnTrain()
    {
        // Choose random lane, avoid same lane as last
        int lane;
        do
        {
            lane = Random.Range(0, 3); // 0,1,2
        } while (lane == lastLane && trains.Count > 0);

        lastLane = lane;

        float spawnX = 0f;
        if (lane == 0) spawnX = -laneDistance;
        else if (lane == 2) spawnX = laneDistance;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        GameObject newTrain = Instantiate(trainPrefab, spawnPos, Quaternion.identity);

        // Ensure train has Rigidbody2D
        Rigidbody2D rb = newTrain.GetComponent<Rigidbody2D>();
        if (rb == null) rb = newTrain.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Ensure train has BoxCollider2D with trigger enabled
        BoxCollider2D col = newTrain.GetComponent<BoxCollider2D>();
        if (col == null) col = newTrain.AddComponent<BoxCollider2D>();
        col.isTrigger = true;  // <-- Trigger so player can pass through

        // Add TrainMovement if missing
        if (newTrain.GetComponent<TrainMovement>() == null)
        {
            TrainMovement tm = newTrain.AddComponent<TrainMovement>();
            tm.moveSpeed = moveSpeed;
        }

        trains.Add(newTrain);
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
}
