using Youregone.HealthComponent;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable, IPauseable
{
    public event Action<Enemy> OnDeath;
    public event Action<Enemy> OnEndReached;

    [Header("Enemy Config")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _damage;

    [Header("Debug Fields")]
    [SerializeField] private Rigidbody2D _rigidBody;

    private bool _isInitialized = false;
    private HealthSystem _healthSystem;

    public int Damage => _damage;
    public bool IsInitialized => _isInitialized;

    public void Initialize(Vector2 position)
    {
        if (_isInitialized)
        {
            Debug.LogError($"Trying to initialize enemy second time! | ({gameObject.name})");
            return;
        }

        transform.position = position;

        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.velocity = Vector2.down * _moveSpeed;

        _healthSystem = new HealthSystem(_maxHealth);
        _healthSystem.OnDeath += HealthSystem_OnDeath;

        _isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerDamageZone playerDamageZone))
        {
            OnEndReached?.Invoke(this);
        }
    }

    private void OnDisable()
    {
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public void Pause()
    {
        _rigidBody.velocity = Vector2.zero;
    }

    public void Unpause()
    {
        _rigidBody.velocity = Vector2.down * _moveSpeed;
    }

    public void TakeDamage(int damageAmount)
    {
        _healthSystem.TakeDamage(damageAmount);
    }

    public void RefreshEnemy(Vector2 position)
    {
        if (!_isInitialized)
        {
            Debug.LogError($"Trying to refresh non-initialized enemy! | ({gameObject.name})");
            return;
        }

        transform.position = position;

        _healthSystem.RefreshHealthSystem();
        _healthSystem.OnDeath += HealthSystem_OnDeath;

        _rigidBody.velocity = Vector2.down * _moveSpeed;
    }

    private void HealthSystem_OnDeath()
    {
        OnDeath?.Invoke(this);
    }
}