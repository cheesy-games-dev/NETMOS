using KadenZombie8.BIMOS.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Rig
{
    [RequireComponent(typeof(Feet))]
    public class FeetSoundPlayer : SoundPlayer
    {
        [SerializeField]
        private AudioResource
            _walkSound,
            _runSound;

        private SmoothLocomotion _smoothLocomotion;
        private Feet _feet;

        private void Awake() => _feet = GetComponent<Feet>();

        private void Start() => _smoothLocomotion = BIMOSRig.Instance.PhysicsRig.SmoothLocomotion;

        private void OnEnable() => _feet.OnStep += Stepped;

        private void OnDisable() => _feet.OnStep -= Stepped;

        private void Stepped() => Play(_smoothLocomotion.IsRunning ? _runSound : _walkSound);
    }
}
