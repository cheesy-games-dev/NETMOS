using UnityEngine;

namespace BIMOS
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab, _placeholder;

        private void Awake()
        {
            Destroy(_placeholder);
        }

        public GameObject Spawn()
        {
            GameObject player = Instantiate(_playerPrefab, transform.position, Quaternion.identity);
            player.GetComponent<BIMOSRig>().ControllerRig.transform.rotation = transform.rotation;
            return player;
        }
    }
}