using UnityEngine;
public class EnemyDummy : MonoBehaviour
{
    Renderer r; void Awake() { r = GetComponent<Renderer>(); }
    public void TakeHit(int amount) { if (r) r.material.color = Color.red; }
}
