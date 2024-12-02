using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobiljalan : MonoBehaviour
{
    public float moveSpeed = 10f;           // Max speed of the car
    public float turnSpeed = 100f;          // Speed of the car's rotation
    public float acceleration = 5f;         // How fast the car accelerates
    public float deceleration = 5f;         // How fast the car decelerates
    public float maxSpeed = 20f;            // Maximum speed the car can reach
    public float brakingForce = 20f; 
    private float currentSpeed = 0f;        // Current speed of the car
    private Rigidbody rb;                   // Reference to the Rigidbody component

    // References to the wheels
    public Transform GreenFrontLeft;
    public Transform GreenFrontRight;
    public Transform GreenBackLeft;
    public Transform GreenBackRight;
    public float wheelRotationSpeed = 360f; // Speed of wheel rotation

    public float maxSteeringAngle = 30f;    // Maximum steering angle for the front wheels

    // Reference to the smoke particle system
    public ParticleSystem smokeParticleSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        HandleBraking();
        RotateWheels();
        UpdateSteering();
        UpdateSmoke();
    }

    // Function to handle movement
    private void HandleMovement()
    {
        // Get input for forward/backward movement
        float moveInput = -Input.GetAxis("Vertical"); // W/S or Up/Down arrows (W = 1, S = -1)

        // Accelerate or decelerate based on input
        if (moveInput != 0)
        {
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            // Gradually slow down the car when no input is given
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        // Clamp the current speed to max speed limits
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Move the car forward/backward
        Vector3 moveDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    private void HandleBraking()
    {
        // Check if the space key is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            // Apply braking force
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakingForce * Time.fixedDeltaTime);

            // Optional: Reduce velocity directly for an instant stop
            rb.velocity = Vector3.zero;
        }
    }
    // Function to rotate the wheels based on speed
    private void RotateWheels()
    {
        // Calculate the rotation for the wheels based on current speed
        float wheelCircumference = 2 * Mathf.PI * 0.5f; // Approximation of the wheel circumference (assuming radius is around 0.5 units)
        float distanceMoved = currentSpeed * Time.fixedDeltaTime; // How far the car has moved in this frame
        float rotationAngle = (distanceMoved / wheelCircumference) * 360f; // Convert distance to rotation in degrees

        // Rotate each wheel around its local X axis (as if it's rolling forward/backward)
        GreenFrontLeft.Rotate(Vector3.right, rotationAngle);
        GreenFrontRight.Rotate(Vector3.right, rotationAngle);
        GreenBackLeft.Rotate(Vector3.right, rotationAngle);
        GreenBackRight.Rotate(Vector3.right, rotationAngle);
    }

    // Function to update steering of front wheels
    private void UpdateSteering()
    {
        float turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows (A = -1, D = 1)
        float steeringAngle = maxSteeringAngle * turnInput;

        // Adjust only Y-axis rotation for the front wheels (left and right turn)
        Quaternion steeringRotation = Quaternion.Euler(0f, steeringAngle, 0f);

        // Combine the steering rotation with the existing rotation on the X-axis
        GreenFrontLeft.localRotation = steeringRotation * Quaternion.Euler(GreenFrontLeft.localRotation.eulerAngles.x, 0f, 0f);
        GreenFrontRight.localRotation = steeringRotation * Quaternion.Euler(GreenFrontRight.localRotation.eulerAngles.x, 0f, 0f);
    }

    private void HandleSteering()
    {
        if (currentSpeed != 0)
        {
            // Calculate the steering angle and scale it with the current speed
            float turnInput = Input.GetAxis("Horizontal");

            // Scale turning based on the current speed
            float speedFactor = Mathf.Clamp(currentSpeed / maxSpeed, 0.1f, 1f); // Prevent zero turn speed
            float steeringAngle = maxSteeringAngle * turnInput * speedFactor;

            // Apply turning to the car
            float turn = steeringAngle * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void UpdateSmoke()
    {
        // Check if the car is moving and turning
        float moveInput = Mathf.Abs(Input.GetAxis("Vertical"));
        float turnInput = Mathf.Abs(Input.GetAxis("Horizontal"));
        bool isTurningAndMoving = moveInput > 0 && turnInput > 0.1f;

        // Enable or disable smoke particles based on turning and moving status
        var emission = smokeParticleSystem.emission;
        emission.enabled = isTurningAndMoving;
    }
}
