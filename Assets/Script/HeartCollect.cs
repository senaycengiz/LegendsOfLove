using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HeartCollect : MonoBehaviour
{

    public int hearts = 0;
    public TMP_Text heartText;
    public GameObject finalDoor;

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

            if (hearts >= 10)
                finalDoor.SetActive(false); // kapıyı aç
        }
    }

    void UpdateText()
    {
        if (heartText != null)
            heartText.text = "❤️ " + hearts + "/10";
    }
}