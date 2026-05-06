using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class EatState : CreatureState
    {
        public EatState(Creature creature, StateMachine stateMachine) : base(creature, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            creature.ResumeNavMesh();
            Vector3 eatPos = creature.player.transform.position + Vector3.forward * 0.5f + Vector3.right * 0.5f;
            creature.Wrap(eatPos); // Ensure the creature wraps around the player position
            creature.StopNavMesh(); // Stop NavMeshAgent to prevent movement during eating
            creature.animator.SetBool("Bite", true);
            SoundManager.Instance.PlayLoop(SoundManager.Instance.zombieEating); // Play the eating sound
        }

        public override void LogicUpdate()
        {
            if (creature.player == null)
            {   
                creature.animator.SetBool("Bite", false); // Stop biting animation
                SoundManager.Instance.StopLoop(); // Stop the eating sound
                stateMachine.ChangeState(creature.wander); // If player is null, go back to wandering
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void Exit()
        {
            base.Exit();

        }
    }
}