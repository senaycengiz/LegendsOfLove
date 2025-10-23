using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerC : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 4.5f;
    public float sprintMultiplier = 1.6f;
    public float rotationSpeed = 12f;

    [Header("Camera Reference")]
    public OrbitCamera orbitCamera;

    [Header("Gravity & Jump")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.25f;
    public float groundedRemember = 0.12f;

    [Tooltip("İstersen CC.isGrounded yerine fiziksel ground check kullan.")]
    public bool usePhysicsGroundCheck = false;
    public LayerMask groundMask;
    public float groundCheckOffset = 0.05f;
    public float groundCheckRadius = 0.18f;

    [Header("Shooting (Raycast)")]
    public float shootRange = 100f;
    public LayerMask hitMask;
    public Transform muzzle;
    public ParticleSystem muzzleFlash;
    public AudioSource shotAudio;

    CharacterController cc;
    Animator anim;
    float yVel;
    float groundedTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        if (!orbitCamera) orbitCamera = FindObjectOfType<OrbitCamera>();
    }

    void Update()
    {
        // --- 1) Girdi
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        UnityEngine.Vector3 input = new UnityEngine.Vector3(h, 0f, v).normalized;

        // --- 2) Kameraya göre yön ---
        float cameraYaw = orbitCamera ? orbitCamera.Yaw : transform.eulerAngles.y;
        Quaternion yawRot = Quaternion.Euler(0f, cameraYaw, 0f);
        UnityEngine.Vector3 moveDir = yawRot * input;
        moveDir.Normalize();

        // --- 3) Zemin kontrolü
        bool grounded = usePhysicsGroundCheck ? PhysicsGrounded() : cc.isGrounded;
        groundedTimer -= Time.deltaTime;
        if (grounded) groundedTimer = groundedRemember;

        // --- 4) Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && groundedTimer > 0f)
        {
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("Jump");
        }

        // --- 5) Yer çekimi
        if (grounded && yVel < 0f) yVel = -2f;
        yVel += gravity * Time.deltaTime;

        // --- 6) Sprint
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier;

        // --- 7) Hareket uygula
        UnityEngine.Vector3 velocity = moveDir * speed + UnityEngine.Vector3.up * yVel;
        cc.Move(velocity * Time.deltaTime);

        // --- ✅ 8) Kamera yönüne dön (idle haldeyken bile)
        Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);

        // Eğer hareket varsa hızlı, hareket yoksa yavaş dönsün
        float turnRate = (input.sqrMagnitude > 0.001f) ? rotationSpeed * 5f : rotationSpeed * 2f;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnRate * Time.deltaTime);

        // --- 9) Animator besleme
        float planarSpeed = new UnityEngine.Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        anim.SetFloat("Speed", planarSpeed);
        anim.SetBool("IsGrounded", grounded);

        // --- 10) Ateş (Sol Tık)
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Fire");
            FaceCameraYaw();
            ShootRay();
        }
    }

    // --- Atış ---
    void ShootRay()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        UnityEngine.Vector3 screenCenter = new UnityEngine.Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray aimRay = cam.ScreenPointToRay(screenCenter);

        UnityEngine.Vector3 aimPoint;
        int aimMask = ~0;
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, shootRange, aimMask, QueryTriggerInteraction.Ignore))
            aimPoint = aimHit.point;
        else
            aimPoint = aimRay.origin + aimRay.direction * shootRange;

        UnityEngine.Vector3 origin = muzzle ? muzzle.position : cam.transform.position;
        UnityEngine.Vector3 dir = (aimPoint - origin).normalized;

        if (muzzle)
        {
            float dot = UnityEngine.Vector3.Dot(muzzle.forward, dir);
            if (dot < 0.1f) dir = muzzle.forward;
        }

        if (Physics.Raycast(origin, dir, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                    enemy.TakeDamage(50);
            }
        }

        if (muzzleFlash)
        {
            muzzleFlash.gameObject.SetActive(true);
            muzzleFlash.Play();
        }
        if (shotAudio)
            shotAudio.Play();
    }

    void FaceCameraYaw(float turnSpeed = 20f)
    {
        if (!orbitCamera) return;
        Quaternion look = Quaternion.Euler(0f, orbitCamera.Yaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
    }

    bool PhysicsGrounded()
    {
        UnityEngine.Vector3 baseCenter = transform.position + cc.center + UnityEngine.Vector3.down * (cc.height / 2f - cc.radius + groundCheckOffset);
        return Physics.CheckSphere(baseCenter, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        if (!usePhysicsGroundCheck || cc == null) return;
        Gizmos.color = Color.cyan;
        UnityEngine.Vector3 baseCenter = transform.position + cc.center + UnityEngine.Vector3.down * (cc.height / 2f - cc.radius + groundCheckOffset);
        Gizmos.DrawWireSphere(baseCenter, groundCheckRadius);
    }
}