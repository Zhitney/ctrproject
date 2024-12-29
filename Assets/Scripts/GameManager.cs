using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string selectedCar; // Store the name or ID of the selected car
    public string selectedStage; // Store the name or ID of the selected stage

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BeginGame()  {
        
    }


}