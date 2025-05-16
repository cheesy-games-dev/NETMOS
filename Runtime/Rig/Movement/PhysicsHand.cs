using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsHand : MonoBehaviour
    {
        public Transform Target, Controller;
        public Vector3 TargetOffsetPosition;
        public Quaternion TargetOffsetRotation;

        private BIMOSRig _player;

        private ConfigurableJoint _handJoint;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _player = BIMOSRig.Instance;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.solverIterations = 60;
            _rigidbody.solverVelocityIterations = 10;

            TargetOffsetRotation = Quaternion.identity;
            _handJoint = GetComponent<ConfigurableJoint>();
        }

        private void FixedUpdate()
        {
            Vector3 targetPosition = Target.TransformPoint(TargetOffsetPosition);
            Vector3 headOffset = targetPosition - _player.PhysicsRig.HeadRigidbody.position;
            _handJoint.targetPosition = headOffset;

            //Rotation
            _handJoint.targetRotation = Target.rotation * TargetOffsetRotation;
        }
    }
}