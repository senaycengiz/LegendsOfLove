using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerC : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 4.5f;        // normal hız
    public float sprintMultiplier = 1.6f; // Shift basılıyken çarpan
    public float rotationSpeed = 12f;

    [Header("Gravity & Jump")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.25f;
    public float groundedRemember = 0.12f; // coyote time

    [Tooltip("İstersen CC.isGrounded yerine fiziksel ground check kullan.")]
    public bool usePhysicsGroundCheck = false;
    public LayerMask groundMask;            // Ground/Environment
    public float groundCheckOffset = 0.05f; // kapsül tabanından pay
    public float groundCheckRadius = 0.18f;

    [Header("Shooting (Raycast)")]
    public float shootRange = 100f;
    public LayerMask hitMask;               // Enemy layer işaretli olmalı
    public Transform muzzle;                // Pistol/Muzzle
    public ParticleSystem muzzleFlash;      // opsiyonel
    public AudioSource shotAudio;           // opsiyonel

    CharacterController cc;
    Animator anim;
    float yVel;
    float groundedTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- 1) Girdi
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        UnityEngine.Vector3 input = new UnityEngine.Vector3(h, 0f, v);
        input = UnityEngine.Vector3.ClampMagnitude(input, 1f);

        // --- 2) Kameraya göre yön
        Transform cam = Camera.main ? Camera.main.transform : transform;
        UnityEngine.Vector3 camF = UnityEngine.Vector3.Scale(cam.forward, new UnityEngine.Vector3(1f, 0f, 1f)).normalized;
        UnityEngine.Vector3 camR = cam.right;
        UnityEngine.Vector3 moveDir = (camF * input.z + camR * input.x).normalized;

        // --- 3) Zemin kontrolü
        bool grounded = usePhysicsGroundCheck ? PhysicsGrounded() : cc.isGrounded;
        groundedTimer -= Time.deltaTime;
        if (grounded) groundedTimer = groundedRemember;

        // --- 4) Zıplama (Space)
        if (Input.GetKeyDown(KeyCode.Space) && groundedTimer > 0f)
        {
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("Jump"); // Animator'da Jump trigger'ı varsa
        }

        // --- 5) Yer çekimi
        if (grounded && yVel < 0f) yVel = -2f;
        yVel += gravity * Time.deltaTime;

        // --- 6) Sprint (Shift)
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= sprintMultiplier;

        // --- 7) Hareket uygula
        UnityEngine.Vector3 velocity = moveDir * speed + UnityEngine.Vector3.up * yVel;
        cc.Move(velocity * Time.deltaTime);

        // --- 8) Yönlendirme
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(moveDir, UnityEngine.Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }

        // --- 9) Animator besleme
        float planarSpeed = new UnityEngine.Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        anim.SetFloat("Speed", planarSpeed);
        anim.SetBool("IsGrounded", grounded);

        // --- 10) Ateş (Sol Tık)
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Fire");
            FaceCameraYaw();   // opsiyonel: ateş anında kameraya dön
            ShootRay();        // atış işlemi
        }
    }

    // --- 2 aşamalı atış (kamera merkezinden nişan, namludan raycast) ---
    void ShootRay()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        // 1️⃣ Kamera merkezinden hedef noktayı bul
        UnityEngine.Vector3 screenCenter = new UnityEngine.Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray aimRay = cam.ScreenPointToRay(screenCenter);

        UnityEngine.Vector3 aimPoint;
        int aimMask = ~0; // her şeyi görür
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, shootRange, aimMask, QueryTriggerInteraction.Ignore))
            aimPoint = aimHit.point;
        else
            aimPoint = aimRay.origin + aimRay.direction * shootRange;

        // 2️⃣ Namlu yönünden gerçek atış
        UnityEngine.Vector3 origin = muzzle ? muzzle.position : cam.transform.position;
        UnityEngine.Vector3 dir = (aimPoint - origin).normalized;

        // Namlu çok tersse düzelt
        if (muzzle)
        {
            float dot = UnityEngine.Vector3.Dot(muzzle.forward, dir);
            if (dot < 0.1f) dir = muzzle.forward;
        }

        // 3️⃣ Vuruş kontrolü
        if (Physics.Raycast(origin, dir, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Düşman vurulduysa hasar uygula
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                    enemy.TakeDamage(50); // 50 hasar
            }

            // Debug.DrawLine(origin, hit.point, Color.green, 0.25f);
        }
        else
        {
            // Debug.DrawRay(origin, dir * shootRange, Color.red, 0.25f);
        }

        if (muzzleFlash)
        {
            muzzleFlash.gameObject.SetActive(true);
            muzzleFlash.Play();
        }
        if (shotAudio)
            shotAudio.Play();
    }

    // Ateş anında kameraya doğru dön
    void FaceCameraYaw(float turnSpeed = 20f)
    {
        var cam = Camera.main;
        if (!cam) return;
        UnityEngine.Vector3 flatFwd = UnityEngine.Vector3.Scale(cam.transform.forward, new UnityEngine.Vector3(1, 0, 1)).normalized;
        if (flatFwd.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(flatFwd, UnityEngine.Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
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