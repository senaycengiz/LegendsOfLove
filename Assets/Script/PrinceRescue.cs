using UnityEngine;
using TMPro;

public class PrinceRescue : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // Player transformu
    public HeartCollect heartCollect;        // Player'daki HeartCollect
    public Animator anim;

    [Header("Gameplay")]
    public int requiredHearts = 10;
    public bool facePlayer = true;
    public float faceTurnSpeed = 5f;

    private bool rescued = false;

    void Reset()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Prens prensese dönsün
        if (facePlayer && !rescued && player)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, faceTurnSpeed * Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (rescued) return;

        // Player temas etti mi?
        if (other.CompareTag("Player"))
        {
            // Player referansı ve HeartCollect 
            if (!player) player = other.transform;
            if (!heartCollect) heartCollect = other.GetComponent<HeartCollect>();

            // Kalp sayısını kontrol
            if (heartCollect != null && heartCollect.hearts >= requiredHearts)
            {
                DoRescue();
            }
            else
            {
                Debug.Log("Prense ulaşmak için yeterli kalp yok!");
            }
        }
    }

    void DoRescue()
    {
        rescued = true;

        // Animasyon
        if (anim) anim.SetTrigger("rescued");


        //  YOU WIN ekranı
        VictoryManager victory = FindObjectOfType<VictoryManager>();
        if (victory != null)
        {
            victory.ShowWinScreen();
            Debug.Log(" YOU WIN ekranı gösterildi!");
        }
        else
        {
            Debug.LogWarning(" VictoryManager sahnede bulunamadı! GameManager objesine eklemen gerekiyor.");
        }

        // oyuncu hareketini durdurmak:
        var controller = player.GetComponent<PlayerC>();
        if (controller) controller.enabled = false;
    }


}