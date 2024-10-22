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
    private float currentSpeed = 0f;        // Current speed of the car
    private Rigidbody rb;                   // Reference to the Rigidbody component
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input for forward/backward movement
        float moveInput = -Input.GetAxis("Vertical"); // W/S or Up/Down arrows (W = 1, S = -1)
        
        // Accelerate or decelerate based on input
        if (moveInput != 0)
        {
            currentSpeed += moveInput * acceleration * Time.deltaTime;
        }
        else
        {
            // Gradually slow down the car when no input is given
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Clamp the current speed to max speed limits
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Move the car forward/backward
        Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Get input for turning left/right
        float turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows (A = -1, D = 1)
        
        // Turn the car left/right
        float turn = turnInput * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
