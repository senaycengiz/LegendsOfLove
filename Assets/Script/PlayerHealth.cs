using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float targetHealth;              // yavaş azalış için hedef değer
    public float smoothSpeed = 3f;           // azalma hızı 

    [Header("UI")]
    public Image healthBar;                  // dolan çubuk 
    public GameObject deathPanel;            // “Game Over” paneli
    public Animator anim;                    // Ölüm animasyonu 

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        targetHealth = maxHealth;
        UpdateHealthUI(true);
    }

    void Update()
    {
        if (Mathf.Abs(currentHealth - targetHealth) > 0.01f)
        {
            currentHealth = Mathf.Lerp(currentHealth, targetHealth, Time.deltaTime * smoothSpeed);
            UpdateHealthUI();
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        targetHealth -= amount;
        targetHealth = Mathf.Clamp(targetHealth, 0, maxHealth);

        if (targetHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void UpdateHealthUI(bool instant = false)
    {
        if (healthBar == null) return;

        float fill = currentHealth / maxHealth;
        healthBar.fillAmount = fill;

        // can azaldıkça renk geçişi 
        Color barColor = Color.Lerp(Color.red, Color.green, fill);
        healthBar.color = barColor;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("💀 Oyuncu öldü!");
        if (anim) anim.SetTrigger("Die");

        //  Game Over panelini açar
        if (deathPanel) deathPanel.SetActive(true);

        // hareketi devre dışı bırakır
        var moveScript = GetComponent<PlayerC>();
        if (moveScript) moveScript.enabled = false;
    }
}