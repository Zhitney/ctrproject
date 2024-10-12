using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobiljalan : MonoBehaviour
{
     public float moveSpeed = 10f;       // Speed of the car's forward movement
    public float turnSpeed = 100f;      // Speed of the car's rotation
    private Rigidbody rb;               // Reference to the Rigidbody component
    
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
        Debug.Log(moveInput);
        
        // Get input for turning left/right
        float turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows (A = -1, D = 1)
        
        // Move the car forward/backward
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Turn the car left/right
        float turn = turnInput * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
