using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int zombieCount = 10;
    public float spawnRadius = 20f;
    public float safeDistance = 6f; // 🔹 karakterden minimum uzaklık
    public Transform player;        // 🔹 Player referansı

    void Start()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        for (int i = 0; i < zombieCount; i++)
        {
            UnityEngine.Vector3 randomPos;
            int attempts = 0;
            do
            {
                randomPos = transform.position + new UnityEngine.Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );
                attempts++;
            }
            while (UnityEngine.Vector3.Distance(randomPos, player.position) < safeDistance && attempts < 20);

            Instantiate(zombiePrefab, randomPos, Quaternion.identity);
        }
    }
}