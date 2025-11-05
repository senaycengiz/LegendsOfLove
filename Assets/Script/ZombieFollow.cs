using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform player;
    private PlayerHealth playerHealth;
    private Animator anim;

    [Header("Hareket Ayarları")]
    public float moveSpeed = 2.8f;          // Yürüme hızı
    public float rotationSpeed = 8f;        // Dönme hızı
    public float chaseRange = 30f;          // Görüş mesafesi
    public float attackRange = 2f;          // Saldırı mesafesi
    public float attackCooldown = 1.5f;     // Saldırı aralığı

    [Header("Zemin Kontrolü")]
    public LayerMask groundLayer;           // Sadece zemin katmanı
    public float groundCheckDistance = 1.5f;  // Yüksek platformlar için artırıldı

    [Header("Saldırı Ayarları")]
    public float damage = 10f;

    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Oyuncu referansını al
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Altında zemin yoksa hareket etme
        if (!IsGrounded())
        {
            anim?.SetBool("isWalking", false);
            return;
        }

        // Oyuncu görüş mesafesindeyse
        if (distance <= chaseRange && distance > attackRange)
        {
            ChasePlayer();
        }
        // Saldırı mesafesindeyse
        else if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            anim?.SetBool("isWalking", false);
        }
    }

    void ChasePlayer()
    {
        // Oyuncuya doğru bak
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);

        // İleri doğru ilerle
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        anim?.SetBool("isWalking", true);
    }

    void AttackPlayer()
    {
        anim?.SetBool("isWalking", false);

        if (Time.time - lastAttackTime > attackCooldown)
        {
            anim?.SetTrigger("attack");
            lastAttackTime = Time.time;
            isAttacking = true;

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Invoke(nameof(StopAttack), 0.8f);
        }
    }

    void StopAttack()
    {
        isAttacking = false;
    }

    // Güçlendirilmiş zemin kontrolü (her katta çalışır)
    bool IsGrounded()
    {
        float checkDist = groundCheckDistance;
        Vector3 origin = transform.position + Vector3.up * 0.3f;

        // üç noktadan ray gönder (ön, arka, orta)
        Vector3[] points = new Vector3[]
        {
            origin,
            origin + transform.forward * 0.4f,
            origin - transform.forward * 0.4f
        };

        foreach (Vector3 p in points)
        {
            if (Physics.Raycast(p, Vector3.down, out RaycastHit hit, checkDist, groundLayer))
            {
                // Yalnızca yatay yüzeyleri kabul et (duvar değil)
                if (Vector3.Angle(hit.normal, Vector3.up) < 30f)
                    return true;
            }
        }

        return false;
    }

    // Scene görünümünde Chase/Attack mesafesi çiz
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}