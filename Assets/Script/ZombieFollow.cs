//using System.Numerics;
using UnityEngine;

public class ZombieFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float chaseRange = 10f;
    public float attackRange = 1.5f;
    public float rotationSpeed = 5f;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!player) return;

        float distance = UnityEngine.Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange && distance > attackRange)
        {
            // Oyuncuya doğru yürü
            UnityEngine.Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new UnityEngine.Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            anim?.SetBool("isWalking", true);
        }
        else if (distance <= attackRange)
        {
            // Saldırı
            anim?.SetTrigger("attack");
        }
        else
        {
            anim?.SetBool("isWalking", false);
        }
    }
}