using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuReset : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Menüde imleç serbest kalır
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Sahnede tam 1 tane EventSystem olduğundan emin olmak için
        var systems = FindObjectsOfType<EventSystem>();
        if (systems.Length == 0)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        else
            for (int i = 1; i < systems.Length; i++) Destroy(systems[i].gameObject); // fazlaları siler

        // Arka plan Image'ları tıklamayı engellemesin diye
        foreach (var img in FindObjectsOfType<Image>(true))
        {
            if (img.GetComponent<Button>() == null &&
                img.GetComponent<Scrollbar>() == null &&
                img.GetComponent<Toggle>() == null)
            {
                img.raycastTarget = false; 
            }
        }

        foreach (var canvas in FindObjectsOfType<Canvas>())
            if (canvas.GetComponent<GraphicRaycaster>() == null)
                canvas.gameObject.AddComponent<GraphicRaycaster>();
    }
}