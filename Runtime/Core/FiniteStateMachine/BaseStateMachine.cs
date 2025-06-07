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

        protected abstract void Setup();

        private void Awake()
        {
            Setup();

            var stateMachine = (T)this;

            foreach (var state in States)
            {
                var stateInstance = Instantiate(state);
                stateInstance.ConnectTo(stateMachine);
                _stateLookup[stateInstance.GetType()] = stateInstance;
            }

            if (!_initialState)
            {
                Debug.LogError($"{GetType().Name} has no initial state assigned", this);
                return;
            }

            var type = _initialState.GetType();
            if (_stateLookup.TryGetValue(type, out var initialStateInstance))
                ChangeState(initialStateInstance);
            else
                Debug.LogError($"Initial state {_initialState.name} was not found in {GetType().Name}'s states");
        }

        public virtual void ChangeState(IState<T> newState)
        {
            CurrentState?.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }

        public void ChangeState<U>() where U : IState<T>
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

        public void UpdateState() => CurrentState?.UpdateState();
    }
}
