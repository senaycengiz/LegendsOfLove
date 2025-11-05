using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 100;
    public GameObject heartPrefab;
    public bool destroyInstantly = true;  
    public float destroyDelay = 0.2f;    
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

        
        if (destroyInstantly)
        {
            Destroy(gameObject); 
        }
        else
        {
            Destroy(gameObject, destroyDelay); 
        }
    }
}