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
    public GameObject congratsPanel;         // (artık kullanılmayacak ama yedekte kalabilir)

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
        // Prens sürekli oyuncuya dönsün istiyorsak
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
                Debug.Log("⚠️ Prense ulaşmak için yeterli kalp yok!");
            }
        }
    }

    void DoRescue()
    {
        rescued = true;

        // 1️⃣ Animasyon
        if (anim) anim.SetTrigger("rescued");

        // 2️⃣ VFX / Ses efektleri
        if (celebrationVFX) celebrationVFX.SetActive(true);
        if (voiceLine) voiceLine.Play();

        // 3️⃣ YOU WIN ekranı
        VictoryManager victory = FindObjectOfType<VictoryManager>();
        if (victory != null)
        {
            victory.ShowWinScreen();
            Debug.Log("🎉 YOU WIN ekranı gösterildi!");
        }
        else
        {
            Debug.LogWarning("⚠️ VictoryManager sahnede bulunamadı! GameManager objesine eklemen gerekiyor.");
        }

        // 4️⃣ (Opsiyonel) Oyuncu hareketini durdurmak istersen:
        var controller = player.GetComponent<PlayerC>();
        if (controller) controller.enabled = false;
    }

    // (İsteğe bağlı kısa sinematik)
    /*
    IEnumerator BriefCinematic()
    {
        var controller = player.GetComponent<PlayerC>();
        if (controller) controller.enabled = false;

        yield return new WaitForSeconds(2.5f);

        if (controller) controller.enabled = true;
    }
    */
}