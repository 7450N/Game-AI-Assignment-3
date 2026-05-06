using System.Collections;
using Unity.Behavior;
using UnityEngine;
using RayWenderlich.Unity.StatePatternInUnity; // Ensure you have the correct namespace for the Behavior Graph Agent

public class Monster : MonoBehaviour
{
    public float monsterHealth = 100f; // Health of the monster
    
    [SerializeField] private float attackRange = 2f; // Attack range of the monster
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private GameObject target;
    [SerializeField] private BehaviorGraphAgent bgAgent; // Reference to the Behavior Graph Agent
    private float damage;

    private MonsterHitBox hitBox; // Reference to the monster's hitbox script
    private Animator animator;

    private float playerHealth; // Health of the player (if needed for interactions)
    private bool isAttacking = false;

    public float DamagePerHit => damage; // Property to get the damage dealt by the monster

    
    void Start()
    {      
        animator = GetComponent<Animator>(); // Get the Animator component attached to this GameObject
        hitBox = GetComponent<MonsterHitBox>(); // Get the MonsterHitBox component attached to this GameObject
        bgAgent.BlackboardReference.SetVariableValue("Health", monsterHealth); // Corrected method name
    }

    // Update is called once per frame
    void Update()
    {   
        if (target == null) return; // If no target is set, exit the method
        playerHealth = target.GetComponent<Character>().PlayerHP; // Get the player's health from the Character script
        bgAgent.BlackboardReference.SetVariableValue("playerHealth", playerHealth); // Update the player's health in the blackboard
        Vector3 dir = target.transform.position - transform.position; // Calculate the direction from the bot to the player
        float distance = dir.magnitude; // Get the distance to the player
        dir.Normalize(); // Normalize the direction vector

        if (distance <= detectionRange && Vector3.Angle(transform.forward, dir) <= fieldOfView / 2f)
        {
            bgAgent.BlackboardReference.SetVariableValue("playerDetected", true); // Set the target in the blackboard
            if (distance <= attackRange)
            {
                bgAgent.BlackboardReference.SetVariableValue("playerInAttackRange", true); // Set the monster can attack
            }
            else
            {
                bgAgent.BlackboardReference.SetVariableValue("playerInAttackRange", false); // Reset the attack range variable
            }
        }
        else
        {
            bgAgent.BlackboardReference.SetVariableValue("playerDetected", false); // Reset the player detection variable
            bgAgent.BlackboardReference.SetVariableValue("playerInAttackRange", false); // Reset the attack range variable
        }
    }

    public void TakeDamage(float damage)
    {
        monsterHealth -= damage;
        bgAgent.BlackboardReference.SetVariableValue("Health", monsterHealth); // Update health in the blackboard
        bgAgent.BlackboardReference.SetVariableValue("gettingHit", true); // Update the blackboard to indicate the monster is getting hit
    }

    public void Attack()
    {   
        if (isAttacking) return; // Prevent multiple attacks at the same time
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        transform.LookAt(target.transform); // Look at the target
        int randomNum = Random.Range(0, 2); // Randomly choose an attack animation
        
        switch (randomNum)
        {
            case 0:
                animator.SetTrigger("Attack 01"); // Trigger attack animation
                damage = 5f;
                break;
            case 1:
                animator.SetTrigger("Attack 02"); // Trigger attack animation
                damage = 10f;
                break;
        }
        hitBox.EnableAttack(); // Enable the hitbox for the attack

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float length = stateInfo.length; // Get the length of the current animation
        yield return new WaitForSeconds(length); // Wait for the animation to finish

        hitBox.DisableAttack(); // Disable the hitbox after the attack
        isAttacking = false; // Reset the attacking flag
    }
}
