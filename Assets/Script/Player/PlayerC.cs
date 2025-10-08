using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerC : MonoBehaviour   // <= DOSYA ADI PlayerC.cs ise böyle kalmalı
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
    public LayerMask hitMask;               // Enemy
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
        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f);

        // --- 2) Kameraya göre yön
        Transform cam = Camera.main ? Camera.main.transform : transform;
        Vector3 camF = Vector3.Scale(cam.forward, new Vector3(1f, 0f, 1f)).normalized;
        Vector3 camR = cam.right;
        Vector3 moveDir = (camF * input.z + camR * input.x).normalized;

        // --- 3) Zemin
        bool grounded = usePhysicsGroundCheck ? PhysicsGrounded() : cc.isGrounded;
        groundedTimer -= Time.deltaTime;
        if (grounded) groundedTimer = groundedRemember;

        // --- 4) Zıplama (Space)
        if (Input.GetKeyDown(KeyCode.Space) && groundedTimer > 0f)
        {
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity); // v = sqrt(h * -2g)
            anim.SetTrigger("Jump"); // Animator'da Jump trigger'ı varsa
        }

        // --- 5) Yer çekimi
        if (grounded && yVel < 0f) yVel = -2f;
        yVel += gravity * Time.deltaTime;

        // --- 6) Sprint (Shift)  // *** DÜZELTME: bu kısmı YORUMDAN ÇIKAR ***
        float speed = moveSpeed;                                           // FIX: tanımlı
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= sprintMultiplier;

        // --- 7) Hareket uygula
        Vector3 velocity = moveDir * speed + Vector3.up * yVel;
        cc.Move(velocity * Time.deltaTime);

        // --- 8) Yönlendirme
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }

        // --- 9) Animator besleme
        float planarSpeed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        anim.SetFloat("Speed", planarSpeed);     // Blend Tree (Idle/Walk/Run) bunu kullanıyor
        anim.SetBool("IsGrounded", grounded);

        // --- 10) Ateş (sol tık)
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Fire");
            ShootRay();
        }
    }

    void ShootRay()
    {
        // *** DÜZELTME: Screen.height büyük 'S' ile olmalı ***
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            hit.collider.SendMessage("TakeHit", 10, SendMessageOptions.DontRequireReceiver);
        }

        if (muzzleFlash) { muzzleFlash.gameObject.SetActive(true); muzzleFlash.Play(); }
        if (shotAudio) shotAudio.Play();
    }

    bool PhysicsGrounded()
    {
        Vector3 baseCenter = transform.position + cc.center + Vector3.down * (cc.height / 2f - cc.radius + groundCheckOffset);
        return Physics.CheckSphere(baseCenter, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        if (!usePhysicsGroundCheck || cc == null) return;
        Gizmos.color = Color.cyan;
        Vector3 baseCenter = transform.position + cc.center + Vector3.down * (cc.height / 2f - cc.radius + groundCheckOffset);
        Gizmos.DrawWireSphere(baseCenter, groundCheckRadius);
    }
}