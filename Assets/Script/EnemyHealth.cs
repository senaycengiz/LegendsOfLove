using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    public GameObject heartPrefab;
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

        if (anim != null)
            anim.SetTrigger("death");

        if (heartPrefab != null)
            Instantiate(heartPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        Destroy(gameObject, 2f);
    }
}