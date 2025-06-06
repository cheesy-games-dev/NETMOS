using System;
using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public abstract class BaseStateMachine<T> : MonoBehaviour, IStateMachine<T> where T : BaseStateMachine<T>
    {
        [SerializeField]
        protected List<BaseState<T>> States;

        [SerializeField]
        private BaseState<T> _initialState;

        protected IState<T> CurrentState { get; private set; }

        private readonly Dictionary<Type, IState<T>> _stateLookup = new();

        protected virtual void Awake()
        {
            var stateMachine = (T)this;
            foreach (var state in States)
            {
                state.AttachTo(stateMachine);
                _stateLookup[state.GetType()] = state;
            }

            ChangeState(_initialState);
        }

        public virtual void ChangeState(IState<T> newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void SetState<U>() where U : IState<T>
        {
            var state = GetState<U>();
            if (state != null)
                ChangeState(state);
        }

        public IState<T> GetState<U>() where U : IState<T>
        {
            if (_stateLookup.TryGetValue(typeof(U), out var state))
                return state;

            return null;
        }

        public void UpdateState() => CurrentState.Update();
    }
}
