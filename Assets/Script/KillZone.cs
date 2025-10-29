using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("⚠️ KillZone tetiklendi!");

            // Oyuncunun PlayerHealth script’ini bul
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null && playerHealth.deathPanel != null)
            {
                // Game Over panelini aç
                playerHealth.deathPanel.SetActive(true);
                Debug.Log("💀 Game Over paneli aktif edildi.");
            }

            // Oyunu dondur
            Time.timeScale = 0f;
        }
    }
}