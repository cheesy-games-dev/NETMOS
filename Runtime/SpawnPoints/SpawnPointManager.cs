using UnityEngine;

namespace BIMOS
{
    public class SpawnPointManager : MonoBehaviour
    {
        public SpawnPoint SpawnPoint;
        public GameObject PlayerInstance;

        [Tooltip("Inserts this prefab in front of player on respawn")]
        public GameObject StarterProp;

        public Vector3 StarterPropOffset = new Vector3(0, 1f, 1f);

        private GameObject _starterPropInstance;

        private void Awake()
        {
            if (!SpawnPoint)
            {
                SpawnPoint = FindFirstObjectByType<SpawnPoint>();
                if (!SpawnPoint)
                {
                    Debug.LogError("You must have at least one spawn point!");
                    return;
                }
            }

            Respawn();
        }

        public void SetStarterPropOffsetX(float x) => StarterPropOffset.x = x;

        public void SetStarterPropOffsetY(float y) => StarterPropOffset.y = y;

        public void SetStarterPropOffsetZ(float z) => StarterPropOffset.z = z;

        public void SetSpawnPoint(SpawnPoint spawnPoint) => SpawnPoint = spawnPoint;

        public void SetStarterProp(GameObject starterProp) => StarterProp = starterProp;

        public void Respawn()
        {
            if (PlayerInstance)
                Destroy(PlayerInstance);

            PlayerInstance = SpawnPoint.Spawn();

            Destroy(_starterPropInstance);

            if (StarterProp)
                _starterPropInstance = Instantiate(StarterProp, SpawnPoint.transform.TransformPoint(StarterPropOffset), SpawnPoint.transform.rotation);
        }
    }
}