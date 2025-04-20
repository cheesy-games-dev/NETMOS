using UnityEngine;

namespace BIMOS
{
    public class SpawnPoint : MonoBehaviour
    {
        private void Awake() => Destroy(transform.GetChild(0).gameObject);

        public GameObject Spawn()
        {
            GameObject player = Instantiate(
                SpawnPointManager.Instance.RigPrefab,
                transform.position,
                Quaternion.identity
            );
            player.GetComponent<BIMOSRig>().ControllerRig.transform.rotation = transform.rotation;
            return player;
        }
    }
}