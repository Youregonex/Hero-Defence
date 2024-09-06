using Youregone.HealthComponent;
using Youregone.UI;
using System;

namespace Youregone.PlayerComponents
{
    public class PlayerHealth : IDisposable
    {
        public event Action OnPlayerDeath;

        private readonly PlayerDamageZone _playerDamageZone;
        private readonly HealthSystem _healthSystem;
        private readonly PlayerHealthUI _playerHealthUI;

        public HealthSystem HealthSystem => _healthSystem;

        public PlayerHealth(PlayerDamageZone playerDamageZone, PlayerHealthUI playerHealthUI, int maxPlayerHealth)
        {
            _playerDamageZone = playerDamageZone;
            _playerDamageZone.OnEnemyEntered += PlayerDamageZone_OnEnemyEntered;

            _healthSystem = new HealthSystem(maxPlayerHealth);
            _healthSystem.OnDeath += HealthSystem_OnDeath;

            _playerHealthUI = playerHealthUI;
            _playerHealthUI.SetHealthUI(_healthSystem.CurrentHealth);
        }

        public void Dispose()
        {
            _playerDamageZone.OnEnemyEntered -= PlayerDamageZone_OnEnemyEntered;
            _healthSystem.OnDeath -= HealthSystem_OnDeath;
        }

        private void PlayerDamageZone_OnEnemyEntered(Enemy enemy)
        {
            _healthSystem.TakeDamage(enemy.Damage);
            _playerHealthUI.SetHealthUI(_healthSystem.CurrentHealth);
        }

        private void HealthSystem_OnDeath()
        {
            OnPlayerDeath?.Invoke();
        }
    }
}