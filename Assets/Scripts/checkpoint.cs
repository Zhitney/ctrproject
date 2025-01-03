using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public int checkpointIndex; // Index of this checkpoint in the list
    private LapManager lapManager;

    private void Start()
    {
        lapManager = FindObjectOfType<LapManager>(); // Find the LapManager in the scene
        if (lapManager == null)
        {
            Debug.LogError("LapManager not found in the scene. Ensure LapManager script is attached to a GameObject.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has either "Player" or "Enemy" tag
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && lapManager != null)
        {
            GameObject car = other.gameObject; // Get the car GameObject
            lapManager.CheckpointReached(car, checkpointIndex); // Pass the car and checkpoint index
            Debug.Log($"Car {car.name} passed checkpoint {checkpointIndex}");
        }
    }
}
