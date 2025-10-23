using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;           // Player veya CameraTarget
    public float targetHeight = 1.5f;  // karakterin omuz / baş hizası

    [Header("Orbit Ayarları")]
    public float distance = 4f;        // Kamera uzaklığı
    public float minDistance = 2f;
    public float maxDistance = 6f;
    public float sensitivityX = 150f;  // Fare X hassasiyeti
    public float sensitivityY = 120f;  // Fare Y hassasiyeti
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public float smooth = 10f;         // Kamera yumuşatma hızı

    [Header("Omuz Ofseti (Yerel)")]
    public Vector3 shoulderOffset = new Vector3(0.35f, 1.55f, 0f);

    [Header("Bakış Ofseti (Yerel)")]
    public Vector3 lookOffset = new Vector3(0.4f, 0.1f, 0f);

    [Header("Çarpışma")]
    public float collisionRadius = 0.2f;
    public LayerMask obstructionMask;

    // Dahili değişkenler
    private float yaw;     // sağ-sol dönüş
    private float pitch;   // yukarı-aşağı dönüş
    private Vector3 currentPos; // yumuşatma için

    // >>> PlayerController'ın okuyabilmesi için public getter:
    public float Yaw => yaw;

    void Start()
    {
        if (!target)
        {
            Debug.LogWarning("OrbitCamera: Target atanmamış!");
            enabled = false;
            return;
        }

        // Başlangıç açılarını kaydet
        Vector3 e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        // Fareyi kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1️⃣ Fare girdisi
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * sensitivityX * Time.deltaTime;
        pitch -= mouseY * sensitivityY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 2️⃣ Zoom (tekerlek)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
            distance = Mathf.Clamp(distance - scroll * 3f, minDistance, maxDistance);

        // 3️⃣ Hedef noktası
        Vector3 targetPos = target.position + Vector3.up * targetHeight;

        // 4️⃣ Rotasyon ve ofset
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 localOffset = shoulderOffset + new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = targetPos + rot * localOffset;
        Vector3 lookTarget = targetPos + rot * lookOffset;

        // 5️⃣ Çarpışma kontrolü (duvara girmesin)
        Vector3 dir = desiredPos - targetPos;
        float wantDist = dir.magnitude;
        if (wantDist > 0.001f)
        {
            Vector3 dirN = dir / wantDist;
            if (Physics.SphereCast(targetPos, collisionRadius, dirN, out RaycastHit hit, wantDist, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                float clipped = Mathf.Max(minDistance, hit.distance - 0.1f);
                desiredPos = targetPos + dirN * clipped;
            }
        }

        // 6️⃣ Konum yumuşatma ve bakış yönü
        if (currentPos == Vector3.zero)
            currentPos = desiredPos;

        currentPos = Vector3.Lerp(currentPos, desiredPos, smooth * Time.deltaTime);
        transform.position = currentPos;
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
    }
}