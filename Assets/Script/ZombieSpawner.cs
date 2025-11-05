using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombi Ayarları")]
    public GameObject zombiePrefab;
    public int zombieCount = 10;               // Toplam zombi sayısı
    public float spawnRadius = 60f;            // Oyun alanı yarıçapı
    public float minZombieSpacing = 5f;        // Zombiler arası minimum mesafe
    public float playerSafeDistance = 8f;      // Oyuncuya minimum uzaklık
    public Transform player;                   // Oyuncu referansı

    [Header("Katmanlar ve Filtreler")]
    public LayerMask groundLayer;              // Zemin katmanı
    public LayerMask noSpawnMask;              // Prensin olduğu "NoSpawn" katmanı
    public float noSpawnCheckRadius = 1f;      // NoSpawn kontrol yarıçapı

    [Header("Yükseklik Kontrolü (Opsiyonel)")]
    public float maxSpawnY = 8.5f;             // Üst kat eşiği – bundan yüksekse doğma (prens alanı)
    public float rayHeight = 80f;              // Yukarıdan ışın gönderme yüksekliği

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        // Her oyun başında farklı random üretim
        Random.InitState(System.Environment.TickCount);

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        SpawnZombies();
    }

    void SpawnZombies()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < zombieCount && attempts < zombieCount * 800)
        {
            Vector3 pos = FindValidGround();

            if (pos == Vector3.zero)
            {
                attempts++;
                continue;
            }

            // Oyuncuya çok yakın olmasın
            if (Vector3.Distance(pos, player.position) < playerSafeDistance)
            {
                attempts++;
                continue;
            }

            // Diğer zombilere çok yakın olmasın
            bool tooClose = false;
            foreach (Vector3 used in usedPositions)
            {
                if (Vector3.Distance(pos, used) < minZombieSpacing)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
            {
                attempts++;
                continue;
            }

            // Zombiyi oluştur
            Instantiate(zombiePrefab, pos, Quaternion.identity);
            usedPositions.Add(pos);
            spawned++;
            attempts++;
        }

        Debug.Log($"{spawned} zombi güvenli zeminlerde doğdu ({attempts} denemede).");
    }

    Vector3 FindValidGround()
    {
        for (int i = 0; i < 400; i++)
        {
            // Rastgele dünya konumu üret
            Vector3 origin = new Vector3(
                transform.position.x + Random.Range(-spawnRadius, spawnRadius),
                transform.position.y + rayHeight,
                transform.position.z + Random.Range(-spawnRadius, spawnRadius)
            );

            // Yukarıdan aşağı raycast
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayHeight * 2f, groundLayer))
            {
                Vector3 pos = hit.point + Vector3.up * 0.2f;

                // Eğimli yüzeyleri atla (duvar, rampa, sütun)
                if (Vector3.Angle(hit.normal, Vector3.up) > 15f)
                    continue;

                // Prens katı (üst kat) tespiti
                if (pos.y >= maxSpawnY)
                    continue;

                // NoSpawn hacmi (örneğin prens bölgesi) içinde mi?
                Collider[] noSpawnHits = Physics.OverlapSphere(
                    pos + Vector3.up * 0.5f,
                    noSpawnCheckRadius,
                    noSpawnMask,
                    QueryTriggerInteraction.Collide
                );
                if (noSpawnHits != null && noSpawnHits.Length > 0)
                    continue;

                // Duvar veya kolon isimli collider’ları atla
                string nameLower = hit.collider.name.ToLower();
                if (nameLower.Contains("wall") || nameLower.Contains("column") || nameLower.Contains("pillar"))
                    continue;

                return pos;
            }
        }

        return Vector3.zero;
    }

    // Scene görünümünde spawn alanını görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}