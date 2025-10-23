using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 100;
    public GameObject heartPrefab;
    public bool destroyInstantly = true; // 🔹 vurunca hemen yok olmasını istiyorsan true
    public float destroyDelay = 0.2f;    // 🔹 eğer animasyon oynasın istiyorsan 0.2–0.5 arası gecikme

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 🔹 Ölüm animasyonu tetikle (isteğe bağlı)
        if (anim != null)
            anim.SetTrigger("death");

        // 🔹 Kalp objesini oluştur (pickup)
        if (heartPrefab != null)
            Instantiate(heartPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        // 🔹 Zombiyi sahneden kaldır
        if (destroyInstantly)
        {
            Destroy(gameObject); // anında yok et
        }
        else
        {
            Destroy(gameObject, destroyDelay); // çok kısa gecikme (anim için)
        }
    }
}