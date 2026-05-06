using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SheathSwordState : MeleeState
    {   
        private int sheathSwordParam = Animator.StringToHash("SheathMelee");
        public SheathSwordState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            character.TriggerAnimation(sheathSwordParam); // Trigger the sheath animation
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSheath); // Play the sheath sound
            character.SheathWeapon(); // Sheath the weapon
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            switch (character.currentEquipment)
            {
                case (Character.EquipmentType.Melee):
                    stateMachine.ChangeState(character.drawSword); // Transition to DrawSwordState if the equipment is melee
                    break;
                case (Character.EquipmentType.Ranged):
                    stateMachine.ChangeState(character.magic); // Transition to MagicState if the equipment is magic
                    break;
                case (Character.EquipmentType.None):
                    stateMachine.ChangeState(character.attackIdle); // Transition to IdleState if no equipment is equipped
                    break;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void Exit()
        {
            base.Exit();
            character.Unequip(); // Unequip the weapon after sheathing
        }
    }
}

