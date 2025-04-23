using UnityEngine;

namespace BIMOS
{
    /// <summary>
    /// Sends haptic impulses to specified grabs relating to the grabbable
    /// </summary>
    public class GrabHapticsHandler : MonoBehaviour
    {
        [SerializeField]
        private Grabbable[] _grabs;

        /// <summary>
        /// Sends haptic impulses to each of the defined grabs
        /// </summary>
        /// <param name="amplitude">The amplitude of the impulse</param>
        /// <param name="duration">The duration of the impulse</param>
        public void SendHapticImpulse(float amplitude, float duration)
        {
            foreach (Grabbable grab in _grabs) {
                if (grab.LeftHand)
                    grab.LeftHand.SendHapticImpulse(amplitude, duration);
                if (grab.RightHand)
                    grab.RightHand.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
