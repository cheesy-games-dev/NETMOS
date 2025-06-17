using Mirror;
using UnityEngine;

namespace Netmos
{
    [AddComponentMenu("NETMOS/Network Respawner")]
    public class NetworkRespawner : MonoBehaviour {
        [SerializeField]
        private bool _respawnOnAwake;

        [SerializeField]
        private GameObject _prefab;

        private GameObject _instance;

        private void Start() {
            if (_respawnOnAwake)
                Respawn();
        }

        public void Respawn() {
            if (!NetworkServer.active)
                return;
            NetworkServer.Destroy(_instance);
            _instance = Instantiate(_prefab, transform.position, transform.rotation);
            NetworkServer.Spawn(_instance);
        }
    }
}
