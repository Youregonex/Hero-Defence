using Youregone.PlayerComponents;
using UnityEngine;
using System;

namespace Youregone.GameplaySystems
{
    public class GameState : IDisposable
    {
        public event Action<EGameState> OnGameStateChanged;

        private EGameState _currentGameState;
        private PlayerHealth _playerHealth;

        public EGameState CurrentGameState => _currentGameState;

        public GameState(PlayerHealth playerHealth)
        {
            _playerHealth = playerHealth;
            _playerHealth.OnPlayerDeath += PlayerHealth_OnPlayerDeath;

            SetGameState(EGameState.Gameplay);
        }

        public void Dispose()
        {
            _playerHealth.OnPlayerDeath -= PlayerHealth_OnPlayerDeath;
        }

        private void PlayerHealth_OnPlayerDeath()
        {
            SetGameState(EGameState.GameOver);
            Debug.Log("Game over!");
        }

        public void SetGameState(EGameState gameState)
        {
            if(gameState == _currentGameState)
                return;

            _currentGameState = gameState;
            OnGameStateChanged?.Invoke(_currentGameState);
        }
    }
}