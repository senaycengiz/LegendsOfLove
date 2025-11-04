using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;         // FadePanel'in Image component'i
    public float fadeDuration = 1.5f; // Kararma süresi (saniye)

    void Start()
    {
        // Menü başında panel tamamen şeffaf olsun
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 🔹 Sadece fade-out (kararma)
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Kısa bekleme efekti
        yield return new WaitForSecondsRealtime(0.3f);

        // 🔹 Yeni sahneye geç
        SceneManager.LoadScene(sceneName);
    }
}