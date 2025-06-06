using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public abstract class BaseState<T> : ScriptableObject, IState<T> where T : IStateMachine<T>
    {
        protected T StateMachine;

        public event Action OnEnter;
        public event Action OnUpdate;
        public event Action OnExit;

        public void AttachTo(T stateMachine) => StateMachine = stateMachine;

        public virtual void Enter() => OnEnter?.Invoke();

        public virtual void Update () => OnUpdate?.Invoke();

        public virtual void Exit() => OnExit?.Invoke();
    }
}
