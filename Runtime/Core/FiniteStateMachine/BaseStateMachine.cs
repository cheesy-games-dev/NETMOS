using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public abstract class BaseStateMachine<T> : MonoBehaviour, IStateMachine<T> where T : BaseStateMachine<T>
    {
        protected IState<T> CurrentState { get; private set; }

        public event Action<IState<T>> OnStateEntered;
        public event Action<IState<T>> OnStateExited;

        public virtual void SetState(IState<T> newState)
        {
            CurrentState?.Exit();
            OnStateExited?.Invoke(CurrentState);

            CurrentState = newState;

            CurrentState.Enter();
            OnStateEntered?.Invoke(CurrentState);
        }
    }
}
