using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "Game Data/Creature Data")]
public class CreatureData : ScriptableObject
{
    public float playerDetectionRange = 20f;
    public float fieldOfView = 120f;
    public float attackRange = 2f;
    public float moveSpeed = 0.5f;
    public float chaseSpeed = 3f;
    public float jumpHeight = 2f;
    public float jumpDuration = 1f;
    public float throwForce = 10f;
    public float creatureLungeRange = 7f;
    public float creatureAttackRange = 2f;
    public float jumpAttackDamage = 20f;
    public float normalAttackDamage = 10f;
}
