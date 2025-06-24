using UnityEngine;
using System.Collections; // Required for using Coroutines
using System.Collections.Generic;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("A list of different AI Car prefabs to choose from when spawning.")]
    public GameObject[] aiCarPrefabs; // Changed to an array for variety

    [Tooltip("A list of WaypointNodes where cars can be spawned.")]
    public List<WaypointNode> spawnPoints;

    [Header("Settings")]
    [Tooltip("The total number of AI cars to spawn in the level.")]
    public int numberOfCarsToSpawn = 5;

    [Tooltip("The delay before the first car spawns after the level loads.")]
    public float initialSpawnDelay = 1.0f;

    [Tooltip("The time delay between each car spawn.")]
    public float spawnInterval = 3.0f;

    void Start()
    {
        // Instead of spawning cars immediately, we start our timed routine.
        StartCoroutine(SpawnCarsRoutine());
    }

    // A Coroutine allows us to pause execution over a period of time.
    IEnumerator SpawnCarsRoutine()
    {
        // 1. Wait for an initial delay before anything happens.
        yield return new WaitForSeconds(initialSpawnDelay);

        for (int i = 0; i < numberOfCarsToSpawn; i++)
        {
            // Check if we have prefabs and spawn points assigned to prevent errors.
            if (aiCarPrefabs.Length == 0 || spawnPoints.Count == 0)
            {
                Debug.LogError("AI Car Prefabs or Spawn Points are not assigned in the Traffic Spawner!");
                yield break; // Stop the coroutine
            }

            // 2. Pick a random car model from our array.
            GameObject carToSpawn = aiCarPrefabs[Random.Range(0, aiCarPrefabs.Length)];

            // 3. Pick a random spawn point from the list.
            WaypointNode spawnNode = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // 4. Instantiate the chosen car at the chosen spawn point.
            GameObject newCar = Instantiate(carToSpawn, spawnNode.transform.position, spawnNode.transform.rotation);
            newCar.transform.SetParent(this.transform);

            // 5. Tell the new car where to go first.
            AICarController aICarController = newCar.GetComponent<AICarController>();
            if (aICarController != null)
            {
                aICarController.startingNode = spawnNode;
            }

            // 6. Wait for the specified interval before spawning the next car.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}