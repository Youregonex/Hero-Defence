using System.Collections.Generic;
using UnityEngine;
using Youregone.Factories;
using Youregone.ObjectPool;

namespace Youregone.GameplaySystems
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawner Config")]
        [SerializeField] private List<Transform> _spawnPositions;
        [SerializeField] private float _enemySpawnTimerMin;
        [SerializeField] private float _enemySpawnTimerMax;
        [SerializeField] private Transform _enemyParentPool;

        [Header("Enemy Config")]
        [SerializeField] private List<Enemy> _enemyPrefabList;

        [Header("Debug Fields")]
        [SerializeField] private List<Enemy> _activeEnemies = new();
        [SerializeField] private bool _isInitialized = false;
        [SerializeField] private float _nextEnemySpawnTimer;

        private ObjectPool<Enemy> _enemyPool;
        private int _lastSpawnPointIndex;

        public void Initialize()
        {
            if(_isInitialized)
            {
                Debug.LogError($"Tried to initialize EnemySpawner second time!");
                return;
            }

            _isInitialized = true;
            _enemyPool = new ObjectPool<Enemy>(new EnemyFactory(), _enemyParentPool);
            _nextEnemySpawnTimer = UnityEngine.Random.Range(_enemySpawnTimerMin, _enemySpawnTimerMax);
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            if (_nextEnemySpawnTimer > 0)
                _nextEnemySpawnTimer -= Time.deltaTime;
            else
            {
                _nextEnemySpawnTimer = UnityEngine.Random.Range(_enemySpawnTimerMin, _enemySpawnTimerMax);
                SpawnEnemy();
            }
        }

        private void OnDestroy()
        {
            foreach (Enemy enemy in _activeEnemies)
            {
                enemy.OnDeath -= DespawnEnemy;
                enemy.OnEndReached -= Enemy_OnEndReached;
            }
        }

        private void SpawnEnemy()
        {
            Vector2 enemySpawnPosition = GetNextSpawnPosition();

            Enemy enemy = _enemyPool.Get(_enemyPrefabList[0], enemySpawnPosition);
            ActivateEnemy(enemy, enemySpawnPosition);
            _activeEnemies.Add(enemy);
        }

        private void DespawnEnemy(Enemy enemy)
        {
            enemy.OnDeath -= DespawnEnemy;
            enemy.OnEndReached -= Enemy_OnEndReached;

            _activeEnemies.Remove(enemy);
            _enemyPool.Release(enemy);
        }

        private void ActivateEnemy(Enemy enemy, Vector2 spawnPosition)
        {
            enemy.OnDeath += DespawnEnemy;
            enemy.OnEndReached += Enemy_OnEndReached;

            if (enemy.IsInitialized)
                enemy.RefreshEnemy(spawnPosition);
            else
                enemy.Initialize(spawnPosition);
        }

        private Vector2 GetNextSpawnPosition()
        {
            int currentSpawnPositionIndex = UnityEngine.Random.Range(0, _spawnPositions.Count);

            if (currentSpawnPositionIndex == _lastSpawnPointIndex)
            {
                int positionCalculationsAmount = 0;
                int maxPositionCalculationAmount = 20;

                while (currentSpawnPositionIndex == _lastSpawnPointIndex)
                {
                    positionCalculationsAmount++;
                    currentSpawnPositionIndex = UnityEngine.Random.Range(0, _spawnPositions.Count);

                    if (positionCalculationsAmount > maxPositionCalculationAmount)
                    {
                        Debug.LogError($"Enemy spawn position calculation took more {maxPositionCalculationAmount} attempts!");
                        break;
                    }
                }
            }

            Vector2 enemySpawnPosition = _spawnPositions[currentSpawnPositionIndex].position;
            _lastSpawnPointIndex = currentSpawnPositionIndex;

            return enemySpawnPosition;
        }

        private void Enemy_OnEndReached(Enemy enemy)
        {
            DespawnEnemy(enemy);
        }
    }
}
