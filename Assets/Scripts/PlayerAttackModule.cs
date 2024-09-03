using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Youregone.ObjectPool;
using Youregone.Factories;

namespace Youregone.PlayerComponents
{
    public class PlayerAttackModule : MonoBehaviour
    {
        [Header("Player Attack Config")]
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackCooldownMax = 1f;
        [SerializeField] private LayerMask _enemyLayerMask;

        [Header("Projectile Config")]
        [SerializeField] private int _projectileDamage;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private List<Projectile> _attackProjectilePrefab;
        [SerializeField] private Transform _projectileParent;

        private List<Projectile> _activeProjectiles = new();
        private ObjectPool<Projectile> _projectilePool = new();
        private ProjectileFactory _projectileFactory;
        private float _attackCooldownCurrent;

        public void Initialize()
        {
            _projectileFactory = new ProjectileFactory(_attackProjectilePrefab);
            _attackCooldownCurrent = _attackCooldownMax;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
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

        private void TryAttack()
        {
            Enemy targetEnemy = FindClosestTarget();

            if (targetEnemy == null)
                return;

            Vector2 attackDirectionNormalized = (targetEnemy.transform.position - transform.position).normalized;

            Projectile projectile;

            if (_projectilePool.IsPoolEmpty())
            {
                projectile = _projectileFactory.GetProjectile();
                projectile.transform.SetParent(_projectileParent);
                projectile.Initialize(attackDirectionNormalized, _projectileSpeed, _projectileDamage);
            }
            else
            {
                projectile = _projectilePool.Get();
                projectile.RefreshProjectile(attackDirectionNormalized);
            }

            projectile.OnProjectileCollision += Projectile_OnProjectileCollision;
            projectile.transform.position = transform.position;
            _activeProjectiles.Add(projectile);
        }

        private void Projectile_OnProjectileCollision(Projectile projectile)
        {
            projectile.OnProjectileCollision -= Projectile_OnProjectileCollision;
            _activeProjectiles.Remove(projectile);

            _projectilePool.Return(projectile);
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

