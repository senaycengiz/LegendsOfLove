using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    public string gameSceneName = "LOL1";
    private SceneFader fader;

    void Start()
    {
        fader = FindObjectOfType<SceneFader>();
    }

    public void PlayGame()
    {
        if (fader != null)
            StartCoroutine(fader.FadeAndLoadScene(gameSceneName));
        else
            SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}