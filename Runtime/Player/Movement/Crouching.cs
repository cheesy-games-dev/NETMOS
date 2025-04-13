using UnityEngine;

namespace BIMOS
{
    public class Crouching : MonoBehaviour
    {
        private BIMOSRig _player;

        private void Start()
        {
            _player = BIMOSRig.Instance;
        }

        public void FixedUpdate()
        {
            VirtualCrouching();
            ApplyCrouch();
        }

        private void VirtualCrouching()
        {
            bool isJumping = _player.PhysicsRig.JumpState == PhysicsRig.JumpStates.Ascending || _player.PhysicsRig.JumpState == PhysicsRig.JumpStates.Descending;
            if (Mathf.Abs(_player.ControllerRig.InputReader.CrouchInput) > 0.75f && !isJumping)
            {
                _player.PhysicsRig.FenderPelvisOffset += _player.ControllerRig.InputReader.CrouchInput * 2.5f * Time.fixedDeltaTime / 1.65f;
            }
        }

        private void ApplyCrouch()
        {
            float lowerBound = 0.2f;
            float upperBound = 0.55f;
            if (_player.PhysicsRig.JumpState == PhysicsRig.JumpStates.Anticipation)
            {
                lowerBound = 0f; //Enables full crouching
                upperBound = 0.35f; //Disables standing up
            }
            else if (Mathf.Abs(_player.ControllerRig.InputReader.CrouchInput) > 0.1f || _player.PhysicsRig.JumpState != PhysicsRig.JumpStates.NotJumping)
            {
                lowerBound = 0f; //Enables full crouching
                upperBound = 0.65f; //Enables tippy-toes
            }
            _player.PhysicsRig.FenderPelvisOffset = Mathf.Clamp(_player.PhysicsRig.FenderPelvisOffset, lowerBound, upperBound);
            _player.PhysicsRig.FenderPelvisJoint.targetPosition = new Vector3(0f, -_player.PhysicsRig.FenderPelvisOffset, 0f);

            _player.PhysicsRig.RealFenderPelvisOffset = _player.PhysicsRig.PelvisRigidbody.position.y - _player.PhysicsRig.FenderRigidbody.position.y - 0.225f;
        }
    }
}