using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public Transform cam;  // Inspector’dan Main Camera’yı sürükleyebilirsin

    CharacterController controller;
    UnityEngine.Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
    }

    void Update()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");   // A/D, Sol/Sağ
        float v = Input.GetAxis("Vertical");     // W/S, İleri/Geri
        bool sprint = Input.GetKey(KeyCode.LeftShift);

        // Kameraya göre yönlü hareket
        UnityEngine.Vector3 forward = cam ? UnityEngine.Vector3.ProjectOnPlane(cam.forward, UnityEngine.Vector3.up).normalized : transform.forward;
        UnityEngine.Vector3 right = cam ? UnityEngine.Vector3.ProjectOnPlane(cam.right, UnityEngine.Vector3.up).normalized : transform.right;

        UnityEngine.Vector3 move = (forward * v + right * h);
        if (move.sqrMagnitude > 1f) move.Normalize();

        float speed = sprint ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // Yürüme yönüne bak
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
           // transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // Zıplama
        if (grounded && Input.GetButtonDown("Jump"))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Yerçekimi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
