using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform spawnPoint; // Assign a spawn point in the scene
    public GameObject[] cars; // Assign all car prefabs here
    public GameObject[] enemyCars; // Assign all enemy car prefabs here
    public Transform[] enemySpawnPoints; // Assign all enemy spawn points here
    public Transform[] EnemyPaths; // Each Transform is a "folder" of waypoints

    private void Start()
    {
        // Spawn the selected player's car
        string selectedCar = GameManager.Instance.selectedCar;

        foreach (GameObject car in cars)
        {
            if (car.name == selectedCar)
            {
                GameObject spawnedCar = Instantiate(car, spawnPoint.position, spawnPoint.rotation);
                spawnedCar.tag = "Player"; // Tag the spawned car as Player
                break;
            }
        }

        // Spawn enemy cars
        SpawnEnemyCars();
    }

    void SpawnEnemyCars()
    {
        List<GameObject> availableEnemyCars = new List<GameObject>(enemyCars);

        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            if (availableEnemyCars.Count == 0)
            {
                Debug.LogWarning("No more unique enemy cars available to spawn!");
                break;
            }

            // Get the spawn point
            Transform spawnPoint = enemySpawnPoints[i];

            // Get a random car that is not the player's car
            GameObject randomCar;
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, availableEnemyCars.Count);
                randomCar = availableEnemyCars[randomIndex];
            }
            while (randomCar.name == GameManager.Instance.selectedCar);

            // Instantiate the enemy car
            GameObject spawnedCar = Instantiate(randomCar, spawnPoint.position, spawnPoint.rotation);

            // Ensure it has a BotKartController component
            BotKartController botController = spawnedCar.GetComponent<BotKartController>();
            if (botController == null)
            {
                botController = spawnedCar.AddComponent<BotKartController>();
            }

            // Assign the paths to the BotKartController
            if (i < EnemyPaths.Length)
            {
                Transform pathParent = EnemyPaths[i];
                Transform[] waypoints = GetChildTransforms(pathParent);
                botController.SetWaypoints(waypoints);
            }
            else
            {
                Debug.LogWarning("Not enough paths available for all enemies!");
            }

            // Add a Rigidbody to ensure physics interaction
            Rigidbody rb = spawnedCar.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = spawnedCar.AddComponent<Rigidbody>();
                rb.useGravity = true; // Optional, depending on your needs
            }

            // Remove the spawned car from the available pool
            availableEnemyCars.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// Retrieves all child transforms of a parent Transform.
    /// </summary>
    /// <param name="parent">The parent Transform.</param>
    /// <returns>An array of child Transforms.</returns>
    private Transform[] GetChildTransforms(Transform parent)
    {
        int childCount = parent.childCount;
        Transform[] childTransforms = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            childTransforms[i] = parent.GetChild(i);
        }

        return childTransforms;
    }
}
