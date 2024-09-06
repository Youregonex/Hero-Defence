using System.Collections.Generic;
using Youregone.ObjectPool;
using Youregone.Factories;
using UnityEngine;
using Zenject;

namespace Youregone.GameplaySystems
{
    public class EnemySpawner : MonoBehaviour, IPauseable
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
        [SerializeField] private float _enemySpawnTimerCurrent;

        private PauseManager _pauseManager;
        private GameState _gameState;
        private bool _isPaused;
        private ObjectPool<Enemy> _enemyPool;
        private int _lastSpawnPointIndex;

        [Inject]
        public void Construct(PauseManager pauseManager, GameState gameState)
        {
            _pauseManager = pauseManager;
            _gameState = gameState;
        }

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(new EnemyFactory(), _enemyParentPool);
            _enemySpawnTimerCurrent = UnityEngine.Random.Range(_enemySpawnTimerMin, _enemySpawnTimerMax);

            _pauseManager.RegisterPausable(this);
        }

        private void Update()
        {
            if (_isPaused || _gameState.CurrentGameState != EGameState.Gameplay)
                return;

            if (_enemySpawnTimerCurrent > 0)
                _enemySpawnTimerCurrent -= Time.deltaTime;
            else
            {
                _enemySpawnTimerCurrent = UnityEngine.Random.Range(_enemySpawnTimerMin, _enemySpawnTimerMax);
                SpawnEnemy();
            }
        }

        private void OnDestroy()
        {
            foreach (Enemy enemy in _activeEnemies)
            {
                enemy.OnDeath -= DespawnEnemy;
                enemy.OnEndReached -= DespawnEnemy;
            }
        }

        public void Pause()
        {
            _isPaused = true;

            foreach (var enemy in _activeEnemies)
            {
                enemy.Pause();
            }
        }

        public void Unpause()
        {
            _isPaused = false;

            foreach (var enemy in _activeEnemies)
            {
                enemy.Unpause();
            }
        }

        private void SpawnEnemy()
        {
            Vector2 enemySpawnPosition = GetNextSpawnPosition();

            Enemy enemy = _enemyPool.Get(_enemyPrefabList[0], enemySpawnPosition);
            ActivateEnemy(enemy, enemySpawnPosition);
        }

        private void DespawnEnemy(Enemy enemy)
        {
            enemy.OnDeath -= DespawnEnemy;
            enemy.OnEndReached -= DespawnEnemy;

            _activeEnemies.Remove(enemy);
            _enemyPool.Release(enemy);
        }

        private void ActivateEnemy(Enemy enemy, Vector2 spawnPosition)
        {
            _activeEnemies.Add(enemy);

            enemy.OnDeath += DespawnEnemy;
            enemy.OnEndReached += DespawnEnemy;

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
                int positionCalculationAmountCurrent = 0;
                int positionCalculationAmountMax = 20;

                while (currentSpawnPositionIndex == _lastSpawnPointIndex)
                {
                    positionCalculationAmountCurrent++;
                    currentSpawnPositionIndex = UnityEngine.Random.Range(0, _spawnPositions.Count);

                    if (positionCalculationAmountCurrent > positionCalculationAmountMax)
                    {
                        Debug.LogError($"Enemy spawn position calculation took more than {positionCalculationAmountMax} attempts!");
                        break;
                    }
                }
            }

            Vector2 enemySpawnPosition = _spawnPositions[currentSpawnPositionIndex].position;
            _lastSpawnPointIndex = currentSpawnPositionIndex;

            return enemySpawnPosition;
        }
    }
}