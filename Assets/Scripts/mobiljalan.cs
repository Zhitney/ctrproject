using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobiljalan : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    public float acceleration = 10f;
    public float deceleration = 5f;
    public float maxSpeed = 15f;
    public float brakingForce = 20f;
    private float currentSpeed = 0f;
    private Rigidbody rb;

    public Transform GreenFrontLeft;
    public Transform GreenFrontRight;
    public Transform GreenBackLeft;
    public Transform GreenBackRight;

    public float wheelRotationSpeed = 360f;
    public float maxSteeringAngle = 30f;

    public ParticleSystem smokeParticleSystem; // Referensi untuk partikel asap

    private Renderer[] allRenderers; // Array untuk semua komponen Renderer pada mobil

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ambil semua komponen Renderer di dalam mobil (termasuk anak objek)
        allRenderers = GetComponentsInChildren<Renderer>();

        // Awalnya, buat mobil dan rodanya tak terlihat
        SetVisibility(false);

        // Mulai efek glitch
        StartCoroutine(GlitchEffect());
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

    private void HandleMovement()
    {
        float moveInput = -Input.GetAxis("Vertical");

        if (moveInput != 0)
        {
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
        Vector3 moveDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    private void HandleBraking()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakingForce * Time.fixedDeltaTime);
            rb.velocity = Vector3.zero;
        }
    }

    private void RotateWheels()
    {
        float wheelCircumference = 2 * Mathf.PI * 0.5f;
        float distanceMoved = currentSpeed * Time.fixedDeltaTime;
        float rotationAngle = (distanceMoved / wheelCircumference) * 360f;

        GreenFrontLeft.Rotate(Vector3.right, rotationAngle);
        GreenFrontRight.Rotate(Vector3.right, rotationAngle);
        GreenBackLeft.Rotate(Vector3.right, rotationAngle);
        GreenBackRight.Rotate(Vector3.right, rotationAngle);
    }

    private void UpdateSteering()
    {
        float turnInput = Input.GetAxis("Horizontal");
        float steeringAngle = maxSteeringAngle * turnInput;

        Quaternion steeringRotation = Quaternion.Euler(0f, steeringAngle, 0f);

        GreenFrontLeft.localRotation = steeringRotation * Quaternion.Euler(GreenFrontLeft.localRotation.eulerAngles.x, 0f, 0f);
        GreenFrontRight.localRotation = steeringRotation * Quaternion.Euler(GreenFrontRight.localRotation.eulerAngles.x, 0f, 0f);
    }

    private void HandleSteering()
    {
        if (currentSpeed != 0)
        {
            float turnInput = Input.GetAxis("Horizontal");
            float speedFactor = Mathf.Clamp(currentSpeed / maxSpeed, 0.1f, 1f);
            float steeringAngle = maxSteeringAngle * turnInput * speedFactor;

            float turn = steeringAngle * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void UpdateSmoke()
    {
        float moveInput = Mathf.Abs(Input.GetAxis("Vertical"));
        float turnInput = Mathf.Abs(Input.GetAxis("Horizontal"));
        bool isTurningAndMoving = moveInput > 0 && turnInput > 0.1f;

        var emission = smokeParticleSystem.emission;
        emission.enabled = isTurningAndMoving;
    }

    // Fungsi untuk mengatur visibilitas mobil dan rodanya
    private void SetVisibility(bool isVisible)
    {
        // Atur visibilitas komponen mobil
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer.gameObject.name != "SmokeCar") // Jangan ubah visibilitas partikel
            {
                renderer.enabled = isVisible;
            }
        }
    }


    // Coroutine untuk efek glitch
    private IEnumerator GlitchEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            for (int i = 0; i < 5; i++)
            {
                SetVisibility(true);
                yield return new WaitForSeconds(0.1f);
                SetVisibility(false);
                yield return new WaitForSeconds(0.1f);
            }

            SetVisibility(false);
        }
    }
}
