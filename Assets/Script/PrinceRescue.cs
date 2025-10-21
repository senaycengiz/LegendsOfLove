using UnityEngine;
using TMPro;

public class PrinceRescue : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // Player transformu
    public HeartCollect heartCollect;        // Player'daki HeartCollect
    public Animator anim;                    // Prince Animator
    public GameObject celebrationVFX;        // (ops.) kalpler veya ışık efekti
    public AudioSource voiceLine;            // (ops.) kısa konuşma/teşekkür sesi
    public GameObject congratsPanel;         // (ops.) UI Panel "Tebrikler"

    [Header("Gameplay")]
    public int requiredHearts = 10;
    public bool facePlayer = true;
    public float faceTurnSpeed = 5f;

    bool rescued = false;

    void Reset()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // İstersen prens sürekli oyuncuya yüzünü dönebilsin
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
            // Player referansı ve HeartCollect yoksa bul
            if (!player) player = other.transform;
            if (!heartCollect) heartCollect = other.GetComponent<HeartCollect>();

            // Kalp sayısını kontrol et
            if (heartCollect != null && heartCollect.hearts >= requiredHearts)
            {
                DoRescue();
            }
            else
            {
                // Yeterli kalp yoksa küçük bir ipucu göstermek istersen:
                // (World-space Canvas üzerindeki TMP_Text'i doldurabilirsin)
                // Debug.Log("Prense ulaşmak için yeterli kalp yok!");
            }
        }
    }

    void DoRescue()
    {
        rescued = true;

        // 1) Animasyon
        if (anim) anim.SetTrigger("rescued");

        // 2) VFX / Ses
        if (celebrationVFX) celebrationVFX.SetActive(true);
        if (voiceLine) voiceLine.Play();

        // 3) UI Panel (Tebrikler – Sevgi her engeli aşar ❤️)
        if (congratsPanel) congratsPanel.SetActive(true);

        // 4) İstersen oyuncu kontrolünü kısa süre kilitleyebilirsin
        // StartCoroutine(BriefCinematic());
    }

    // Örnek mini sinematik (opsiyonel)
    /*
    IEnumerator BriefCinematic()
    {
        // Player hareketini kilitle
        var controller = player.GetComponent<PlayerC>();
        if (controller) controller.enabled = false;

        yield return new WaitForSeconds(2.5f);

        if (controller) controller.enabled = true;
    }
    */
}