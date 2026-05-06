using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class MeleeState : State
    {

        private int meleeParam = Animator.StringToHash("IsMelee");

        public MeleeState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }
 
        public override void Enter()
        {
            base.Enter();
            character.SetAnimationBool(meleeParam, true); // Set melee animation
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
        }
        public override void Exit()
        {
            base.Exit();
            character.SetAnimationBool(meleeParam, false); // Reset melee animation

            // Cleanup after meelee attack if necessary
        }
    }
}

