using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class AttackState : CreatureState
    {
        private Character characterScript;
        public AttackState(Creature creature, StateMachine stateMachine) : base(creature, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            creature.StopNavMesh();
            characterScript = creature.player.GetComponent<Character>();
        }

        public override void LogicUpdate()
        {

            if (characterScript.PlayerHP <= 0f)
            {
                stateMachine.ChangeState(creature.eat);
                return;
            }

            if (creature.isAttacking) return; // wait until current attack finishes

            if (Vector3.Distance(creature.transform.position, creature.player.transform.position) <= creature.AttackRange)
            {
                creature.NormalAttack(); // Perform normal attack
            }
            else if (Vector3.Distance(creature.transform.position, creature.player.transform.position) <= creature.LungeRange)
            {
                // If within lunge range, perform a jump attack
                creature.JumpAttack();
            }
            else
            {
                // If player is too far, stop attacking and transition to chase state
                stateMachine.ChangeState(creature.chase);
                return;
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

