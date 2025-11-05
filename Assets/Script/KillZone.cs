using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("KillZone tetiklendi!");

            // Oyuncunun PlayerHealth script’ini bulur
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null && playerHealth.deathPanel != null)
            {
                // Game Over panelini açar
                playerHealth.deathPanel.SetActive(true);
                Debug.Log("Game Over paneli aktif edildi.");
            }

            // Oyunu dondurur
            Time.timeScale = 0f;
        }
    }
}