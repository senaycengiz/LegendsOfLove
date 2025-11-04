using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuReset : MonoBehaviour
{
    void Start()
    {
        // Oyun hızını ve sesi mutlaka normale al
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Menüde imleç serbest
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Sahnede tam 1 tane EventSystem olduğundan emin ol
        var systems = FindObjectsOfType<EventSystem>();
        if (systems.Length == 0)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        else
            for (int i = 1; i < systems.Length; i++) Destroy(systems[i].gameObject); // fazlaları sil

        // Arka plan Image'ları tıklamayı engellemesin
        foreach (var img in FindObjectsOfType<Image>(true))
        {
            if (img.GetComponent<Button>() == null &&
                img.GetComponent<Scrollbar>() == null &&
                img.GetComponent<Toggle>() == null)
            {
                img.raycastTarget = false; // buton dışındaki görüntüler raycast almasın
            }
        }

        // Canvas'ta Graphic Raycaster var mı (butonlar için şart)
        foreach (var canvas in FindObjectsOfType<Canvas>())
            if (canvas.GetComponent<GraphicRaycaster>() == null)
                canvas.gameObject.AddComponent<GraphicRaycaster>();
    }
}