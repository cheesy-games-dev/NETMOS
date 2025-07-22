using Mirror;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Spawning
{
    public class SpawnPoint : NetworkStartPosition
    {
        private void Start() => Destroy(transform.GetChild(0).gameObject);
    }
}