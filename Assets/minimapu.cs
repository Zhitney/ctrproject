using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapu : MonoBehaviour
{   
    public Transform objHero; // Referensi ke objek mobil
    public RectTransform carIndicator; // Referensi ke indikator mobil (lingkaran merah di Canvas)
    public Camera minimapCamera; // Referensi ke kamera minimap

    private void Update()
    {
       
        // Update posisi indikator mobil di Canvas
        if (minimapCamera != null && carIndicator != null)
        {
            // Konversi posisi mobil dari dunia 3D ke posisi UI 2D di Canvas
            Vector3 screenPos = minimapCamera.WorldToViewportPoint(objHero.position);

            // Posisikan lingkaran merah berdasarkan viewport
            carIndicator.anchorMin = screenPos;
            carIndicator.anchorMax = screenPos;
        }
    }

}
