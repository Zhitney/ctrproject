using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    public GameObject[] carPrefabs; // Prefabs of cars
    public Transform carSpawnPoint; // Spawn point for the cars
    public Button next;
    public Button prev;
    private GameObject currentCar; // Reference to the currently active car
    private int index;

    void Start()
    {
        // Reset index to 0 (or a default value)
        index = 0;
        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();

        // Spawn the first car
        SpawnCar(index);
    }

    void Update()
    {
        // Update button interactability
        next.interactable = index < carPrefabs.Length - 1;
        prev.interactable = index > 0;
    }

    public void Next()
    {
        if (index < carPrefabs.Length - 1)
        {
            index++;
            ChangeCar();
        }
    }

    public void Prev()
    {
        if (index > 0)
        {
            index--;
            ChangeCar();
        }
    }

    private void ChangeCar()
    {
        // Destroy the current car
        if (currentCar != null)
        {
            Destroy(currentCar);
        }

        // Spawn the new car
        SpawnCar(index);

        // Save the current selection
        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
    }

    private void SpawnCar(int carIndex)
{
    if (carIndex >= 0 && carIndex < carPrefabs.Length)
    {
        // Instantiate the car
        currentCar = Instantiate(carPrefabs[carIndex], carSpawnPoint.position, carSpawnPoint.rotation);

        // Set "pijakan" as the parent
        GameObject pijakan = GameObject.Find("pijakan"); // Find the "pijakan" GameObject in the scene
        if (pijakan != null)
        {
            currentCar.transform.SetParent(pijakan.transform);
        }
    }
}

    public void selectedCar()
    {
        GameManager.Instance.selectedCar = carPrefabs[index].name;
        SceneManager.LoadScene(GameManager.Instance.selectedStage);
    }
}
