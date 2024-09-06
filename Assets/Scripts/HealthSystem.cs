using System;

namespace Youregone.HealthComponent
{
    public class HealthSystem
    {
        public event Action OnDeath;

        private int _maxHealth;
        private int _currentHealth;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        public HealthSystem(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            if (damageAmount <= 0 || _currentHealth <= 0)
                return;

            if(damageAmount >= _currentHealth)
            {
                _currentHealth = 0;
                OnDeath?.Invoke();
            }
            else
            {
                _currentHealth -= damageAmount;
            }
        }

        public void RefreshHealthSystem()
        {
            _currentHealth = _maxHealth;
        }
    }
}
