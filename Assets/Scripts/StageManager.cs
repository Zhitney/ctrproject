using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform spawnPoint; // Assign a spawn point in the scene
    public GameObject[] cars; // Assign all car prefabs here
    public GameObject[] enemyCars; // Assign all enemy car prefabs here
    public Transform[] enemySpawnPoints; // Assign all enemy spawn points here

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
    // Create a list to track available enemy cars
    List<GameObject> availableEnemyCars = new List<GameObject>(enemyCars);

    // Loop through each spawn point
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            if (availableEnemyCars.Count == 0)
            {
                Debug.LogWarning("No more unique enemy cars available to spawn!");
                break;
            }

            // Select a random car that is not the selected player's car
            GameObject randomCar;
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, availableEnemyCars.Count);
                randomCar = availableEnemyCars[randomIndex];
            }
            while (randomCar.name == GameManager.Instance.selectedCar);

            // Instantiate the selected enemy car
            Instantiate(randomCar, spawnPoint.position, spawnPoint.rotation);

            // Remove the spawned car from the list to avoid duplication
            availableEnemyCars.RemoveAt(randomIndex);
        }
    }
}
