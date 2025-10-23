using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float chaseRange = 10f;
    public float attackRange = 1.5f;
    public float rotationSpeed = 5f;
    public float damage = 10f;            // 🔹 her saldırıda verilecek hasar
    public float attackCooldown = 1.5f;   // 🔹 saldırı aralığı (saniye)

    private Animator anim;
    private float lastAttackTime;
    private PlayerHealth playerHealth;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player) playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!player) return;

        float distance = UnityEngine.Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange && distance > attackRange)
        {
            // Oyuncuya doğru yürü
            UnityEngine.Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new UnityEngine.Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            anim?.SetBool("isWalking", true);
        }
        else if (distance <= attackRange)
        {
            // Saldırı
            anim?.SetBool("isWalking", false);
            if (Time.time - lastAttackTime > attackCooldown)
            {
                anim?.SetTrigger("attack");
                lastAttackTime = Time.time;
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
        else
        {
            anim?.SetBool("isWalking", false);
        }
    }
}
