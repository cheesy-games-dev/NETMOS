using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace KadenZombie8.BIMOS.Rig
{
    public class ControllerRig : MonoBehaviour
    {
        private BIMOSRig _player;

        public InputReader InputReader;

        [Header("Transforms")]
        public Transform CameraOffsetTransform;
        public Transform CameraTransform;
        public Transform MenuCameraTransform;
        public Transform LeftControllerTransform;
        public Transform RightControllerTransform;
        public Transform FloorOffsetTransform;
        public float HeadsetStandingHeight = 1.65f;
        public float SmoothTurnSpeed = 5f;
        public float SnapTurnIncrement = 45f;
        public float FloorOffset
        {
            get => _floorOffset;
            set
            {
                _floorOffset = value;
                FloorOffsetTransform.localPosition = new Vector3(0f, _floorOffset, 0f);
            }
        }
        private float _floorOffset;

        private TrackedPoseDriver
            _headsetDriver,
            _leftControllerDriver,
            _rightControllerDriver;

        public void Start()
        {
            _player = BIMOSRig.Instance;
            transform.parent = _player.PhysicsRig.PelvisRigidbody.transform;

            CameraTransform.GetComponent<Camera>().cullingMask = ~LayerMask.GetMask("BIMOSMenu");
            MenuCameraTransform.GetComponent<Camera>().cullingMask = LayerMask.GetMask("BIMOSMenu");

            #region Preferences
            HeadsetStandingHeight = PlayerPrefs.GetFloat("HeadsetStandingHeight", 1.65f);
            HeadsetStandingHeight = Mathf.Clamp(HeadsetStandingHeight, 1f, 3f);

            FloorOffset = PlayerPrefs.GetFloat("FloorOffset", 0f);
            FloorOffset = Mathf.Clamp(FloorOffset, -1.35f, 0.65f);
            FloorOffsetTransform.localPosition = new Vector3(0f, PlayerPrefs.GetFloat("FloorOffset", 0f), 0f);

            SmoothTurnSpeed = PlayerPrefs.GetFloat("SmoothTurnSpeed", 10f);
            SnapTurnIncrement = PlayerPrefs.GetFloat("SnapTurnIncrement", 45f);
            #endregion

            ScaleCharacter();
            StartCoroutine(WaitForMotionControls());
        }

        private IEnumerator WaitForMotionControls()
        {
            _headsetDriver = CameraTransform.GetComponent<TrackedPoseDriver>();
            _leftControllerDriver = LeftControllerTransform.GetComponent<TrackedPoseDriver>();
            _rightControllerDriver = RightControllerTransform.GetComponent<TrackedPoseDriver>();

            _headsetDriver.enabled
                = _leftControllerDriver.enabled
                    = _rightControllerDriver.enabled
                        = false;

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

        public void ScaleCharacter()
        {
            float scaleFactor = _player.AnimationRig.AvatarEyeHeight / HeadsetStandingHeight;
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
