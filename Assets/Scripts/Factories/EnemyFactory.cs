using UnityEngine;

namespace Youregone.Factories
{
    public class EnemyFactory : IFactory<Enemy>
    {
        public Enemy Create(Enemy enemyPrefab, Vector2 position)
        {
            Enemy enemy = GameObject.Instantiate(enemyPrefab, position, Quaternion.identity);
            return enemy;
        }
    }
}