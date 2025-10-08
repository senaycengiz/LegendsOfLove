using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerUnifiedController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                       // Main Camera
    public Transform muzzle;                 // (ops) Silah/Muzzle
    public ParticleSystem muzzleFx;          // (ops)
    public AudioSource fireSfx;              // (ops)

    [Header("Speeds")]
    public float walkSpeed = 4f;             // A/D/W/S
    public float sprintSpeed = 7f;           // Shift basılı
    public float strafeSpeed = 3.5f;         // Aim açıkken WASD

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.1f;
    public float gravity = -9.81f;
    public LayerMask groundMask;             // zeminin layer’ları (Default/Obstacle)
    public float groundProbeRadius = 0.24f;  // ayak altı küre
    public float groundProbeOffset = 0.03f;  // kapsül tabanının az üstü
    public float coyoteTime = 0.12f;         // yerden ayrıldıktan sonra tolerans
    public float jumpBufferTime = 0.10f;     // space’i erken basma toleransı

    CharacterController cc;
    Animator anim;
    Vector3 vel;                              // dikey hız
    int upperLayer; float aimWeight;
    float coyoteCounter, jumpBufferCounter;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        if (!cam) cam = Camera.main;
        upperLayer = anim.GetLayerIndex("UpperBody"); // Animator layer adı birebir olmalı
    }

    void Update()
    {
        // -------- INPUT --------
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        bool aiming = Input.GetMouseButton(1);              // sağ tık
        if (Input.GetKeyDown(KeyCode.Space))                // jump buffer
            jumpBufferCounter = jumpBufferTime;

        anim.SetBool("Aim", aiming);

        // -------- KAMERA EKSENLERİ --------
        Vector3 cf = cam.transform.forward; cf.y = 0; cf.Normalize();
        Vector3 cr = cam.transform.right; cr.y = 0; cr.Normalize();
        Vector3 moveDir = (cf * v + cr * h);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // -------- HAREKET (yatay) + ANIM PARAM --------
        Vector3 horizontal = Vector3.zero;
        if (!aiming)
        {
            // Serbest koşu (Locomotion / Speed)
            float input01 = Mathf.Clamp01(new Vector2(h, v).magnitude);
            float speed01 = input01 * (sprint ? 1f : 0.5f);     // 0=Idle, 0.5=Walk, 1=Run
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), speed01, 10f * Time.deltaTime));
            anim.SetFloat("MoveX", 0f); anim.SetFloat("MoveY", 0f);

            float speed = sprint ? sprintSpeed : walkSpeed;
            horizontal = moveDir * speed;

            // gittiğin yöne dön
            if (input01 > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 12f * Time.deltaTime);
        }
        else
        {
            // Aim strafe (Strafe / MoveX,MoveY) + kameraya bak
            if (cf.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cf), 15f * Time.deltaTime);

            anim.SetFloat("MoveX", h, 0.1f, Time.deltaTime);
            anim.SetFloat("MoveY", v, 0.1f, Time.deltaTime);
            anim.SetFloat("Speed", 0f);

            horizontal = moveDir * strafeSpeed;
        }

        // -------- GROUND CHECK + JUMP (coyote+buffer) --------
        bool grounded = GroundedCheck() || cc.isGrounded;
        if (grounded) coyoteCounter = coyoteTime;
        else coyoteCounter = Mathf.Max(0, coyoteCounter - Time.deltaTime);

        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        if (coyoteCounter > 0f && jumpBufferCounter > 0f)
        {
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            coyoteCounter = 0f;
            jumpBufferCounter = 0f;
        }

        if (grounded && vel.y < 0f) vel.y = -2f;           // zemine yapıştır
        vel.y += gravity * Time.deltaTime;                 // yerçekimi

        // -------- MOVE (tek Move çağrısı) --------
        Vector3 move = horizontal * Time.deltaTime + Vector3.up * vel.y * Time.deltaTime;
        cc.Move(move);

        // -------- ÜST GÖVDE LAYER AĞIRLIĞI --------
        if (upperLayer >= 0)
        {
            aimWeight = Mathf.Lerp(aimWeight, aiming ? 1f : 0f, 10f * Time.deltaTime);
            anim.SetLayerWeight(upperLayer, aimWeight);
        }
        anim.SetBool("IsGrounded", grounded);

        // -------- (OPS) ATEŞ --------
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Shoot");
            if (muzzleFx) muzzleFx.Play();
            if (fireSfx) fireSfx.Play();
            // Raycast ile vurmak istersen buraya ekleyebilirsin
        }
    }

    // Ayak altında küresel ground-check
    bool GroundedCheck()
    {
        Vector3 centerWorld = transform.position + cc.center;
        float down = (cc.height * 0.5f) - cc.radius + groundProbeOffset;
        Vector3 feet = centerWorld + Vector3.down * down;
        return Physics.CheckSphere(feet, groundProbeRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    // Probu sahnede görmek için
    void OnDrawGizmosSelected()
    {
        if (!TryGetComponent<CharacterController>(out var c)) return;
        Vector3 centerWorld = transform.position + c.center;
        float down = (c.height * 0.5f) - c.radius + groundProbeOffset;
        Vector3 feet = centerWorld + Vector3.down * down;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(feet, groundProbeRadius);
    }
}