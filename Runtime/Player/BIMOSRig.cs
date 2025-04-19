using UnityEngine;

namespace BIMOS
{
    public class BIMOSRig : MonoBehaviour
    {
        public static BIMOSRig Instance { get; private set; }

        public ControllerRig ControllerRig;
        public PhysicsRig PhysicsRig;
        public AnimationRig AnimationRig;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }
}