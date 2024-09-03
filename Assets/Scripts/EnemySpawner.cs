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

        private ObjectPool<Enemy> _enemyPool = new();
        private EnemyFactory _enemyFactory;
        private int _lastSpawnPointIndex;

        public void Initialize()
        {
            if(_isInitialized)
            {
                Debug.LogError($"Tried to initialize EnemySpawner second time!");
                return;
            }

            _isInitialized = true;
            _enemyFactory = new EnemyFactory(_enemyPrefabList);
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
                enemy.OnEnemyReachedEnd -= Enemy_OnEnemyReachedEnd;
            }
        }

        private void SpawnEnemy()
        {
            Enemy enemy;

            if (_enemyPool.IsPoolEmpty())
            {
                enemy = _enemyFactory.GetRandomEnemy();
                enemy.transform.SetParent(_enemyParentPool);
                ActivateEnemy(enemy, false);
            }
            else
            {
                enemy = _enemyPool.Get();
                ActivateEnemy(enemy, true);
            }

            _activeEnemies.Add(enemy);
        }

        private void DespawnEnemy(Enemy enemy)
        {
            enemy.OnDeath -= DespawnEnemy;
            enemy.OnEnemyReachedEnd -= Enemy_OnEnemyReachedEnd;

            _activeEnemies.Remove(enemy);
            _enemyPool.Return(enemy);
        }

        private void ActivateEnemy(Enemy enemy, bool enemyInitialized)
        {
            enemy.OnDeath += DespawnEnemy;
            enemy.OnEnemyReachedEnd += Enemy_OnEnemyReachedEnd;

            int currentSpawnPositionIndex = UnityEngine.Random.Range(0, _spawnPositions.Count);

            if(currentSpawnPositionIndex == _lastSpawnPointIndex)
            {
                int positionCalculationsAmount = 0;
                int maxPositionCalculationAmount = 20;

                while(currentSpawnPositionIndex == _lastSpawnPointIndex)
                {
                    positionCalculationsAmount++;
                    currentSpawnPositionIndex = UnityEngine.Random.Range(0, _spawnPositions.Count);

                    if(positionCalculationsAmount > maxPositionCalculationAmount)
                    {
                        Debug.LogError($"Enemy spawn position calculation took more {maxPositionCalculationAmount} attempts!");
                        break;
                    }
                }
            }

            Vector2 enemySpawnPosition = _spawnPositions[currentSpawnPositionIndex].position;
            _lastSpawnPointIndex = currentSpawnPositionIndex;

            if (enemyInitialized)
                enemy.RefreshEnemy(enemySpawnPosition);
            else
                enemy.Initialize(enemySpawnPosition);
        }

        private void Enemy_OnEnemyReachedEnd(Enemy enemy)
        {
            DespawnEnemy(enemy);
        }
    }
}
