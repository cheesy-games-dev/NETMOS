using UnityEngine;

namespace BIMOS
{
    public class SpawnPoint : MonoBehaviour
    {
        private SpawnPointManager _spawnPointManager;

        private void Start()
        {
            GameObject placeholder = transform.GetChild(0).gameObject;
            Destroy(placeholder);

            _spawnPointManager = SpawnPointManager.Instance;
        }

        public GameObject Spawn()
        {
            GameObject player = Instantiate(_spawnPointManager.RigPrefab, transform.position, Quaternion.identity);
            player.GetComponent<BIMOSRig>().ControllerRig.transform.rotation = transform.rotation;
            return player;
        }
    }
}