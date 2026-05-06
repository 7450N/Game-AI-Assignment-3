using UnityEngine;
using RayWenderlich.Unity.StatePatternInUnity;


public class CreatureHitBox : MonoBehaviour
{
    public BoxCollider hitBox; // Collider for the sword's hitbox   
    private bool attackFinished;     // Prevent multiple hits per animation
    [SerializeField] private Creature creature; // Reference to the creature script
    private float damage;

    private void Start()
    {
        hitBox.enabled = false; // Off by default
    }

    public void EnableAttack()
    {
        hitBox.enabled = true;
        attackFinished = false; // reset hit tracking
    }

    public void DisableAttack()
    {
        hitBox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitBox.enabled) return; // only active during attack window
        if (attackFinished) return; // already hit something this swing

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Character>();
            if (player != null)
            {   
                if (creature.JumpAttackOccured)
                {
                    damage = creature.JumpAttackDamage; // Use jump attack damage
                }
                else
                {
                    damage = creature.NormalAttackDamage; // Use normal attack damage
                }
                player.TakeDamage(damage);
                attackFinished = true; // prevent repeated hits from same swing
            }
        }
    }
}

