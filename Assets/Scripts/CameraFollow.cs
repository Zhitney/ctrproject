using System.Collections;
using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target object that the camera will follow
    public Transform target;

    // Offset from the target's position
    public Vector3 offset = new Vector3(0, 4, -12);

    // How smooth the camera follows the target
    public float smoothSpeed = 0.125f;
    public float rotationSpeed = 5f;

    // Velocity for SmoothDamp
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // Start a coroutine to add a delay before finding the target
        StartCoroutine(InitializeTargetWithDelay(0.5f)); // Adjust delay time as needed
    }

    private IEnumerator InitializeTargetWithDelay(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Find the target with a specific tag
        if (target == null)
        {
            GameObject targetObject = GameObject.FindWithTag("Player");
            if (targetObject != null)
            {
                target = targetObject.transform;

                // Set initial position and orientation
                transform.position = target.TransformPoint(offset);
                transform.LookAt(target);
            }
            else
            {
                Debug.LogError("Target not found after delay. Please assign a valid target or check the tag.");
                enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position of the camera relative to the car's orientation
        Vector3 desiredPosition = target.TransformPoint(offset);

        // Smoothly move to the desired position using SmoothDamp
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;

        // Smoothly rotate the camera to look at the car
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
