using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        private readonly Dictionary<string, ObjectPool> _objectPools = new();

        private void Awake()
        {
            if (Instance)
                Destroy(this);
            else
                Instance = this;
        }

        public void RegisterPool(PoolConfig config)
        {
            if (_objectPools.ContainsKey(config.PoolID))
                return;

            _objectPools[config.PoolID] = new(config.name, config.PoolSize);
        }

        public GameObject AddToPool(string poolID, GameObject prefab)
        {
            if (!_objectPools.TryGetValue(poolID, out var objectPool))
                return null;

            var instance = Instantiate(prefab, objectPool.GameObject.transform);
            objectPool.Queue.Enqueue(instance);

            if (objectPool.Queue.Count > objectPool.PoolSize)
                Destroy(objectPool.Queue.Dequeue());

            return instance;
        }
    }

    public class ObjectPool
    {
        public GameObject GameObject = new();
        public Queue<GameObject> Queue = new();
        public int PoolSize;

        public ObjectPool(string name, int poolSize)
        {
            GameObject.transform.parent = PoolManager.Instance.transform;
            GameObject.name = name;
            PoolSize = poolSize;
        }
    }
}