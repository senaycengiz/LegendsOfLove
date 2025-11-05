using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Win Screen UI")]
    public GameObject winPanel;

    public void ShowWinScreen()
    {
        if (winPanel)
        {
            winPanel.SetActive(true);
        }

        // Oyun dursun
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // İmleç serbest
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("🎉 YOU WIN ekranı açıldı!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}