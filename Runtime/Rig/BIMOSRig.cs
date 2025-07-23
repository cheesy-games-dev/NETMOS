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

        public override void OnStartClient()
        {
            if (isLocalPlayer) return;
            ControllerRig.gameObject.SetActive(false);
            foreach (var obj in DisableForObservers)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
            return;
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Instance = this;
        }
    }
}