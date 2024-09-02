using UnityEngine;
using System.Collections.Generic;

namespace Youregone.ObjectPool
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _objectPool = new();

        public T Get()
        {
            if (IsPoolEmpty())
                return null;

            T pooledObject = _objectPool.Dequeue();
            pooledObject.gameObject.SetActive(true);

            return pooledObject;
        }

        public void Return(T poolObject)
        {
            poolObject.gameObject.SetActive(false);
            _objectPool.Enqueue(poolObject);
        }

        public bool IsPoolEmpty() => _objectPool.Count == 0 ? true : false;
    }
}
