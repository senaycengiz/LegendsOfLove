using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // PlayerRoot/CameraTarget
    public float targetHeight = 0f;   // ekstra dikey ofset (gerekirse)

    [Header("Orbit")]
    public float distance = 4.2f;
    public float minDistance = 2.0f;
    public float maxDistance = 6.0f;
    public float mouseSensitivityX = 150f;
    public float mouseSensitivityY = 120f;
    public float minPitch = -30f;     // aşağı bakış limiti
    public float maxPitch = 70f;      // yukarı bakış limiti
    public float smooth = 12f;

    [Header("Shoulder Offset (local to camera yaw/pitch)")]
    // Omuz üstü görünüm: X sağa-sola, Y yukarı-aşağı, Z ileri-geri (negatif Z = kamerayı geriye çeker)
    public Vector3 shoulderOffset = new Vector3(0.35f, 1.55f, 0f);

    [Header("Collision")]
    public float collisionRadius = 0.2f;
    // Sadece duvar/zemin katmanlarını ekle (Ground, Environment)
    public LayerMask obstructionMask;

    float yaw;   // yatay dönüş (Mouse X)
    float pitch; // dikey dönüş (Mouse Y)
    float currentDistance;

    void Start()
    {
        if (!target) { enabled = false; return; }

        currentDistance = distance;
        var rot = transform.rotation.eulerAngles;
        yaw = rot.y;
        pitch = rot.x;

        // (İstersen) imleci kilitle:
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1) Fare girdisi
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 2) Zoom (tekerlek)
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 3f, minDistance, maxDistance);

        // 3) Hedef noktası (bakacağımız nokta)
        Vector3 targetPos = target.position + Vector3.up * targetHeight;

        // 4) Yaw/pitch'e göre rotasyon ve lokaldeki omuz ofseti
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        //     -distance eksenini ayrıca uygularız (geri kaçma)
        Vector3 localOffset = shoulderOffset + new Vector3(0f, 0f, -distance);

        // 5) İstenen kamera pozisyonu (rotasyonla lokal ofseti world'e çevir)
        Vector3 desiredPos = targetPos + rot * localOffset;

        // 6) Çarpışma: hedef→kamera yönüne SphereCast
        Vector3 rayDir = (desiredPos - targetPos).normalized;
        float desiredDist = localOffset.magnitude; // yaklaşık (omuz + mesafe)
        // Sadece mesafeyi kısaltmak istiyoruz; hedefe olan doğrultuda çarpışma testi:
        if (Physics.SphereCast(targetPos, collisionRadius, rayDir, out RaycastHit hit, distance, obstructionMask, QueryTriggerInteraction.Ignore))
            desiredPos = targetPos + rayDir * Mathf.Max(minDistance, hit.distance - 0.1f);

        // 7) Yumuşak yerleştirme + bakış
        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);

        // Debug görmek istersen:
        // Debug.DrawLine(targetPos, transform.position, Color.cyan);
    }
}