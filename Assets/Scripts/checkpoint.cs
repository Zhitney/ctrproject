using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointIndex; // Index checkpoint ini dalam daftar checkpoints
    public LapManager lapManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "raceCarGreen")
        {
            lapManager.CheckpointReached(checkpointIndex);
        }
    }
}
