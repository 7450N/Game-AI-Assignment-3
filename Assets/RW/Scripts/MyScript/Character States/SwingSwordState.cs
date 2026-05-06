using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SwingSwordState : MeleeState
    {
        private int swingParam = Animator.StringToHash("SwingMelee");
        private bool swingAttack;
        private bool blocking;

        public SwingSwordState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            swingAttack = Input.GetButtonDown("Fire1"); // Check for swing attack input
            blocking = Input.GetButton("Fire2"); // Check for blocking input
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (blocking)
            {
                character.Animator.SetBool("Block", true); // Set the block animation
                character.isBlocking = true; // Set the blocking flag
            }
            else
            {
                character.Animator.SetBool("Block", false); // Reset the block animation
                character.isBlocking = false; // Reset the blocking flag
            }
            if (swingAttack)
            {
                character.TriggerAnimation(swingParam); // Trigger the swing animation
                SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSwings); // Play swing sound
                // Enable attack window via MonoBehaviour coroutine
                character.StartAttackWindow(0.4f); // 0.4s attack window
            }
            
            if (character.currentEquipment != Character.EquipmentType.Melee)
            {
                stateMachine.ChangeState(character.sheathSword); // Transition to SheathSwordState if the equipment is not melee
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