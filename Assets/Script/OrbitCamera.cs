using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float targetHeight = 1.5f;

    [Header("Orbit Ayarları")]
    public float distance = 4f;
    public float minDistance = 2f;
    public float maxDistance = 6f;
    public float sensitivityX = 150f;
    public float sensitivityY = 120f;
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public float smooth = 10f;

    [Header("Omuz Ofseti (Yerel)")]
    public UnityEngine.Vector3 shoulderOffset = new UnityEngine.Vector3(0.35f, 1.55f, 0f);

    [Header("Bakış Ofseti (Yerel)")]
    public UnityEngine.Vector3 lookOffset = new UnityEngine.Vector3(0.4f, 0.1f, 0f);

    [Header("Çarpışma")]
    public float collisionRadius = 0.2f;
    public LayerMask obstructionMask;

    private float yaw;
    private float pitch;
    private UnityEngine.Vector3 currentPos;

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
        UnityEngine.Vector3 e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        //Oyun başında fareyi kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        //Eğer oyun duraklatıldıysa 
        if (Time.timeScale == 0f)
        {
            // Fareyi görünür bırak, kamera hareket etmesin
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // Oyun devam ediyorsa fareyi kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Fare girdisi
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * sensitivityX * Time.deltaTime;
        pitch -= mouseY * sensitivityY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
            distance = Mathf.Clamp(distance - scroll * 3f, minDistance, maxDistance);

        // Hedef noktası
        UnityEngine.Vector3 targetPos = target.position + UnityEngine.Vector3.up * targetHeight;

        // Rotasyon ve ofset
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        UnityEngine.Vector3 localOffset = shoulderOffset + new UnityEngine.Vector3(0f, 0f, -distance);
        UnityEngine.Vector3 desiredPos = targetPos + rot * localOffset;
        UnityEngine.Vector3 lookTarget = targetPos + rot * lookOffset;

        // Çarpışma kontrolü
        UnityEngine.Vector3 dir = desiredPos - targetPos;
        float wantDist = dir.magnitude;
        if (wantDist > 0.001f)
        {
            UnityEngine.Vector3 dirN = dir / wantDist;
            if (Physics.SphereCast(targetPos, collisionRadius, dirN, out RaycastHit hit, wantDist, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                float clipped = Mathf.Max(minDistance, hit.distance - 0.1f);
                desiredPos = targetPos + dirN * clipped;
            }
        }

        // Konum yumuşatma ve bakış yönü
        if (currentPos == UnityEngine.Vector3.zero)
            currentPos = desiredPos;

        currentPos = UnityEngine.Vector3.Lerp(currentPos, desiredPos, smooth * Time.deltaTime);
        transform.position = currentPos;
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position, UnityEngine.Vector3.up);
    }
}