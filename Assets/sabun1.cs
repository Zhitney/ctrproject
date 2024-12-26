using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sabun1 : MonoBehaviour
{
    public float motorForce = 500f;         // Gaya dorong motor
    public float steerForce = 20f;         // Gaya kemudi
    public float brakingForce = 50f;       // Gaya pengereman bertahap
    public float acceleration = 10f;       // Akselerasi mobil
    public float deceleration = 5f;        // Deselerasi mobil
    public float maxSpeed = 40f;           // Kecepatan maksimum mobil

    private float currentSpeed = 0f;       // Kecepatan saat ini
    private float maxSteeringAngle = 30f;  // Sudut maksimum kemudi

    public WheelCollider roda1;            // Collider roda depan kiri
    public WheelCollider roda2;            // Collider roda depan kanan
    public WheelCollider roda3;            // Collider roda belakang kiri
    public WheelCollider roda4;            // Collider roda belakang kanan

    void Update()
    {
        HandleMovement();
        HandleSteering();
        HandleBraking();
    }

    // Fungsi untuk menangani akselerasi/deselerasi dinamis
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical"); // Input W/S atau panah atas/bawah

        if (moveInput != 0)
        {
            // Tambahkan akselerasi atau deselerasi berdasarkan input
            currentSpeed += moveInput * acceleration * Time.deltaTime;
        }
        else
        {
            // Kurangi kecepatan secara bertahap jika tidak ada input
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Batasi kecepatan maksimum
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Terapkan gaya motor pada semua roda
        roda1.motorTorque = currentSpeed;
        roda2.motorTorque = currentSpeed;
        roda3.motorTorque = currentSpeed;
        roda4.motorTorque = currentSpeed;
    }

    // Fungsi untuk menangani kemudi dengan skala berdasarkan kecepatan
    private void HandleSteering()
    {
        float turnInput = Input.GetAxis("Horizontal"); // Input A/D atau panah kiri/kanan
        float speedFactor = Mathf.Clamp(currentSpeed / maxSpeed, 0.1f, 1f); // Faktor skala berdasarkan kecepatan
        float steeringAngle = maxSteeringAngle * turnInput * speedFactor;

        // Terapkan sudut kemudi pada roda depan
        roda1.steerAngle = steeringAngle;
        roda2.steerAngle = steeringAngle;
    }

    // Fungsi untuk menangani pengereman bertahap
    private void HandleBraking()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Kurangi kecepatan secara bertahap
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakingForce * Time.deltaTime);

            // Terapkan gaya rem pada roda depan
            roda1.brakeTorque = brakingForce;
            roda2.brakeTorque = brakingForce;

            // Kurangi traksi pada roda belakang untuk memungkinkan drift
            WheelFrictionCurve sidewaysFriction = roda3.sidewaysFriction;
            sidewaysFriction.stiffness = 0.5f; // Kurangi stiffness untuk simulasi slip
            roda3.sidewaysFriction = sidewaysFriction;
            roda4.sidewaysFriction = sidewaysFriction;
        }
        else
        {
            // Kembalikan gaya rem dan traksi saat pengereman dilepaskan
            roda1.brakeTorque = 0;
            roda2.brakeTorque = 0;
            roda3.brakeTorque = 0;
            roda4.brakeTorque = 0;

            // Pulihkan traksi roda belakang
            WheelFrictionCurve defaultFriction = roda3.sidewaysFriction;
            defaultFriction.stiffness = 1.0f; // Pulihkan stiffness normal
            roda3.sidewaysFriction = defaultFriction;
            roda4.sidewaysFriction = defaultFriction;
        }
    }
}
