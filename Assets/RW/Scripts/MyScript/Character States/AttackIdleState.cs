using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class AttackIdleState : State
    {
        public AttackIdleState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            character.Equip(character.MeleeWeapon); // Equip the melee weapon
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
            }
        }
        public override void Exit()
        {
            base.Exit();
            character.Unequip(); // Unequip the weapon
        }
    }
}

