using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsHand : MonoBehaviour
    {
        public Transform Target, Controller;
        public Vector3 TargetOffsetPosition;
        public Quaternion TargetOffsetRotation;
        public Rigidbody Rigidbody;

        private BIMOSRig _player;

        private ConfigurableJoint _handJoint;

        private void Start()
        {
            _player = BIMOSRig.Instance;

            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.solverIterations = 60;
            Rigidbody.solverVelocityIterations = 10;

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