using RayWenderlich.Unity.StatePatternInUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    #region Variables
    public StateMachine FSM;
    public WanderState wander;
    public ChaseState chase; 
    public AttackState attack;
    public EatState eat;

    public Transform[] wayPoints;
    public Animator animator;

    [SerializeField] private CreatureData data;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private CreatureHitBox attackHitBox; // Reference to creature hitbox script
    public GameObject player;
    private bool jumpAttackOccured;
    public bool isAttacking;

    [SerializeField] private float creatureHp = 100f;
    #endregion

    #region Properties
    public float MoveSpeed => data.moveSpeed;
    public float ChaseSpeed => data.chaseSpeed;
    public float DetectionRange => data.playerDetectionRange;
    public float FieldOfView => data.fieldOfView;
    public float JumpHeight => data.jumpHeight;
    public float JumpDuration => data.jumpDuration;
    public float LungeRange => data.creatureLungeRange;
    public float AttackRange => data.creatureAttackRange;
    public float JumpAttackDamage => data.jumpAttackDamage;
    public float NormalAttackDamage => data.normalAttackDamage;
    public bool JumpAttackOccured => jumpAttackOccured;
    
    #endregion

    #region Methods
    public bool SeePlayer()
    {
        if (player == null) return false;
        Vector3 dir = player.transform.position - transform.position;
        float distance = dir.magnitude;
        dir.Normalize();
        float angle = Vector3.Angle(transform.forward, dir);
        if (distance <= DetectionRange && angle <= FieldOfView / 2f)
        {
            return true; // Player is within detection range and field of view
        }
        return false; // Player is not visible
    }

    public void Move(Vector3 targetPos, float speed)
    {
        if (targetPos == null) return;
        navMeshAgent.isStopped = false; // Ensure NavMeshAgent is not stopped
        navMeshAgent.speed = speed;
        transform.LookAt(targetPos); // Rotate towards the target position
        navMeshAgent.SetDestination(targetPos);
        animator.SetFloat("Speed", navMeshAgent.speed);          //just make sure navMeshAgent speed is the same as ChaseSpeed
        if (navMeshAgent.isOnOffMeshLink)
        {
            StartCoroutine(JumpOverLink(navMeshAgent.currentOffMeshLinkData));
        }
    }

    public void StopNavMesh()
    {
        navMeshAgent.isStopped = true; // Stop the NavMeshAgent
        animator.SetFloat("Speed", 0f); // Set speed to 0 in the animator   
    }

    public void ResumeNavMesh()
    {
        navMeshAgent.isStopped = false; // Resume the NavMeshAgent
    }

    IEnumerator JumpOverLink(OffMeshLinkData link)
    {
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = link.endPos;
        float t = 0f;

        // Trigger jump animation
        animator.SetTrigger("Jump");

        while (t < 1f)
        {
            t += Time.deltaTime / JumpDuration;

            // Parabola motion for jump/fall
            float height = Mathf.Sin(Mathf.PI * t) * JumpHeight;
            transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

            yield return null;
        }

        // Trigger land animation when reaching destination

        navMeshAgent.Warp(endPos);
        navMeshAgent.CompleteOffMeshLink();
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    public void TakeDamage(float damage)
    {
        creatureHp -= damage;
        if (creatureHp <= 0)
        {   
            animator.applyRootMotion = true; // Enable root motion for death animation
            animator.SetTrigger("Die"); // Trigger die animation
            SoundManager.Instance.PlaySound(SoundManager.Instance.zombieDeath); // Play death sound
            Destroy(gameObject, 3.5f); // Destroy the creature after the death animation finishes
        }
        else
        {
            animator.applyRootMotion = false;
            animator.SetTrigger("Hit"); // Trigger get hit animation
            SoundManager.Instance.PlaySound(SoundManager.Instance.zombieHurt); // Play hurt sound
        }
    }

    public void JumpAttack()
    {   
        if (isAttacking) return; // Prevent multiple attacks at the same time
        StartCoroutine(JumpAttackCoroutine());
    }

    public void NormalAttack()
    {   
        if (isAttacking) return; // Prevent multiple attacks at the same time
        StartCoroutine(NormalAttackCoroutine());
    }

    IEnumerator JumpAttackCoroutine()
    {   
        isAttacking = true; // Set attacking flag to true
        transform.LookAt(player.transform.position); // Ensure the creature faces the player before jumping
        navMeshAgent.isStopped = false; // Ensure NavMeshAgent is not stopped

        //animator.applyRootMotion = true; // Enable root motion for the jump attack
        animator.SetTrigger("Jump Attack"); // Trigger jump attack animation

        jumpAttackOccured = true; // Set the jump attack flag to true
        attackHitBox.EnableAttack(); // Enable the hitbox for the attack

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float length = stateInfo.length; // Get the length of the jump attack animation

        yield return new WaitForSeconds(length); // Wait for the animation to start before moving

        attackHitBox.DisableAttack(); // Disable the hitbox after the attack

        navMeshAgent.isStopped = true; // Stop the NavMeshAgent after the attack
        jumpAttackOccured = false; // Reset the jump attack flag
        isAttacking = false; // Reset the attacking flag
        //animator.applyRootMotion = false; // Disable root motion after the attack
    }

    IEnumerator NormalAttackCoroutine()
    {   
        isAttacking = true; // Set attacking flag to true
        transform.LookAt(player.transform.position); // Ensure the creature faces the player before attacking
        animator.SetTrigger("Normal Attack"); // Trigger normal attack animation
        attackHitBox.EnableAttack(); // Enable the hitbox for the attack
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float length = stateInfo.length; // Get the length of the normal attack animation
        yield return new WaitForSeconds(length); // Wait for the animation to finish
        attackHitBox.DisableAttack(); // Disable the hitbox after the attack
        isAttacking = false; // Reset the attacking flag
    }

    public void Wrap(Vector3 position)
    {
        navMeshAgent.Warp(position); // Warp the creature to a new position
    }
    #endregion

    #region MonoBehaviour Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
 
        FSM = new StateMachine();
        wander = new WanderState(this, FSM);
        chase = new ChaseState(this, FSM);
        attack = new AttackState(this, FSM);
        eat = new EatState(this, FSM);

        FSM.Initialize(wander);

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FSM.CurrentCreatureState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        FSM.CurrentCreatureState.PhysicsUpdate();
    }
    #endregion
}