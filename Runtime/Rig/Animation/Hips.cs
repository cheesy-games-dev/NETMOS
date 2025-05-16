using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class Hips : MonoBehaviour
    {
        private BIMOSRig _player;

        [SerializeField]
        private Transform _hipStandTransform, _hipCrouchTransform;

        private void Start()
        {
            _player = BIMOSRig.Instance;
        }

        void Update()
        {
            float standingPercent = (_player.ControllerRig.CameraTransform.position.y - _player.PhysicsRig.LocomotionSphereRigidbody.position.y + 0.2f) / 1.65f;
            transform.position = Vector3.Lerp(_hipCrouchTransform.position, _hipStandTransform.position, standingPercent);
        }
    }
}