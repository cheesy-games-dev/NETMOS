using Mirror;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [DefaultExecutionOrder(-1)]
    public class BIMOSRig : NetworkBehaviour
    {
        public static BIMOSRig Instance { get; private set; }

        public ControllerRig ControllerRig;
        public PhysicsRig PhysicsRig;
        public AnimationRig AnimationRig;
        public GameObject[] DisableForObservers;

        private void Awake()
        {
            if (!isLocalPlayer)
            {
                ControllerRig.gameObject.SetActive(false);
                foreach (var obj in DisableForObservers)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
                return;
            }
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }
}