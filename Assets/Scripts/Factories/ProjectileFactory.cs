using UnityEngine;

namespace Youregone.Factories
{
    public class ProjectileFactory : IFactory<Projectile>
    {
        public Projectile Create(Projectile prefab, Vector2 position)
        {
            Projectile projectile = GameObject.Instantiate(prefab, position, Quaternion.identity);
            return projectile;
        }
    }
}