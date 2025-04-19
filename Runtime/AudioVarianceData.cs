using UnityEngine;

namespace BIMOS
{
    [CreateAssetMenu(fileName = "AudioVarianceData", menuName = "BIMOS/Audio Variance Data")]
    public class AudioVarianceData : ScriptableObject
    {
        public AudioClip[] AudioClips;

        /// <returns>A random clip from the variance data</returns>
        public AudioClip GetRandomClip() => AudioClips[Random.Range(0, AudioClips.Length)];
    }
}
