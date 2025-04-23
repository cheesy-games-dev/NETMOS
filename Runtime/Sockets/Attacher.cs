using UnityEngine;

namespace BIMOS
{
    [AddComponentMenu("BIMOS/Attacher")]
    public class Attacher : MonoBehaviour
    {
        public string[] Tags;

        public Grabbable[] EnableGrabs, DisableGrabs;

        [HideInInspector]
        public Rigidbody Rigidbody;

        [HideInInspector]
        public Socket Socket;

        private void Awake() => Rigidbody = GetComponentInParent<Rigidbody>();

        public bool IsGrabbed()
        {
            foreach (Grabbable grab in Rigidbody.GetComponentsInChildren<Grabbable>())
                if (grab.LeftHand || grab.RightHand)
                    return true;

            return false;
        }

        public void AttemptDetach()
        {
            if (!Socket)
                return;

            Socket.Detach();
        }
    }
}
