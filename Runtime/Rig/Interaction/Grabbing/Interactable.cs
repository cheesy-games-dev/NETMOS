using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Rig
{
    [AddComponentMenu("BIMOS/Grabbables/Interactable")]
    public class Interactable : NetworkBehaviour
    {
        public UnityEvent
            TriggerDownEvent,
            TriggerUpEvent,
            PrimaryDownEvent,
            PrimaryUpEvent,
            SecondaryDownEvent,
            SecondaryUpEvent,
            GrabEvent,
            ReleaseEvent;
        public TickEvent OnTick;
        public TickEvent OnPhysicsTick;

        private Grabbable _grab;

        private void Awake() => _grab = GetComponent<Grabbable>();

        private void OnEnable()
        {
            _grab.OnGrab += OnGrab;
            _grab.OnRelease += OnRelease;
        }

        private void OnDisable()
        {
            _grab.OnGrab -= OnGrab;
            _grab.OnRelease -= OnRelease;
        }

        private void CheckInputs(out float trigger, out bool primary, out bool secondary)
        {
            float leftTrigger = 0f;
            bool leftPrimary = false;
            bool leftSecondary = false;
            if (_grab.LeftHand)
            {
                leftTrigger = _grab.LeftHand.HandInputReader.Trigger;
                leftPrimary = _grab.LeftHand.HandInputReader.PrimaryButton;
                leftSecondary = _grab.LeftHand.HandInputReader.SecondaryButton;
            }

            float rightTrigger = 0f;
            bool rightPrimary = false;
            bool rightSecondary = false;
            if (_grab.RightHand)
            {
                rightTrigger = _grab.RightHand.HandInputReader.Trigger;
                rightPrimary = _grab.RightHand.HandInputReader.PrimaryButton;
                rightSecondary = _grab.RightHand.HandInputReader.SecondaryButton;
            }

            trigger = Mathf.Max(leftTrigger, rightTrigger);
            primary = leftPrimary || rightPrimary;
            secondary = leftSecondary || rightSecondary;
        }

        public void Tick()
        {
            CheckInputs(out float trigger, out bool primary, out bool secondary);
            OnTick?.Invoke(trigger, primary, secondary);
        }

        public void PhysicsTick()
        {
            CheckInputs(out float trigger, out bool primary, out bool secondary);
            OnPhysicsTick?.Invoke(trigger, primary, secondary);
        }

        [Command(requiresAuthority = false)]
        public void OnTrigger(bool isButtonDown)=>OnTriggerEvent(isButtonDown);

        [Command(requiresAuthority = false)]
        public void OnPrimary(bool isButtonDown)=>OnPrimaryEvent(isButtonDown);


        [Command(requiresAuthority = false)]
        public void OnSecondary(bool isButtonDown)=>OnSecondaryEvent(isButtonDown);

        [Command(requiresAuthority = false)]
        public void OnGrab()=>OnGrabEvent();

        [Command(requiresAuthority = false)]
        public void OnRelease()=>OnReleaseEvent();

        [ClientRpc]
        private void OnTriggerEvent(bool isButtonDown)
        {
            if (isButtonDown)
                TriggerDownEvent?.Invoke();
            else
                TriggerUpEvent?.Invoke();
        }

        [ClientRpc]
        private void OnPrimaryEvent(bool isButtonDown)
        {
            if (isButtonDown)
                PrimaryDownEvent?.Invoke();
            else
                PrimaryUpEvent?.Invoke();
        }

        [ClientRpc]
        private void OnSecondaryEvent(bool isButtonDown)
        {
            if (isButtonDown)
                SecondaryDownEvent?.Invoke();
            else
                SecondaryUpEvent?.Invoke();
        }
        [ClientRpc]
        private void OnGrabEvent()
        {
            GrabEvent?.Invoke();
        }
        [ClientRpc]
        private void OnReleaseEvent()
        {
            ReleaseEvent?.Invoke();
        }

        [Serializable]
        public class TickEvent : UnityEvent<float, bool, bool> { }
    }
}