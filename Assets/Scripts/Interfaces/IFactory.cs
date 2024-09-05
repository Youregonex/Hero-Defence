using UnityEngine;

namespace Youregone.Factories
{
    public interface IFactory<T>
    {
        public T Create(T prefab, Vector2 position);
    }
}