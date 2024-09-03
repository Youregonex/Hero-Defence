using UnityEngine;
using System.Collections.Generic;

namespace Youregone.Factories
{
    public class ProjectileFactory
    {
        private List<Projectile> _projectilePrefabList;

        public ProjectileFactory(List<Projectile> projectilePrefabs)
        {
            _projectilePrefabList = projectilePrefabs;
        }

        public Projectile GetProjectile()
        {
            int randomIndex = UnityEngine.Random.Range(0, _projectilePrefabList.Count);
            Projectile projectile = CreateProjectile(randomIndex);

            return projectile;
        }

        private Projectile CreateProjectile(int randomIndex)
        {
            Projectile projectile = GameObject.Instantiate(_projectilePrefabList[randomIndex]);
            return projectile;
        }
    }
}