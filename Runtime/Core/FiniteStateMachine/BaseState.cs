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

        public void ConnectTo(T stateMachine) => StateMachine = stateMachine;

        public virtual void OnAwake() { }
        public virtual void OnStart() { }

        public void EnterState()
        {
            OnEnter?.Invoke();
            Enter();
        }

        protected virtual void Enter() { }

        public void UpdateState()
        {
            OnUpdate?.Invoke();
            Update();
        }

        protected virtual void Update() { }

        public void ExitState()
        {
            OnExit?.Invoke();
            Exit();
        }

        protected virtual void Exit() { }
    }
}
