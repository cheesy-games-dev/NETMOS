using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class SpawnPointManager : MonoBehaviour
    {
        public static SpawnPointManager Instance { get; private set; }

        public SpawnPoint SpawnPoint;
        public GameObject PlayerInstance;

        [Tooltip("Inserts this prefab in front of player on respawn")]
        public GameObject StarterProp;

        public Vector3 StarterPropOffset = new Vector3(0, 1f, 1f);

        private BIMOSRig _player;
        private GameObject _starterPropInstance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _player = BIMOSRig.Instance;

            if (!SpawnPoint)
            {
                SpawnPoint = FindFirstObjectByType<SpawnPoint>();
                if (!SpawnPoint)
                {
                    Debug.LogError("You must have at least one spawn point!");
                    return;
                }
            }
        }

        private void Start() => Respawn();

        public void SetStarterPropOffsetX(float x) => StarterPropOffset.x = x;

        public void SetStarterPropOffsetY(float y) => StarterPropOffset.y = y;

        public void SetStarterPropOffsetZ(float z) => StarterPropOffset.z = z;

        public void SetSpawnPoint(SpawnPoint spawnPoint) => SpawnPoint = spawnPoint;

        public void SetStarterProp(GameObject starterProp) => StarterProp = starterProp;

        public void Respawn()
        {
            TeleportToSpawnPoint(SpawnPoint.transform);

            Destroy(_starterPropInstance);
            if (StarterProp)
                _starterPropInstance = Instantiate(StarterProp, SpawnPoint.transform.TransformPoint(StarterPropOffset), SpawnPoint.transform.rotation);
        }

        private void TeleportToSpawnPoint(Transform spawnPoint)
        {
            var rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
            var rootPosition = _player.PhysicsRig.LocomotionSphereRigidbody.position;
            foreach (var rigidbody in rigidbodies)
            {
                var offset = rigidbody.position - rootPosition; //Calculates the offset between the locoball and the rigidbody
                rigidbody.position = spawnPoint.position + offset; //Sets the rigidbody's position
                rigidbody.transform.position = spawnPoint.position + offset; //Sets the transform's position
            }

            //Update the animation rig's position
            _player.AnimationRig.Transforms.Hips.position += spawnPoint.position - rootPosition;

            //Move the player's animated feet to the new position
            _player.AnimationRig.Feet.TeleportFeet();

            _player.ControllerRig.transform.rotation = transform.rotation;
        }
    }
}