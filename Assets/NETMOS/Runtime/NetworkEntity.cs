using Mirror;
using UnityEngine;

namespace Netmos
{
    [RequireComponent(typeof(NetworkTransform)), RequireComponent(typeof(NetworkIdentity))]
    public class NetworkEntity : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            NetworkServer.SpawnObjects();
            netIdentity.AssignClientAuthority(NetworkServer.localConnection);
        }
    }
}