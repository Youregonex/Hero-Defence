using System.Collections.Generic;
using Youregone.GameplaySystems;
using Youregone.ObjectPool;
using Youregone.Factories;
using UnityEngine;
using System.Linq;
using Zenject;

namespace Youregone.PlayerComponents
{
    public class PlayerAttackModule : MonoBehaviour, IPauseable
    {
        [Header("Player Attack Config")]
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackCooldownMax = 1f;
        [SerializeField] private LayerMask _enemyLayerMask;

        [Header("Projectile Config")]
        [SerializeField] private int _projectileDamage;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private List<Projectile> _attackProjectilePrefabList;
        [SerializeField] private Transform _projectileParent;

        private PauseManager _pauseManager;
        private List<Projectile> _activeProjectiles = new();
        private ObjectPool<Projectile> _projectilePool;
        private float _attackCooldownCurrent;
        private bool _isPaused;

        [Inject]
        public void Construct(PauseManager pauseManager)
        {
            _pauseManager = pauseManager;
        }

        private void Awake()
        {
            _attackCooldownCurrent = _attackCooldownMax;
            _projectilePool = new ObjectPool<Projectile>(new ProjectileFactory(), _projectileParent);

            _pauseManager.RegisterPausable(this);
        }

        private void Update()
        {
            if (_isPaused)
                return;

            if (_attackCooldownCurrent > 0)
                _attackCooldownCurrent -= Time.deltaTime;
            else
            {
                _attackCooldownCurrent = _attackCooldownMax;
                TryAttack();
            }
        }

        private void OnDestroy()
        {
            foreach (Projectile projectile in _activeProjectiles)
            {
                projectile.OnProjectileCollision -= Projectile_OnProjectileCollision;
            }
        }

        public void Pause()
        {
            _isPaused = true;

            foreach (var projectile in _activeProjectiles)
            {
                projectile.Pause();
            }
        }

        public void Unpause()
        {
            _isPaused = false;

            foreach (var projectile in _activeProjectiles)
            {
                projectile.Unpause();
            }
        }

        private void TryAttack()
        {
            Enemy targetEnemy = FindClosestTarget();

            if (targetEnemy == null)
                return;

            Vector2 attackDirectionNormalized = (targetEnemy.transform.position - transform.position).normalized;

            Projectile projectile = _projectilePool.Get(
                _attackProjectilePrefabList[0],
                transform.position,
                (projectile) => { projectile.transform.position = this.transform.position; });

            if (projectile.IsInitialized)
                projectile.RefreshProjectile(attackDirectionNormalized);
            else
                projectile.Initialize(attackDirectionNormalized, _projectileSpeed, _projectileDamage);

            projectile.OnProjectileCollision += Projectile_OnProjectileCollision;
            _activeProjectiles.Add(projectile);
        }

        private void Projectile_OnProjectileCollision(Projectile projectile)
        {
            projectile.OnProjectileCollision -= Projectile_OnProjectileCollision;
            _activeProjectiles.Remove(projectile);

            _projectilePool.Release(
                projectile,
                (t) => { Debug.Log($"Returned {t.gameObject.name} to pool!"); });
        }

        private Enemy FindClosestTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayerMask);

            if (hits.Length == 0)
                return null;

            List<Collider2D> hitList = new(hits);

            Enemy closestEnemy = hitList
                .Where(t => t.TryGetComponent(out Enemy enemy))
                .OrderBy(t => Vector2.Distance(t.transform.position, transform.position))
                .FirstOrDefault()
                .GetComponent<Enemy>();

            return closestEnemy;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}