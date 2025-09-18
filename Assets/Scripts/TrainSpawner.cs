using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns trains on predefined lane positions. Some trains spawn sitting (stationary) for a duration,
/// others spawn already moving. This simulates Subway Surfers train behavior.
/// Attach to an empty GameObject and assign a Train prefab and lane Transforms.
/// </summary>
public class TrainSpawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject trainPrefab; // should contain Train.cs
    public Transform[] lanes; // lane positions (set Y and Z appropriately)
    public float spawnInterval = 2f;
    public int maxActiveTrains = 10;
    [Tooltip("Number of pooled instances to prewarm. If 0, no prewarm is performed.")]
    public int poolSize = 5;

    [Header("Sitting / Moving")]
    [Range(0f, 1f)]
    public float sittingProbability = 0.4f; // chance a spawned train is sitting
    public float minSit = 2f;
    public float maxSit = 6f;
    public float movingSpeed = 6f;
    public Vector3 movingDirection = Vector3.forward; // direction trains move when moving
    [Header("Multi-car trains (optional)")]
    [Tooltip("If set, this prefab will be instantiated as extra cars attached behind the main train.")]
    public GameObject carPrefab;
    [Tooltip("How many cars per train (including main). If 1 then only the trainPrefab is used.")]
    public int carsPerTrain = 1;
    public Vector3 carOffset = new Vector3(-1.5f, 0f, 0f);

    private float spawnTimer = 0f;
    private List<GameObject> active = new List<GameObject>();
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // Prewarm pool with basic trainPrefab instances
        if (poolSize > 0 && trainPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var go = Instantiate(trainPrefab);
                go.SetActive(false);
                pool.Enqueue(go);
            }
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawn();
        }

        // Cleanup trains that are far away to keep active count reasonable
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var go = active[i];
            if (go == null)
            {
                active.RemoveAt(i);
                continue;
            }
            // Simple distance-based despawn (tweak as needed)
            if (Vector3.Distance(transform.position, go.transform.position) > 100f)
            {
                Despawn(go);
                active.RemoveAt(i);
            }
        }
    }

    void TrySpawn()
    {
        if (trainPrefab == null || lanes == null || lanes.Length == 0) return;
        if (active.Count >= maxActiveTrains) return;

        // pick a random lane
        int laneIndex = Random.Range(0, lanes.Length);
        Transform lane = lanes[laneIndex];

        // Spawn from pool or instantiate
        GameObject go = null;
        if (pool.Count > 0)
        {
            go = pool.Dequeue();
            go.transform.SetParent(null);
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(trainPrefab);
        }

        // position and initialize
        // Make a container if we will add extra cars so we can treat the entire train as one unit
        GameObject container = go;
        if (carsPerTrain > 1 && carPrefab != null)
        {
            // create container parent
            container = new GameObject("TrainContainer");
            container.transform.position = lane.position;
            container.transform.rotation = lane.rotation;

            // attach main car to container
            go.transform.SetParent(container.transform, false);
            go.transform.localPosition = Vector3.zero;

            // spawn additional cars behind the main car
            Vector3 offset = carOffset;
            for (int c = 1; c < carsPerTrain; c++)
            {
                var car = Instantiate(carPrefab, container.transform);
                car.transform.localPosition = offset * c;
                car.transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            container.transform.position = lane.position;
            container.transform.rotation = lane.rotation;
        }

        Train t = go.GetComponent<Train>();
        if (t != null)
        {
            bool sitting = Random.value < sittingProbability;
            float sitDur = Random.Range(minSit, maxSit);
            t.Initialize(sitting, sitDur, movingSpeed, movingDirection);
        }

        active.Add(container);
    }

    void Despawn(GameObject go)
    {
        // If this is a container (children), try to return main car to pool and destroy others
        if (go == null) return;

        // If container has children and one of them matches a pooled prefab type, detach it and pool
        if (go.transform.childCount > 0)
        {
            // assume the first child is the main pooled object
            var main = go.transform.GetChild(0).gameObject;
            main.SetActive(false);
            main.transform.SetParent(null);
            pool.Enqueue(main);

            // destroy other children (could be pooled too if needed)
            for (int i = go.transform.childCount - 1; i >= 0; i--)
            {
                var child = go.transform.GetChild(i).gameObject;
                if (child != main)
                    Destroy(child);
            }

            Destroy(go);
        }
        else
        {
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    // Editor helper to prefill lanes with children
    [ContextMenu("Auto Fill Lanes From Children")]
    void AutoFillLanes()
    {
        var children = new List<Transform>();
        foreach (Transform t in transform)
            children.Add(t);

        lanes = children.ToArray();
    }

    void OnDrawGizmos()
    {
        if (lanes == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < lanes.Length; i++)
        {
            var l = lanes[i];
            if (l == null) continue;
            Gizmos.DrawSphere(l.position, 0.25f);
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(l.position + Vector3.up * 0.25f, $"Lane {i}");
            #endif
        }
    }

    // Simple in-game debug UI for spawn and pool control
    void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 220, 200), "Train Spawner", GUI.skin.window);
        GUILayout.Label($"Active: {active.Count}");
        GUILayout.Label($"Pool: {pool.Count}");
        if (GUILayout.Button("Spawn Now"))
        {
            TrySpawn();
        }
        if (GUILayout.Button("Despawn All"))
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                Despawn(active[i]);
            }
            active.Clear();
        }
        if (GUILayout.Button("Prewarm Pool"))
        {
            // prewarm to poolSize
            if (trainPrefab != null)
            {
                int toCreate = Mathf.Max(0, poolSize - pool.Count);
                for (int i = 0; i < toCreate; i++)
                {
                    var go = Instantiate(trainPrefab);
                    go.SetActive(false);
                    pool.Enqueue(go);
                }
            }
        }
        GUILayout.EndArea();
    }
}
