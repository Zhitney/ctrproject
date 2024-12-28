using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxleInfoBasic
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // Determines if the axle is powered
    public bool steering; // Determines if the axle steers
}

public class CarControl : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // List of axle configurations
    public float maxMotorTorque = 500f; // Maximum motor torque
    public float maxSteeringAngle = 30f; // Maximum steering angle
    public float powerBrake = 100000f; // Braking force
    public float maxZRotation = 30f; // Maximum rotation on the Z-axis

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Input handling for motor and steering
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        // Apply brakes when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBrakeTorque(powerBrake);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SetBrakeTorque(0);
        }
    }

    void FixedUpdate()
    {
        // Clamp the car's Z-axis rotation
        Quaternion currentRotation = rb.rotation;
        Vector3 euler = currentRotation.eulerAngles;

        if (euler.z > 180)
        {
            euler.z -= 360;
        }

        euler.z = Mathf.Clamp(euler.z, -maxZRotation, maxZRotation);
        rb.rotation = Quaternion.Euler(euler);
    }

    // Set brake torque for all wheels
    private void SetBrakeTorque(float torque)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.brakeTorque = torque;
            axleInfo.rightWheel.brakeTorque = torque;
        }
    }

    // Update the visual representation of the wheels
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
    {
        Debug.LogError($"{collider.name} has no child for the visual wheel!");
        return;
    }

        Transform visualWheel = collider.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
