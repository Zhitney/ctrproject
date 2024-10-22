using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeControl : MonoBehaviour
{
    public ParticleSystem smokeParticle;  // Referensi ke Particle System

    public float speedThreshold = 1f;     // Kecepatan minimum untuk memunculkan asap
    private Rigidbody rb;                 // Referensi ke Rigidbody mobil

    void Start()
    {
        rb = GetComponent<Rigidbody>();   // Mendapatkan Rigidbody mobil
    }

    void Update()
    {
        // Aktifkan atau nonaktifkan partikel berdasarkan kecepatan mobil
        if (rb.velocity.magnitude > speedThreshold)
        {
            if (!smokeParticle.isPlaying)
                smokeParticle.Play();     // Mulai partikel jika belum aktif
        }
        else
        {
            if (smokeParticle.isPlaying)
                smokeParticle.Stop();     // Hentikan partikel jika mobil berhenti
        }
    }
}
