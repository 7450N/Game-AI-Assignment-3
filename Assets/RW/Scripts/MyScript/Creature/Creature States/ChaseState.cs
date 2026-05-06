using NUnit.Framework.Constraints;
using System.Drawing.Text;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class ChaseState : CreatureState
    {
        private bool seePlayer;
        public ChaseState(Creature creature, StateMachine stateMachine) : base(creature, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            creature.ResumeNavMesh(); // Ensure NavMeshAgent is active
            seePlayer = true;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            Vector3 targetPos = creature.player.transform.position;
            creature.Move(targetPos, creature.ChaseSpeed);
            seePlayer = creature.SeePlayer();
            if (!seePlayer)
            {
                stateMachine.ChangeState(creature.wander);
            }
            if (Vector3.Distance(creature.transform.position, creature.player.transform.position) <= creature.LungeRange)
            {
                // Player is close enough, transition to attack state
                stateMachine.ChangeState(creature.attack);
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

