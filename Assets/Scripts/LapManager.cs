using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3;                           // Jumlah lap yang harus diselesaikan
    private int currentLap = 0;                         // Lap saat ini
    public Text lapText;                                // UI Text untuk menampilkan jumlah lap
    public Text lapTimeText;                            // UI Text untuk menampilkan waktu lap
    public Text totalTimeText;                          // UI Text untuk menampilkan waktu total
    public Transform startFinishLine;                   // Garis Start/Finish
    public mobiljalan carController;                    // Reference to the car controller

    private float gameStartTime;                        // Waktu mulai game
    private float lapStartTime;                         // Waktu mulai lap
    private List<float> lapTimes = new List<float>();   // List untuk menyimpan waktu setiap lap
    private bool lapDetected = false;                   // Flag untuk mendeteksi crossing garis

    private void Start()
    {
        gameStartTime = Time.time;
        lapStartTime = gameStartTime;
        UpdateLapUI();                                  // Inisialisasi UI saat game dimulai
    }

    private void Update()
    {
        // Update total time continuously
        if (currentLap < totalLaps)
        {
            float totalTime = Time.time - gameStartTime;
            totalTimeText.text = FormatTime(totalTime); // Only show the formatted time
        }

        // Reset lap detection after a delay to avoid multiple detections
        if (lapDetected && Time.time - lapStartTime > 1.0f)  // Adjust the delay as needed
        {
            lapDetected = false;
        }
    }

    // Trigger ketika mobil melewati garis start/finish
    private void OnTriggerEnter(Collider other)
    {
        // Tambahkan log untuk memeriksa apakah mobil dan garis berinteraksi
        Debug.Log("Something entered the trigger: " + other.name);

        //if (other.CompareTag("Player") && !lapDetected) // Pastikan hanya mobil pemain yang dihitung dan belum terdeteksi di lap ini
        if (other.gameObject.name == "raceCarGreen")
        {
            Debug.Log("Masuk");
            Rigidbody carRb = other.GetComponent<Rigidbody>();
            Vector3 velocity = carRb.velocity;

            // Pastikan mobil bergerak maju
            Debug.Log("Player Velocity: " + velocity);
            //if (Vector3.Dot(velocity, startFinishLine.forward) > 0)
            //{
            Debug.Log("Lap detected!");

            // Deteksi jika mobil melewati garis
            if (currentLap < totalLaps)
            {
                lapDetected = true;                // Set flag lap detected
                currentLap++;                      // Tambah lap saat ini
                float lapTime = Time.time - lapStartTime; // Hitung waktu lap
                lapTimes.Add(lapTime);             // Simpan waktu lap
                UpdateLapUI();                     // Update UI lap
                lapStartTime = Time.time;          // Mulai waktu untuk lap berikutnya

                Debug.Log("Current Lap: " + currentLap);

                // Jika lap sudah selesai
                if (currentLap == totalLaps)
                {
                    FinishRace();                  // Panggil fungsi untuk akhir balapan
                }
            }
            //}
        }
    }


    // Update UI Lap
    private void UpdateLapUI()
    {
        lapText.text = currentLap + " / " + totalLaps;
        if (currentLap > 0)
        {
            lapTimeText.text = FormatTime(lapTimes[currentLap - 1]);
        }
    }

    // Format waktu menjadi string yang lebih mudah dibaca
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = (time * 1000) % 1000;
        return string.Format("{0}:{1:00}.{2:000}", minutes, seconds, (int)milliseconds);
    }

    // Fungsi untuk menampilkan pesan akhir balapan
    private void FinishRace()
    {
        Debug.Log("Race Finished!");
        lapText.text = "Race Finished!";

        // Menampilkan waktu setiap lap
        lapTimeText.text = "Final Lap Times:\n";
        for (int i = 0; i < lapTimes.Count; i++)
        {
            lapTimeText.text += FormatTime(lapTimes[i]) + "\n";
        }

        // Menampilkan total waktu balapan
        float totalTime = Time.time - gameStartTime;
        totalTimeText.text = "Total Time: " + FormatTime(totalTime);

        // Disable the car controller to stop the car
        carController.enabled = false;

        // Anda bisa menambahkan logic tambahan di sini seperti menghentikan mobil, menunjukkan menu hasil, dsb.
    }
}