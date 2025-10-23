using UnityEngine;
using TMPro;

public class HeartCollect : MonoBehaviour
{
    public int hearts = 0;
    public TMP_Text heartText;
    public DoorSlideController rightDoor;  // sağ kapı
    public DoorSlideController leftDoor;   // sol kapı
    public int heartsNeeded = 10;

    void Start()
    {
        UpdateText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heart"))
        {
            hearts++;
            Destroy(other.gameObject);
            UpdateText();

            if (hearts >= heartsNeeded)
            {
                rightDoor?.OpenDoor();
                leftDoor?.OpenDoor();
            }
        }
    }

    void UpdateText()
    {
        if (heartText != null)
            heartText.text = "PUAN: " + hearts + "/" + heartsNeeded;
    }
}
