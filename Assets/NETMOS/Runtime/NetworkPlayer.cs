namespace Netmos
{
    using UnityEngine;
    using UnityEngine.Events;

    public class NetworkPlayer : NetworkEntity
    {
        public PlayerInfo playerInfo;

        public UnityEvent authorityEvents;
        public UnityEvent noAuthorityEvents;

        void Update()
        {
            if (authority)
            {
                SyncTransform(NetworkManager.LocalPlayer.head, playerInfo.head);
                SyncTransform(NetworkManager.LocalPlayer.leftHand, playerInfo.leftHand);
                SyncTransform(NetworkManager.LocalPlayer.rightHand, playerInfo.rightHand);
                authorityEvents.Invoke();
            }
            else
            {
                noAuthorityEvents.Invoke();
            }
        }

        public static void SyncTransform(Transform target, Transform tracker)
        {
            tracker.position = target.position; 
            tracker.rotation = target.rotation;
        }
    }
}