using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair; 
    public Camera mainCam;         

    void Start()
    {
        if (!crosshair) crosshair = GetComponent<RectTransform>();
        if (!mainCam) mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
    
        crosshair.anchoredPosition = Vector2.zero;
    }

    
    public Ray GetAimRay()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        return mainCam.ScreenPointToRay(screenCenter);
    }
}