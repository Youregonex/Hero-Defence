using UnityEngine;
using System.Collections.Generic;
using Youregone.Factories;
using System;

namespace Youregone.ObjectPool
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _objectPool = new();
        private Transform _parentPoolTransform;
        protected IFactory<T> _factory;

        public ObjectPool(IFactory<T> factory, Transform parentPoolTransform)
        {
            _factory = factory;
            _parentPoolTransform = parentPoolTransform;
        }

        public T Get(T prefab, Vector2 position = default, Action<T> getAction = default)
        {
            if(_objectPool.TryDequeue(out T pooledObject))
            {
                pooledObject.gameObject.SetActive(true);

                getAction?.Invoke(pooledObject);
                return pooledObject;
            }
            else
            {
                pooledObject = _factory.Create(prefab, position);
                pooledObject.transform.SetParent(_parentPoolTransform);

                getAction?.Invoke(pooledObject);
                return pooledObject;
            }
        }

        public void Release(T pooledObject, Action<T> releaseAction = default)
        {
            pooledObject.gameObject.SetActive(false);
            releaseAction?.Invoke(pooledObject);
            _objectPool.Enqueue(pooledObject);
        }
    }
}