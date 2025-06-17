namespace Netmos
{
    using Mirror;
    using UnityEngine;

    public class NetworkTransform : NetworkTransformHybrid
    {
        private Rigidbody rb;
        private ArticulationBody ab;
        private bool wasImmovable;

        private void Start() {
            rb = GetComponent<Rigidbody>();
            ab = GetComponent<ArticulationBody>();
            if (rb)
                wasImmovable = rb.isKinematic;
            else if(ab)
                wasImmovable = ab.immovable;
        }

        private void Update() {
            if (wasImmovable)
                return;
            if (rb)
                rb.isKinematic = !authority;
            else if(ab)
                ab.immovable = !authority;
        }
    }
}

