using BIMOS;
using Mirror;
using UnityEngine;

namespace  Netmos
{
    [RequireComponent(typeof(Interactable)), RequireComponent(typeof(Grab))]
    public class NetworkedInteractable : NetworkBehaviour
    {
        [NetVar] public bool isSelected = false;
        public Interactable interactable;

        #region BasicInit
        public void Start()
        {
            interactable ??= GetComponent<Interactable>();
            interactable.GrabEvent.AddListener(() =>
            {
                OnGrabCmd(NetworkServer.localConnection);
            });
            interactable.ReleaseEvent.AddListener(OnDropCmd);
            InitEvents();
        }

        [NetCmd(requiresAuthority = false)]
        private void OnGrabCmd(NetworkConnectionToClient receviedConn)
        {
            netIdentity.RemoveClientAuthority();
            netIdentity.AssignClientAuthority(receviedConn);
            isSelected = true;
        }

        private void OnDropCmd()
        {
            isSelected = false;
        }
        #endregion
        #region InteractableEvents
        private void InitEvents()
        {
            interactable.GrabEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnGrab), null);
            });
            interactable.ReleaseEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnRelease), null);
            });
            interactable.TriggerDownEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnTrigger), true);
            });
            interactable.TriggerUpEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnTrigger), false);
            });
            interactable.PrimaryDownEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnPrimary), true);
            });
            interactable.PrimaryUpEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnPrimary), false);
            });
            interactable.SecondaryDownEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnSecondary), true);
            });
            interactable.SecondaryUpEvent.AddListener(() =>
            {
                EventCmd(nameof(interactable.OnSecondary), false);
            });
        }

        [NetCmd(requiresAuthority = false)]
        void EventCmd(string eventName, object eventButtonDown) => EventClientRpc(eventName, eventButtonDown);

        [NetClientRpc(includeOwner = true)]
        private void EventClientRpc(string eventName, object eventButtonDown)
        {
            if (eventButtonDown != null)
                interactable.SendMessage(eventName, (bool)eventButtonDown);
            else
                interactable.SendMessage(eventName);
        }
        #endregion
    }
}
