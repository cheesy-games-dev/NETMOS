using UnityEngine;
using UnityEngine.Audio;

namespace KadenZombie8.BIMOS.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class SoundPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        protected virtual void Awake() => _audioSource = GetComponent<AudioSource>();

        protected void Play() => _audioSource.Play();

        protected void Play(AudioResource resource)
        {
            _audioSource.resource = resource;
            Play();
        }
    }
}
