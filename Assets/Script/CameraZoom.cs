using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineCamera cineCam;
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 8f;

    void Update()
    {
        if (cineCam == null) return;

        // ThirdPersonFollow bileşenini al - yeni API böyle
        CinemachineThirdPersonFollow follow =
            cineCam.GetComponent<CinemachineThirdPersonFollow>();

        if (follow != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                follow.CameraDistance = Mathf.Clamp(
                    follow.CameraDistance - scroll * zoomSpeed,
                    minDistance,
                    maxDistance
                );
            }
        }
    }
}
