using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class MagicState : State
    {
        private bool fireMagic;

        public MagicState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            //character.Unequip(); // Unequip any current equipment
            character.Equip(character.ShootableWeapon); // Equip the melee weapon
            SoundManager.Instance.PlaySound(SoundManager.Instance.itemEquip); // Play the draw sword sound
        }

        public override void HandleInput()
        {
            base.HandleInput();
            fireMagic = Input.GetButtonDown("Fire1"); // Check for fire magic input
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (fireMagic)
            {
                character.Shoot(); // Call the method to shoot magic
            }
            switch (character.currentEquipment)
            {
                case (Character.EquipmentType.Melee):
                    stateMachine.ChangeState(character.drawSword); // Transition to DrawSwordState if the equipment is melee
                    break;
                case (Character.EquipmentType.None):
                    stateMachine.ChangeState(character.attackIdle); // Transition to IdleState if no equipment is equipped
                    break;
            }

        }
        public override void Exit()
        {
            base.Exit();
            SoundManager.Instance.PlaySound(SoundManager.Instance.itemUnequip); // Play the sheath sound
            character.Unequip(); // unequip the weapon
        }
    }
}

