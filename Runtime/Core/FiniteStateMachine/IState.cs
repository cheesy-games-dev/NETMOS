namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public interface IState<T> where T : IStateMachine<T>
    {
        void Enter();
        void Update();
        void Exit();
    }
}
