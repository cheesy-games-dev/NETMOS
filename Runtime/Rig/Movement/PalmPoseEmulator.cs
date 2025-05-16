using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Emulates palm pose for standalone devices
    /// </summary>
    public class PalmPoseEmulator : MonoBehaviour
    {
        [SerializeField]
        private bool _isLeftController;

        [SerializeField]
        private InputAction _positionAction, _rotationAction;

#if UNITY_ANDROID
        private Vector3 _positionOffset = new(0.01f, 0.013f, -0.0075f);
        private Vector3 _eulerAnglesOffset = new(0f, 0f, -70f);

        private TrackedPoseDriver _trackedPoseDriver;

        private void OnEnable()
        {
            _trackedPoseDriver = transform.parent.GetComponent<TrackedPoseDriver>();
            _trackedPoseDriver.positionAction = _positionAction;
            _trackedPoseDriver.rotationAction = _rotationAction;

            Quaternion rotationOffset = Quaternion.Euler(_eulerAnglesOffset);

            if (_isLeftController)
            {
                _positionOffset.x *= -1f;
                rotationOffset.x *= -1f;
                rotationOffset.w *= -1f;
            }

            transform.localRotation *= rotationOffset;
            transform.localPosition += _positionOffset;
        }
#endif
    }
}
