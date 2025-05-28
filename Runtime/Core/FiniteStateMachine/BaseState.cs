namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public abstract class BaseState<T> : IState<T> where T : IStateMachine<T>
    {
        protected T StateMachine;

        public void Initialize(T stateMachine) => StateMachine = stateMachine;

        public virtual void Enter() { }

        public virtual void Update () { }

        public virtual void Exit() { }
    }
}
