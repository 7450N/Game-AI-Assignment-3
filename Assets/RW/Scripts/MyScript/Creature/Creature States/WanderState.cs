using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class WanderState : CreatureState
    {
        private int currentWayPointIndex = 0;
        private bool seePlayer;

        public WanderState(Creature creature, StateMachine stateMachine) : base(creature, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            creature.ResumeNavMesh(); // Ensure NavMeshAgent is active
            seePlayer = false;
            if (creature.wayPoints.Length > 0)
            {
                currentWayPointIndex = Random.Range(0, creature.wayPoints.Length);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            seePlayer = creature.SeePlayer();
            if (seePlayer)
            {
                creature.animator.SetTrigger("Scream"); // Trigger the scream animation when the player is seen
                stateMachine.ChangeState(creature.chase);
            }
            if (creature.wayPoints.Length == 0) return;

            Transform target = creature.wayPoints[currentWayPointIndex];

            Vector3 targetPos = new Vector3(target.position.x, creature.transform.position.y, target.position.z);
            creature.Move(targetPos, creature.MoveSpeed);

            if (Vector3.Distance(creature.transform.position, targetPos) < 0.5f)
            {
                // Reached the waypoint, select a new one
                currentWayPointIndex = Random.Range(0, creature.wayPoints.Length);
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

