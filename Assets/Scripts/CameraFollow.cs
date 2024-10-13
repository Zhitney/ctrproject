using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The target object that the camera will follow
    public Transform target;
    
    // Offset from the target's position
    public Vector3 offset;

    // How smooth the camera follows the target
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position is target's position plus the offset
        Vector3 desiredPosition = target.position + offset;

        // Smooth the camera movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;

        // Optionally, if you want the camera to always look at the target, uncomment the next line
        // transform.LookAt(target);
    }
}