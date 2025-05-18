using KadenZombie8.BIMOS.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Rig
{
    [RequireComponent(typeof(GrabHandler))]
    public class GrabSoundPlayer : SoundPlayer
    {
        [SerializeField]
        private AudioResource
            _grabSound,
            _releaseSound;

        private GrabHandler _grabHandler;

        protected override void Awake()
        {
            base.Awake();
            _grabHandler = GetComponent<GrabHandler>();
        }

        private void OnEnable()
        {
            _grabHandler.OnGrab += Grabbed;
            _grabHandler.OnRelease += Released;
        }

        private void OnDisable()
        {
            _grabHandler.OnGrab -= Grabbed;
            _grabHandler.OnRelease -= Released;
        }

        private void Grabbed() => Play(_grabSound);

        private void Released() => Play(_releaseSound);
    }
}
