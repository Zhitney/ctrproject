using UnityEngine;
using UnityEngine.UI;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3;            // Jumlah lap yang harus diselesaikan
    private int currentLap = 0;          // Lap saat ini
    public Text lapText;                 // UI Text untuk menampilkan jumlah lap
    public Transform startFinishLine;    // Garis Start/Finish
    public mobiljalan carController;     // Reference to the car controller

    private void Start()
    {
        UpdateLapUI();                   // Inisialisasi UI saat game dimulai
    }

    // Trigger ketika mobil melewati garis start/finish
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Pastikan hanya mobil pemain yang dihitung
        {
            // Use car's velocity to determine the direction
            Rigidbody carRb = other.GetComponent<Rigidbody>();
            Vector3 velocity = carRb.velocity;

            // Ensure the car is moving forward
            if (Vector3.Dot(velocity, startFinishLine.forward) > 0)
            {
                // Deteksi jika mobil melewati garis
                if (currentLap < totalLaps)
                {
                    currentLap++;            // Tambah lap saat ini
                    UpdateLapUI();           // Update UI lap
                }
                // Jika lap sudah selesai
                if (currentLap == totalLaps)
                {
                    FinishRace();            // Panggil fungsi untuk akhir balapan
                }
            }
        }
    }

    // Update UI Lap
    private void UpdateLapUI()
    {
        lapText.text = "Lap: " + currentLap + " / " + totalLaps;
    }

    // Fungsi untuk menampilkan pesan akhir balapan
    private void FinishRace()
    {
        Debug.Log("Race Finished!");
        lapText.text = "Race Finished!";
        // Disable the car controller to stop the car
        carController.enabled = false;
        // Anda bisa menambahkan logic tambahan di sini seperti menghentikan mobil, menunjukkan menu hasil, dsb.
    }
}
