using UnityEngine;
using System;

public class Projectile : MonoBehaviour, IPauseable
{
    public event Action<Projectile> OnProjectileCollision;

    [Header("Projectile config")]
    [SerializeField] private float _projectileLifetime = 5f;

    [Header("Debug Fields")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _damage;
    [SerializeField] private float _projectileLifetimeCurrent;

    private Rigidbody2D _rigidbody;
    private bool _isInitialized = false;
    private bool _isPaused = false;
    private Vector2 _currentMovementDirection;

    public bool IsInitialized => _isInitialized;
    public int Damage => _damage;

    public void Initialize(Vector2 moveDirectionNormalized, float moveSpeed, int damage)
    {
        if(_isInitialized)
        {
            Debug.LogError($"Trying to initialize projectile second time!");
            return;
        }

        _moveSpeed = moveSpeed;
        _damage = damage;
        _projectileLifetimeCurrent = _projectileLifetime;

        _rigidbody = GetComponent<Rigidbody2D>();
        _isInitialized = true;

        SetDirection(moveDirectionNormalized);
    }

    private void Update()
    {
        if (_isPaused)
            return;

        if (_projectileLifetimeCurrent > 0)
            _projectileLifetimeCurrent -= Time.deltaTime;
        else
            OnProjectileCollision?.Invoke(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
            OnProjectileCollision?.Invoke(this);
        }
    }

    public void Pause()
    {
        _isPaused = true;
        _currentMovementDirection = _rigidbody.velocity;
        _rigidbody.velocity = Vector2.zero;
    }

    public void Unpause()
    {
        _isPaused = false;
        _rigidbody.velocity = _currentMovementDirection;
    }

    public void RefreshProjectile(Vector2 moveDirectionNormalized)
    {
        if(!_isInitialized)
        {
            Debug.LogError($"Trying to refresh non-initialized projectile!");
            return;
        }

        _projectileLifetimeCurrent = _projectileLifetime;
        SetDirection(moveDirectionNormalized);
    }

    private void SetDirection(Vector2 moveDirectionNormalized)
    {
        transform.right = moveDirectionNormalized;
        _rigidbody.velocity = moveDirectionNormalized * _moveSpeed;
    }
}
