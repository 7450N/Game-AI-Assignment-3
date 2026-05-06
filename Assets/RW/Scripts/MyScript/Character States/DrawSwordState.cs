using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DrawSwordState : MeleeState
    {   

        private int drawSwordParam = Animator.StringToHash("DrawMelee");

        private bool swingAttack;

        public DrawSwordState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }
        public override void Enter()
        {
            base.Enter();
            character.Unequip(); // Unequip any current equipment
            character.Equip(character.MeleeWeapon); // Equip the melee weapon
            character.TriggerAnimation(drawSwordParam);
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeEquip); // Play the draw sword sound
        }
        public override void HandleInput()
        {
            base.HandleInput();
            swingAttack = Input.GetButtonDown("Fire1"); 
        }
        public override void LogicUpdate()
        {
            if (swingAttack)
            {
                stateMachine.ChangeState(character.swingSword); // Transition to MeleeState after drawing the sword
            }
            if (character.currentEquipment != Character.EquipmentType.Melee)
            {
                stateMachine.ChangeState(character.sheathSword); // Transition to SheathSwordState if the equipment is not melee
            }
        }
        public override void Exit()
        {
            base.Exit();
            // Cleanup after drawing the sword if necessary
        }
    }

}
