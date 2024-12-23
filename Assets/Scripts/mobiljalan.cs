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

    public float saveInterval = 0.1f;       // Interval to save state (seconds)
    public float rewindDuration = 5f;      // How far back to rewind (seconds)
    private Queue<MobilState> stateHistory = new Queue<MobilState>();
    private float lastRewindTime = 0f;      // Track the last time rewind occurred
    public float rewindCooldown = 1f; 


    // Struct to store state
    private struct MobilState
    {
        public Vector3 position;
        public Quaternion rotation;
        public float speed;
        public float time;

        public MobilState(Vector3 pos, Quaternion rot, float spd, float t)
        {
            position = pos;
            rotation = rot;
            speed = spd;
            time = t;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        InvokeRepeating(nameof(SaveState), 0f, saveInterval);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        HandleBraking();
        RotateWheels();
        UpdateSteering();
        UpdateSmoke();
        if (Input.GetKeyDown(KeyCode.R) && Time.time >= lastRewindTime + rewindCooldown)
        {
            Rewind();
        }
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

    private void SaveState()
    {
        // Save current state with the current time
        stateHistory.Enqueue(new MobilState(transform.position, transform.rotation, currentSpeed, Time.time));

        // Remove states older than rewindDuration
        while (stateHistory.Count > 0 && Time.time - stateHistory.Peek().time > rewindDuration)
        {
            stateHistory.Dequeue();
        }
    }

    public void Rewind()
    {
        if (stateHistory.Count > 0)
        {
            // Get the most recent valid state
            MobilState state = stateHistory.Dequeue();
            transform.position = state.position;
            transform.rotation = state.rotation;
            currentSpeed = state.speed;

            // Stop the Rigidbody's velocity
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Update the last rewind time
            lastRewindTime = Time.time;

            Debug.Log("Rewinded to: " + state.time);
        }
        else
        {
            Debug.Log("No valid state to rewind to.");
        }
    }


