using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Spawning
{
    public class SpawnPoint : MonoBehaviour
    {
        private void Awake() => Destroy(transform.GetChild(0).gameObject);
    }
}