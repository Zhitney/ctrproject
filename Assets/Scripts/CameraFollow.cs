using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The target object that the camera will follow
    public Transform target;
    
    // Offset from the target's position
    public Vector3 offset = new Vector3(0, 2, -7);

    // How smooth the camera follows the target
    public float smoothSpeed = 0.125f;
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position of the camera relative to the car's orientation
        Vector3 desiredPosition = target.TransformPoint(offset);

        // Smoothly interpolate the camera's position for smoother following
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's positionn
        transform.position = smoothedPosition;

        // Smoothly rotate the camera to look at the car
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    
    }
}