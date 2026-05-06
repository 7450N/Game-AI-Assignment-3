using UnityEngine;
using RayWenderlich.Unity.StatePatternInUnity;
using Unity.Behavior;


public class Sword : MonoBehaviour
{
    public BoxCollider weaponCollider; // Collider for the sword's hitbox   
    private bool hasHitThisSwing;     // Prevent multiple hits per swing

    private void Start()
    {
        weaponCollider.enabled = false; // Off by default
    }

    public void EnableAttack()
    {
        weaponCollider.enabled = true;
        hasHitThisSwing = false; // reset hit tracking
    }

    public void DisableAttack()
    {
        weaponCollider.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (!weaponCollider.enabled) return; // only active during attack window
        if (hasHitThisSwing) return; // already hit something this swing

        if (other.CompareTag("Enemy"))
        {
            var npc = other.GetComponent<Creature>();
            if (npc != null)
            {
                npc.TakeDamage(10);
                hasHitThisSwing = true; // prevent repeated hits from same swing
            }
        }
        if (other.CompareTag("Monster"))
        {   
            var npc = other.GetComponent<Monster>();
            if (npc != null)
            {
                npc.TakeDamage(10);
                hasHitThisSwing = true; // prevent repeated hits from same swing
            }
        }
    }
}

