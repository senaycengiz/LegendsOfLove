using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerC : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 4.5f;        // normal hız
    public float sprintMultiplier = 1.6f; // Shift basılıyken 
    public float rotationSpeed = 12f;

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
    }

    void Update()
    {
    
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f);

        // Kameraya göre yön
        Transform cam = Camera.main ? Camera.main.transform : transform;
        Vector3 camF = Vector3.Scale(cam.forward, new Vector3(1f, 0f, 1f)).normalized;
        Vector3 camR = cam.right;
        Vector3 moveDir = (camF * input.z + camR * input.x).normalized;

        // Zemin
        bool grounded = usePhysicsGroundCheck ? PhysicsGrounded() : cc.isGrounded;
        groundedTimer -= Time.deltaTime;
        if (grounded) groundedTimer = groundedRemember;

        // Zıplama 
        if (Input.GetKeyDown(KeyCode.Space) && groundedTimer > 0f)
        {
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("Jump"); 
        }

        // Yer çekimi
        if (grounded && yVel < 0f) yVel = -2f;
        yVel += gravity * Time.deltaTime;

        // hızlı koşu
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= sprintMultiplier;

        
        Vector3 velocity = moveDir * speed + Vector3.up * yVel;
        cc.Move(velocity * Time.deltaTime);

        // Yönlendirme
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }

        float planarSpeed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        anim.SetFloat("Speed", planarSpeed);    
        anim.SetBool("IsGrounded", grounded);

        // Ateş (sol tık)
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Fire");
            FaceCameraYaw();  
            ShootRay();       
        }
    }


    void ShootRay()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        // KAMERA MERKEZİNDEN hedef noktayı alır
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray aimRay = cam.ScreenPointToRay(screenCenter);

        Vector3 aimPoint;
        int aimMask = ~0; 
        if (Physics.Raycast(aimRay, out RaycastHit aimHit, shootRange, aimMask, QueryTriggerInteraction.Ignore))
            aimPoint = aimHit.point;
        else
            aimPoint = aimRay.origin + aimRay.direction * shootRange;

      
        Vector3 origin = muzzle ? muzzle.position : cam.transform.position;
        Vector3 dir = (aimPoint - origin).normalized;

    
        if (muzzle)
        {
            float dot = Vector3.Dot(muzzle.forward, dir);
            if (dot < 0.1f) dir = muzzle.forward;
        }

        if (Physics.Raycast(origin, dir, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            hit.collider.SendMessage("TakeHit", 10, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
        }

        if (muzzleFlash) { muzzleFlash.gameObject.SetActive(true); muzzleFlash.Play(); }
        if (shotAudio) shotAudio.Play();
    }

    // ateş anında karakteri kameranın yatay yönüne baktırır
    void FaceCameraYaw(float turnSpeed = 20f)
    {
        var cam = Camera.main;
        if (!cam) return;
        Vector3 flatFwd = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        if (flatFwd.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(flatFwd, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
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