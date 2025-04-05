using UnityEngine;

namespace BIMOS
{
    public class Head : MonoBehaviour
    {
        private Player _player;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();
        }

        void Update()
        {
            _player.AnimationRig.Transforms.Character.position = _player.ControllerRig.CameraTransform.position - Vector3.up * 1.65f;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(_player.ControllerRig.CameraTransform.right, Vector3.up));
            _player.AnimationRig.Transforms.Character.rotation = Quaternion.Lerp(_player.AnimationRig.Transforms.Character.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
