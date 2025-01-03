using System.Collections;
using System.Collections.Generic;
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
        // Find the target with a specific tag
        if (target == null)
        {
            GameObject targetObject = GameObject.FindWithTag("Player");
            if (targetObject != null)
            {
                target = targetObject.transform;
            }
            else
            {
                Debug.LogError("Target not found. Please assign a valid target or check the tag.");
                enabled = false;
                return;
            }
        }

        // Set initial position and orientation
        transform.position = target.TransformPoint(offset);
        transform.LookAt(target);
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
