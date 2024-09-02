using UnityEngine;
using System.Collections.Generic;

namespace Youregone.Factories
{
    public class EnemyFactory
    {
        private List<Enemy> _enemyPrefabList;

        public EnemyFactory(List<Enemy> enemyPrefabList)
        {
            _enemyPrefabList = enemyPrefabList;
        }

        public Enemy GetRandomEnemy()
        {
            int randomEnemyIndex = UnityEngine.Random.Range(0, _enemyPrefabList.Count);
            Enemy enemy = CreateEnemy(randomEnemyIndex);

            return enemy;
        }

        private Enemy CreateEnemy(int enemyIndex)
        {
            Enemy enemy = GameObject.Instantiate(_enemyPrefabList[enemyIndex]);
            return enemy;
        }
    }
}
