using UnityEngine;

namespace KadenZombie8.Pooling
{
    [CreateAssetMenu(fileName = "Pool Config", menuName = "BIMOS/Object Pool Config")]
    public class PoolConfig : ScriptableObject
    {
        public string PoolID = "Pool";
        public int PoolSize = 30;
    }
}