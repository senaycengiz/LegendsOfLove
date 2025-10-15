using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // PlayerRoot/CameraTarget
    public float targetHeight = 0f;       // ekstra dikey ofset (gerekirse)

    [Header("Orbit")]
    public float distance = 4.2f;
    public float minDistance = 2.0f;
    public float maxDistance = 6.0f;
    public float mouseSensitivityX = 150f;
    public float mouseSensitivityY = 120f;
    public float minPitch = -30f;         // aşağı bakış limiti
    public float maxPitch = 70f;          // yukarı bakış limiti
    public float smooth = 12f;            // konum yumuşatma

    [Header("Shoulder Offset (local, yaw/pitch'e göre)")]
    // X: sağa (+) / sola (-), Y: yukarı, Z: ileri (kamera önüne) – genelde Z=0
    public Vector3 shoulderOffset = new Vector3(0.35f, 1.55f, 0f);

    [Header("Look Offset (local, yaw/pitch'e göre)")]
    // Kameranın baktığı noktayı hedefin biraz sağına/yukarısına kaydırır.
    // X'i büyütürsen karakter ekranda daha sola itilir.
    public Vector3 lookOffset = new Vector3(0.40f, 0.10f, 0f);

    [Header("Collision")]
    public float collisionRadius = 0.2f;  // spherecast yarıçapı
    // Sadece çevre katmanlarını ekle (Ground, Environment)
    public LayerMask obstructionMask;

    // dahili durum
    float yaw;     // yatay dönüş (Mouse X)
    float pitch;   // dikey dönüş (Mouse Y)

    void Start()
    {
        if (!target) { enabled = false; return; }

        // başlangıç rotasyonundan yaw/pitch al
        var e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        // İstersen imleci kilitle:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // --- 1) Fare girdisi
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // --- 2) Zoom (tekerlek)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            distance = Mathf.Clamp(distance - scroll * 3f, minDistance, maxDistance);

        // --- 3) Hedef noktası (omurga/baş hizası)
        Vector3 targetPos = target.position + Vector3.up * targetHeight;

        // --- 4) Kamera rotasyonu ve lokal ofsetler
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        // Pozisyon için: omuz ofseti + geri kaçma (negatif Z = kamera geride)
        Vector3 localPosOffset = shoulderOffset + new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = targetPos + rot * localPosOffset;

        // Bakış için: hedefin biraz sağı/yukarısı (reticle merkezde kalır, karakter kadrajda sola kayar)
        Vector3 lookTarget = targetPos + rot * lookOffset;

        // --- 5) Çarpışma: hedef→kamera doğrultusunda kısalt
        Vector3 dir = (desiredPos - targetPos);
        float wantDist = dir.magnitude;
        if (wantDist > 0.0001f)
        {
            Vector3 dirN = dir / wantDist;
            if (Physics.SphereCast(targetPos, collisionRadius, dirN, out RaycastHit hit, wantDist, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                // minDistance'den daha yakına girmesine izin verme
                float clipped = Mathf.Max(minDistance, hit.distance - 0.1f);
                desiredPos = targetPos + dirN * clipped;
            }
        }

        // --- 6) Yumuşak yerleştirme + bakış
        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);

        // Debug görmek istersen:
        // Debug.DrawLine(targetPos, transform.position, Color.cyan);
        // Debug.DrawRay(targetPos, (lookTarget - targetPos).normalized * 0.5f, Color.yellow);
    }
}