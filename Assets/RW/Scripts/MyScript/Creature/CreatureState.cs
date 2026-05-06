
namespace RayWenderlich.Unity.StatePatternInUnity
{
    public abstract class CreatureState
    {
        protected Creature creature;
        protected StateMachine stateMachine;

        protected CreatureState(Creature creature, StateMachine stateMachine)
        {
            this.creature = creature;
            this.stateMachine = stateMachine;
        }

        protected void DisplayOnUI(UIManager.Alignment alignment)
        {
            UIManager.Instance.Display(this, alignment);
        }

        public virtual void Enter()
        {
            DisplayOnUI(UIManager.Alignment.Right);
        }

        public virtual void LogicUpdate()
        {

        }

        public virtual void PhysicsUpdate()
        {

        }

        public virtual void Exit()
        {

        }
    }
}
    
