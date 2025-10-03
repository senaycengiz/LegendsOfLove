using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;   // Player referansı
    public float distance = 4f;     // Kamera uzaklığı
    public float sensitivity = 2f;  // Mouse hassasiyeti
    public float yMin = -20f, yMax = 60f; // Dikey açı limiti

    private float yaw, pitch;

    void LateUpdate()
    {
        if (!target) return;

        // Mouse hareketlerini oku
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, yMin, yMax);

        // Rotasyon ve pozisyon hesapla
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = target.position - rotation * Vector3.forward * distance;

        // Kamerayı konumlandır
        transform.position = position;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
