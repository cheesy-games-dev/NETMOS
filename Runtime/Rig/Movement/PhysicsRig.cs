using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsRig : MonoBehaviour
    {
        public LocomotionSphere LocomotionSphere;
        public SmoothLocomotion SmoothLocomotion;

        public float WalkSpeed = 7.5f; //The speed of the player while walking
        public float RunSpeed = 15f; //The speed of the player while running

        public Rigidbody
            LocomotionSphereRigidbody,
            FenderRigidbody,
            PelvisRigidbody,
            HeadRigidbody,
            LeftHandRigidbody,
            RightHandRigidbody;

        public ConfigurableJoint
            FenderPelvisJoint,
            PelvisHeadJoint,
            PelvisHeadColliderJoint,
            LeftHandJoint,
            RightHandJoint;

        public float
            AirAcceleration = 3f,
            FenderPelvisOffset = 0.55f,
            RealFenderPelvisOffset = 0.55f; //The multiplier for air acceleration

        public enum JumpStates
        {
            NotJumping,
            Anticipation,
            PushingGround,
            Ascending,
            Descending
        }
        public JumpStates JumpState; //What state of a jump the player is in

        public void Start()
        {
            Time.fixedDeltaTime = 1f / 144f;

            int playerLayer = LayerMask.NameToLayer("BIMOSRig");
            Physics.IgnoreLayerCollision(playerLayer, playerLayer);

            SetLayerRecursive(gameObject, LayerMask.NameToLayer("BIMOSRig"));
        }

        private void SetLayerRecursive(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = layer;

                Transform hasChildren = child.GetComponentInChildren<Transform>();
                if (!hasChildren)
                    continue;

                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }
}