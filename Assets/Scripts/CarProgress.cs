// public class CarProgress : MonoBehaviour
// {
//     public bool isPlayer = false; // Whether this car is controlled by the player
//     public int currentLap = 0;    // Current lap number
//     public int checkpointsPassed = 0; // Number of checkpoints passed
//     public float distanceToNextCheckpoint = 0f; // Distance to the next checkpoint
//     public float lapStartTime;   // Lap start time
//     public bool hasFinished = false; // Whether the car has finished the race

//     private Transform nextCheckpoint; // The next checkpoint to reach
//     private Transform startFinishLine; // Start/Finish line position
//     private List<Transform> checkpoints; // List of all checkpoints
//     private int totalCheckpoints; // Total number of checkpoints

//     public void Initialize(List<Transform> checkpointList, Transform startFinish)
//     {
//         checkpoints = checkpointList;
//         startFinishLine = startFinish;
//         totalCheckpoints = checkpoints.Count;
//         nextCheckpoint = checkpoints[0]; // Set the first checkpoint as the target
//         lapStartTime = Time.time; // Initialize lap start time
//     }

//     private void Update()
//     {
//         if (!hasFinished)
//         {
//             UpdateDistanceToNextCheckpoint();
//         }
//     }

//     private void UpdateDistanceToNextCheckpoint()
//     {
//         if (nextCheckpoint != null)
//         {
//             distanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
//         }
//         else
//         {
//             distanceToNextCheckpoint = float.MaxValue; // Default large distance if no checkpoint is set
//         }
//     }

//     public void CheckpointReached(int checkpointIndex)
//     {
//         if (hasFinished) return;

//         if (checkpointIndex == checkpointsPassed)
//         {
//             checkpointsPassed++;

//             if (checkpointsPassed == totalCheckpoints)
//             {
//                 checkpointsPassed = 0;
//                 currentLap++;

//                 if (currentLap >= LapManager.Instance.totalLaps)
//                 {
//                     hasFinished = true;
//                     Debug.Log($"{gameObject.name} has finished the race!");
//                     return;
//                 }

//                 Debug.Log($"{gameObject.name} completed Lap {currentLap}.");
//             }

//             nextCheckpoint = checkpoints[checkpointsPassed % totalCheckpoints];
//         }
//     }
// }