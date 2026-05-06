using RayWenderlich.Unity.StatePatternInUnity;
using UnityEngine;

public class MonsterHitBox : MonoBehaviour
{
    private Monster monster; // Reference to the monster script
    private BoxCollider hitBox; // The hitbox collider for the monster's attack

    private bool attackFinished;
    private float damage; // Damage dealt by the monster's attack
    
    void Start()
    {
        hitBox = GetComponent<BoxCollider>(); // Get the BoxCollider component attached to this GameObject
        monster = GetComponent<Monster>(); // Get the Monster component attached to this GameObject
        hitBox.enabled = false; // Initially disable the hitbox
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableAttack()
    {
        hitBox.enabled = true; // Enable the hitbox for the monster's attack
        attackFinished = false; // Reset the flag to allow new attacks
    }

    public void DisableAttack()
    {
        hitBox.enabled = false; // Disable the hitbox after the attack
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
                damage = monster.DamagePerHit; // Use the monster's damage property
                player.TakeDamage(damage);
                attackFinished = true; // prevent repeated hits from same swing
            }
        }
    }
}
