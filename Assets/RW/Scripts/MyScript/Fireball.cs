using UnityEngine;

public class Fireball : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SphereCollider sphereCollider;
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var npc = other.GetComponent<Creature>();
            if (npc != null)
            {
                npc.TakeDamage(10);
            }
        }
        if (other.CompareTag("Monster"))
        {
            var npc = other.GetComponent<Monster>();
            if (npc != null)
            {
                npc.TakeDamage(10);
            }
        }
    }
}
