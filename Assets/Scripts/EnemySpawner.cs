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

        private EnemyFactory _enemyFactory;
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
            _enemyFactory = new EnemyFactory(_enemyPrefabList);
            _enemyPool = new ObjectPool<Enemy>();
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

        private void SpawnEnemy()
        {
            Enemy enemy;

            if (_enemyPool.IsPoolEmpty())
            {
                enemy = _enemyFactory.GetRandomEnemy();
                enemy.transform.SetParent(_enemyParentPool);
                InitializeEnemy(enemy, false);
            }
            else
            {
                enemy = _enemyPool.Get();
                InitializeEnemy(enemy, true);
            }
        }

        private void DespawnEnemy(Enemy enemy)
        {
            enemy.OnDeath -= DespawnEnemy;
            _activeEnemies.Remove(enemy);
            _enemyPool.Return(enemy);
        }

        private void InitializeEnemy(Enemy enemy, bool enemyInitialized)
        {
            _activeEnemies.Add(enemy);
            enemy.OnDeath += DespawnEnemy;

            int currentSpawnPositionIndex = UnityEngine.Random.Range(0, _spawnPositions.Count);

            if(currentSpawnPositionIndex == _lastSpawnPointIndex)
                currentSpawnPositionIndex = currentSpawnPositionIndex + 1 == _spawnPositions.Count ? 0 : currentSpawnPositionIndex + 1;

            Vector2 enemySpawnPosition = _spawnPositions[currentSpawnPositionIndex].position;
            _lastSpawnPointIndex = currentSpawnPositionIndex;

            if (enemyInitialized)
                enemy.RefreshEnemy(enemySpawnPosition);
            else
                enemy.Initialize(enemySpawnPosition);
        }
    }
}
