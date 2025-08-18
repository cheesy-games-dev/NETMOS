using Mirror;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Spawning
{
    public class SpawnPoint : NetworkStartPosition
    {
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(0.2f, 2f, 0.3f));
        }
    }
}