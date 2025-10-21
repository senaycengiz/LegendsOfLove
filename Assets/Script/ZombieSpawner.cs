using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int zombieCount = 10;
    public float spawnRadius = 20f;

    void Start()
    {
        for (int i = 0; i < zombieCount; i++)
        {
            UnityEngine.Vector3 randomPos = transform.position + new UnityEngine.Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Instantiate(zombiePrefab, randomPos, Quaternion.identity);
        }
    }
}