using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class PoolManager : PersistentSingleton<PoolManager>
    {
        private readonly Dictionary<string, object> _pools = new();
        private void CreatePool<T>(T prefab, string poolName, int initialSize) where T : MonoBehaviour
        {
            if (_pools.ContainsKey(poolName)) return;

            var pool = new Queue<T>();
            _pools[poolName] = pool;

            for (var i = 0; i < initialSize; i++)
            {
                var newObject = Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                pool.Enqueue(newObject);
            }
        }
        public T GetFromPool<T>(T prefab, string poolName, Transform parent = null) where T : MonoBehaviour
        {

            if (!_pools.ContainsKey(poolName))
                CreatePool(prefab, poolName, 10);

            var pool = _pools[poolName] as Queue<T>;

            if (pool.Count == 0)
            {
                var newObject = Instantiate(prefab, parent);
                newObject.gameObject.SetActive(false);
                return newObject;
            }

            var objectToReturn = pool.Dequeue();
            objectToReturn.transform.parent = parent;
            objectToReturn.gameObject.SetActive(true);
            return objectToReturn;
        }
        public void ReturnToPool<T>(T objectToReturn, string poolName) where T : MonoBehaviour
        {
            objectToReturn.gameObject.SetActive(false);

            if (!_pools.ContainsKey(poolName))
            {
                _pools[poolName] = new Queue<T>();
            }

            var pool = _pools[poolName] as Queue<T>;
            pool?.Enqueue(objectToReturn);
        }
    }
}