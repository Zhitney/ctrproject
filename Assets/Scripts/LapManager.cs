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
    public List<Transform> checkpoints;     // Daftar checkpoint pada track
    private HashSet<int> checkpointsPassed; // Set untuk melacak checkpoint yang dilewati


    private void Start()
    {
        gameStartTime = Time.time;
        lapStartTime = gameStartTime;
        UpdateLapUI();                                  // Inisialisasi UI saat game dimulai
        checkpointsPassed = new HashSet<int>(); // Reset checkpoint yang sudah dilewati

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

    public void CheckpointReached(int checkpointIndex)
    {
        if (!checkpointsPassed.Contains(checkpointIndex))
        {
            checkpointsPassed.Add(checkpointIndex);
            Debug.Log($"Checkpoint {checkpointIndex} dilewati. Total: {checkpointsPassed.Count}/{checkpoints.Count}");
        }
        else
        {
            Debug.Log($"Checkpoint {checkpointIndex} sudah dilewati sebelumnya.");
        }

        if (checkpointsPassed.Count == checkpoints.Count)
        {
            Debug.Log("Semua checkpoint dilewati!");
        }
    }


    private void LapCompleted()
    {
        if (currentLap < totalLaps)
        {
            lapDetected = true;                // Set flag lap detected
            currentLap++;                      // Tambah lap saat ini
            float lapTime = Time.time - lapStartTime; // Hitung waktu lap
            lapTimes.Add(lapTime);             // Simpan waktu lap
            UpdateLapUI();                     // Update UI lap
            lapStartTime = Time.time;          // Mulai waktu untuk lap berikutnya

            Debug.Log("Current Lap: " + currentLap);

            // Reset checkpoint untuk lap berikutnya
            checkpointsPassed.Clear();         // Gunakan Clear() untuk membersihkan set
            lapDetected = false;               // Reset flag setelah reset checkpoint

            // Jika lap sudah selesai
            if (currentLap == totalLaps)
            {
                FinishRace();                  // Panggil fungsi untuk akhir balapan
            }
        }
    }


    // Trigger ketika mobil melewati garis start/finish
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "raceCarGreen")
        {
            Debug.Log("Masuk garis Start/Finish.");

            // Jika semua checkpoint sudah dilewati dan tidak dalam keadaan lapDetected
            if (!lapDetected && checkpointsPassed.Count == checkpoints.Count)
            {
                Debug.Log("Lap selesai karena semua checkpoint dilewati.");
                LapCompleted();
            }
            else if (checkpointsPassed.Count != checkpoints.Count)
            {
                Debug.Log("Belum semua checkpoint dilewati. Lap tidak dihitung.");
            }
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