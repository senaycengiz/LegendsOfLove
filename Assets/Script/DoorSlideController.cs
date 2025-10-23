using UnityEngine;
using System.Collections;

public class DoorSlideController : MonoBehaviour
{
    [Header("Kapı Ayarları")]
    public UnityEngine.Vector3 slideOffset = new UnityEngine.Vector3(1.5f, 0, 0); // Kapı ne kadar yana kaysın
    public float slideDuration = 1.5f;                    // Kaç saniyede açılsın
    private UnityEngine.Vector3 closedPos;
    private UnityEngine.Vector3 openPos;
    private bool isOpen = false;

    [Header("Collider ve Ses Ayarları")]
    public Collider doorCollider; // FinalDoor'un BoxCollider'ı
    public AudioClip openSound;
    public AudioSource audioSource;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + slideOffset;
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(MoveDoor());

        if (doorCollider != null)
            doorCollider.enabled = false;

        if (openSound != null && audioSource != null)
            audioSource.PlayOneShot(openSound);
    }

    IEnumerator MoveDoor()
    {
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            transform.position = UnityEngine.Vector3.Lerp(closedPos, openPos, t / slideDuration);
            yield return null;
        }
        transform.position = openPos;
    }
}
