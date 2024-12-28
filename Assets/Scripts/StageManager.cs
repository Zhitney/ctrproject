using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform spawnPoint; // Assign a spawn point in the scene
    public GameObject[] cars; // Assign all car prefabs here

    private void Start()
    {
        string selectedCar = GameManager.Instance.selectedCar;
        
        foreach (GameObject car in cars)
        {
            if (car.name == selectedCar)
            {
                Instantiate(car, spawnPoint.position, spawnPoint.rotation);
                break;
            }
        }
    }
}
