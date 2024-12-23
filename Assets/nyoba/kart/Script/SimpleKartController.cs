using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}



     
public class SimpleKartController : MonoBehaviour {
    public List<AxleInfo> axleInfos; 
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float topSpeed = 30;

    public GameObject smoke;
    public GameObject turbo;
    public Transform jumpAnchor;

    Rigidbody rb;

    public float saveInterval = 0.1f;       // Interval to save state (seconds)
    public float rewindDuration = 5f;      // How far back to rewind (seconds)
    private Queue<MobilState> stateHistory = new Queue<MobilState>();
    private float lastRewindTime = 0f;      // Track the last time rewind occurred
    public float rewindCooldown = 1f; 
    private float currentSpeed = 0f;        // Current speed of the car
     
    void Start(){
        rb = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(SaveState), 0f, saveInterval);
    }

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

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void Update(){
        if( Mathf.Round(rb.velocity.magnitude) > topSpeed ){
            smoke.SetActive(false);
            turbo.SetActive(true);
        }else{
            smoke.SetActive(true);
            turbo.SetActive(false);
        }
    }
     
    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
     
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
        if (Input.GetKeyDown(KeyCode.R) && Time.time >= lastRewindTime + rewindCooldown)
        {
            Rewind();
        }
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
}
