using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

namespace BIMOS
{
    public class Hand : MonoBehaviour
    {
        public HandAnimator HandAnimator;
        public Grabbable CurrentGrab;
        public HandInputReader HandInputReader;
        public Transform PalmTransform;
        public PhysicsHand PhysicsHand;
        public Transform PhysicsHandTransform;
        public GrabHandler GrabHandler;
        public bool IsLeftHand;
        public Hand otherHand;
        public Collider PhysicsHandCollider;
        public Joint GrabJoint;

        [SerializeField]
        private InputActionReference HapticAction;

        public void SendHapticImpulse(float amplitude, float duration)
            => OpenXRInput.SendHapticImpulse(HapticAction, amplitude, duration);
    }
}
