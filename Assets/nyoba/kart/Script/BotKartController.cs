using UnityEngine;
using System.Collections.Generic;

public class BotKartController : MonoBehaviour
{
    public float maxSpeed = 10f; // Maximum speed of the kart
    public float acceleration = 5f; // Acceleration of the kart
    public float steeringSpeed = 100f; // How quickly the kart can turn
    public float waypointThreshold = 10f; // Distance threshold to reach a waypoint

    public List<Transform> waypoints = new List<Transform>(); // Waypoints assigned at runtime
    private int currentWaypointIndex = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Calculate direction to the target waypoint
        Vector3 directionToWaypoint = (targetWaypoint.position - transform.position).normalized;

        // Calculate the angle between the kart's forward direction and the waypoint direction
        float angleToWaypoint = Vector3.SignedAngle(transform.forward, directionToWaypoint, Vector3.up);

        // Determine steering based on the angle
        float turn = Mathf.Clamp(angleToWaypoint / steeringSpeed, -1f, 1f);
        TurnKart(turn);

        // Move forward
        MoveKart();
        Debug.Log("Distance to waypoint" + Vector3.Distance(transform.position, targetWaypoint.position));
        Debug.Log("Current Waypoint" + currentWaypointIndex);
        // Check if the kart is close enough to the current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count; // Loop back to the first waypoint
        }
    }

    private void MoveKart()
    {
        // Accelerate the kart forward, limited by maxSpeed
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }
    }

    private void TurnKart(float turnInput)
    {
        // Rotate the kart based on the turn input
        float turnAngle = turnInput * steeringSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    /// <summary>
    /// Sets the waypoints for the bot to follow dynamically.
    /// </summary>
    /// <param name="path">An array of transforms representing the path.</param>
    public void SetWaypoints(Transform[] path)
    {
        waypoints.Clear();
        waypoints.AddRange(path);
        currentWaypointIndex = 0; // Reset to the first waypoint
    }
}
