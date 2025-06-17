namespace Netmos
{
    using Mirror;
    using UnityEngine;

    public class NetworkTransform : NetworkTransformHybrid
    {
        private Rigidbody rb;
        private ArticulationBody ab;

        private void Start() {
            rb = GetComponent<Rigidbody>();
            ab = GetComponent<ArticulationBody>();
        }

        private void Update() {
            if (rb)
                rb.isKinematic = !authority;
            else if(ab)
                ab.immovable = !authority;
        }
    }
}

