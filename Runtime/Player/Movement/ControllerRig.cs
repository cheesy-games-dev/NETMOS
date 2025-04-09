using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace BIMOS
{
    public class ControllerRig : MonoBehaviour
    {
        [Header("Transforms")]
        public Transform CameraOffsetTransform;
        public Transform CameraTransform;
        public Transform MenuCameraTransform;
        public Transform LeftControllerTransform;
        public Transform RightControllerTransform;
        public Transform FloorOffsetTransform;

        private TrackedPoseDriver
            _headsetDriver,
            _leftControllerDriver,
            _rightControllerDriver;

        private void Awake()
        {
            CameraTransform.GetComponent<Camera>().cullingMask = ~LayerMask.GetMask("BIMOSMenu");
            MenuCameraTransform.GetComponent<Camera>().cullingMask = LayerMask.GetMask("BIMOSMenu");

            _headsetDriver = CameraTransform.GetComponent<TrackedPoseDriver>();
            _leftControllerDriver = LeftControllerTransform.GetComponent<TrackedPoseDriver>();
            _rightControllerDriver = RightControllerTransform.GetComponent<TrackedPoseDriver>();

            _headsetDriver.enabled
                = _leftControllerDriver.enabled
                    = _rightControllerDriver.enabled
                        = false;

            StartCoroutine(WaitForHeadset());
        }

        private IEnumerator WaitForHeadset()
        {
            var headsetActive = false;

            while (!headsetActive)
            {
                try
                {
                    var display = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
                    if (display.running)
                        headsetActive = true;
                }
                catch { }
                yield return null;
            }

            _headsetDriver.enabled
                = _leftControllerDriver.enabled
                    = _rightControllerDriver.enabled
                        = true;
        }
    }
}
