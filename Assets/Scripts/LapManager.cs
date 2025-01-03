using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3;                           // Jumlah lap yang harus diselesaikan
    private int currentLap = 0;                         // Lap saat ini
    public Text lapText;                                // UI Text untuk menampilkan jumlah lap
    public Text lapTimeText;                            // UI Text untuk menampilkan waktu lap
    public Text totalTimeText;                          // UI Text untuk menampilkan waktu total
    public Transform startFinishLine;                   // Garis Start/Finish
    // public SimpleKartController carController;                    // Reference to the car controller

    private float gameStartTime;                        // Waktu mulai game
    private float lapStartTime;                         // Waktu mulai lap
    private List<float> lapTimes = new List<float>();   // List untuk menyimpan waktu setiap lap
    private bool lapDetected = false;                   // Flag untuk mendeteksi crossing garis
    public List<Transform> checkpoints;                // Daftar checkpoint pada track
    private HashSet<int> checkpointsPassed;            // Set untuk melacak checkpoint yang dilewati

    public List<CarProgress> carsProgress = new List<CarProgress>();
    public Text rankText; // UI Text for showing rank
    private bool raceFinished = false; // Menandai apakah balapan sudah selesai
    private float finalTotalTime = 0f; // Menyimpan total waktu balapan

    [System.Serializable]
    public class CarProgress
    {
        public GameObject car;
        public int currentLap = 0;
        public HashSet<int> checkpointsPassed = new HashSet<int>();
        public float distanceToNextCheckpoint;
    }

    private void Start()
    {
        AddCarsByTag("Player");
        AddCarsByTag("Enemy");

        Debug.Log($"Total cars added to progress: {carsProgress.Count}");

        gameStartTime = Time.time;
        lapStartTime = gameStartTime;
        UpdateLapUI();                                  // Inisialisasi UI saat game dimulai
        checkpointsPassed = new HashSet<int>();         // Reset checkpoint yang sudah dilewati
    }

    private void Update()
    {
        // Hanya perbarui waktu total jika balapan belum selesai
        if (!raceFinished)
        {
            float totalTime = Time.time - gameStartTime;
            totalTimeText.text = FormatTime(totalTime); // Tampilkan waktu yang diperbarui
        }

        // Reset lap detection after a delay to avoid multiple detections
        if (lapDetected && Time.time - lapStartTime > 1.0f) // Adjust the delay as needed
        {
            lapDetected = false;
        }

        UpdateRankings();
    }

    public void CheckpointReached(GameObject car, int checkpointIndex)
    {
        // Find the CarProgress object for the specific car
        CarProgress carProgress = carsProgress.Find(c => c.car == car);

        if (carProgress != null && !carProgress.checkpointsPassed.Contains(checkpointIndex))
        {
            carProgress.checkpointsPassed.Add(checkpointIndex);
            carProgress.distanceToNextCheckpoint = Vector3.Distance(
                car.transform.position,
                checkpoints[(checkpointIndex + 1) % checkpoints.Count].position
            );

            Debug.Log($"Car {car.name} (Tag: {car.tag}) passed checkpoint {checkpointIndex}. Total: {carProgress.checkpointsPassed.Count}/{checkpoints.Count}");

            if (carProgress.checkpointsPassed.Count == checkpoints.Count)
            {
                Debug.Log($"Car {car.name} (Tag: {car.tag}) has passed all checkpoints!");
            }
        }
        else if (carProgress != null)
        {
            Debug.Log($"Checkpoint {checkpointIndex} already passed by {car.name} (Tag: {car.tag}).");
        }
    }

    private void LapCompleted(GameObject car)
    {
        CarProgress carProgress = carsProgress.Find(c => c.car == car);

        if (carProgress != null && carProgress.currentLap < totalLaps)
        {
            lapDetected = true;                         // Set flag lap detected
            carProgress.currentLap++;                  // Tambah lap saat ini
            float lapTime = Time.time - lapStartTime;   // Hitung waktu lap
            lapTimes.Add(lapTime);                     // Simpan waktu lap
            UpdateLapUI();                             // Update UI lap
            lapStartTime = Time.time;                  // Mulai waktu untuk lap berikutnya

            Debug.Log($"Car {car.name} completed lap {carProgress.currentLap}/{totalLaps}");

            // Reset checkpoint untuk lap berikutnya
            carProgress.checkpointsPassed.Clear();

            // Jika lap sudah selesai
            if (carProgress.currentLap == totalLaps)
            {
                FinishRace(car);
            }
        }
    }

    // Trigger ketika mobil melewati garis start/finish
    private void OnTriggerEnter(Collider other)
    {
        CarProgress carProgress = carsProgress.Find(c => c.car == other.gameObject);

        if (carProgress != null)
        {
            if (!lapDetected && carProgress.checkpointsPassed.Count == checkpoints.Count)
            {
                LapCompleted(other.gameObject);
            }
            else if (carProgress.checkpointsPassed.Count != checkpoints.Count)
            {
                Debug.Log($"Car {other.gameObject.name} did not pass all checkpoints. Lap not counted.");
            }
        }
    }

    // Update UI Lap
    private void UpdateLapUI()
{
    // Find the player's progress object
    CarProgress playerProgress = carsProgress.Find(car => car.car.CompareTag("Player"));

    if (playerProgress != null)
    {
        // Update lap text for the player
        lapText.text = playerProgress.currentLap + " / " + totalLaps;

        // Update lap time if the player has completed at least one lap
        if (playerProgress.currentLap > 0 && playerProgress.currentLap <= lapTimes.Count)
        {
            lapTimeText.text = FormatTime(lapTimes[playerProgress.currentLap - 1]);
        }
    }
    else
    {
        Debug.LogWarning("Player progress not found. Make sure the player car is correctly tagged.");
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
    private void FinishRace(GameObject car)
    {
        if (raceFinished) return; // Hindari eksekusi ganda
        raceFinished = true; // Tandai balapan selesai

        Debug.Log($"{car.name} finished the race!");
        lapText.text = "Finished!"; // Update lap text

        // Menampilkan waktu setiap lap
        lapTimeText.text = "Final Lap Times:\n";
        for (int i = 0; i < lapTimes.Count; i++)
        {
            lapTimeText.text += FormatTime(lapTimes[i]) + "\n";
        }

        // Simpan dan tampilkan total waktu balapan
        finalTotalTime = Time.time - gameStartTime; // Simpan total waktu saat balapan selesai
        totalTimeText.text = "Total Time: " + FormatTime(finalTotalTime);

        var rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Hentikan kecepatan
            rb.angularVelocity = Vector3.zero; // Hentikan rotasi
            rb.isKinematic = true; // Nonaktifkan simulasi fisika sementara
        }
    }

    private void UpdateRankings()
    {
        carsProgress.Sort((carA, carB) =>
        {
            // Calculate initial distance to the first checkpoint if no progress
            float initialDistanceA = carA.checkpointsPassed.Count == 0 ? Vector3.Distance(carA.car.transform.position, checkpoints[0].transform.position) : 0;
            float initialDistanceB = carB.checkpointsPassed.Count == 0 ? Vector3.Distance(carB.car.transform.position, checkpoints[0].transform.position) : 0;

            int lastCheckpointA = carA.checkpointsPassed.Count > 0 ? carA.checkpointsPassed.Max() : -1; // Default to -1 if no checkpoints passed
            int lastCheckpointB = carB.checkpointsPassed.Count > 0 ? carB.checkpointsPassed.Max() : -1;

            if (carA.currentLap != carB.currentLap)
                return carB.currentLap.CompareTo(carA.currentLap); // Higher lap first
            if (lastCheckpointA != lastCheckpointB)
                return lastCheckpointB.CompareTo(lastCheckpointA); // Further along the track first
            if (carA.checkpointsPassed.Count == 0 && carB.checkpointsPassed.Count == 0)
                return initialDistanceA.CompareTo(initialDistanceB); // Closer to the first checkpoint
            return carA.distanceToNextCheckpoint.CompareTo(carB.distanceToNextCheckpoint); // Closer to the next checkpoint first
        });

        // Debug log to verify rankings
        for (int i = 0; i < carsProgress.Count; i++)
        {
            Debug.Log($"Rank {i + 1}: {carsProgress[i].car.name}, Lap: {carsProgress[i].currentLap}, Checkpoints: {carsProgress[i].checkpointsPassed.Count}, Distance: {carsProgress[i].distanceToNextCheckpoint}");
        }

        // Update rank UI for the player
        for (int i = 0; i < carsProgress.Count; i++)
        {
            if (carsProgress[i].car.CompareTag("Player"))
            {
                rankText.text = $"Rank: {i + 1}/{carsProgress.Count}";
                break;
            }
        }
    }



    private void AddCarsByTag(string tag)
    {
        foreach (GameObject car in GameObject.FindGameObjectsWithTag(tag))
        {
            if (car.GetComponent<Rigidbody>()) // Or any other component specific to your cars
            {
                Debug.Log($"Adding car with tag {tag}: {car.name}");
                carsProgress.Add(new CarProgress
                {
                    car = car,
                    currentLap = 0,
                    checkpointsPassed = new HashSet<int>(),
                    distanceToNextCheckpoint = 0
                });
            }
            else
            {
                Debug.Log($"Skipping non-car object with tag {tag}: {car.name}");
            }
        }
    }
}
