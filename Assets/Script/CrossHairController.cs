using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair; // Reticle objesi
    public Camera mainCam;          // Ana kamera

    void Start()
    {
        if (!crosshair) crosshair = GetComponent<RectTransform>();
        if (!mainCam) mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Nişangah ekran merkezinde sabit kalır
        crosshair.anchoredPosition = Vector2.zero;
    }

    // Ateş etmek veya ray atmak için kullanılacak
    public Ray GetAimRay()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        return mainCam.ScreenPointToRay(screenCenter);
    }
}