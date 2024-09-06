using System.Collections.Generic;
using UnityEngine;
using System;

namespace Youregone.GameplaySystems
{
    public class PauseManager : IDisposable
    {
        private List<IPauseable> _pausablesList = new();
        private bool _gamePaused = false;
        private GameState _gameState; 

        public bool GamePaused => _gamePaused;

        public PauseManager(GameState gameState)
        {
            _gameState = gameState;
            _gameState.OnGameStateChanged += GameState_OnGameStateChanged;
        }

        public void Dispose()
        {
            _gameState.OnGameStateChanged -= GameState_OnGameStateChanged;
        }

        public void RegisterPausable(IPauseable pauseable)
        {
            if(_pausablesList.Contains(pauseable))
            {
                Debug.LogError($"Pause Manager already contains pausable!");
                return;
            }

            _pausablesList.Add(pauseable);
        }

        public void UnregisterPausable(IPauseable pauseable)
        {
            if(!_pausablesList.Contains(pauseable))
            {
                Debug.LogError($"Can't Unregister pausable!");
                return;
            }

            _pausablesList.Remove(pauseable);
        }

        private void PauseGame()
        {
            if(_gamePaused)
            {
                Debug.LogError("Game is already paused!");
                return;
            }

            foreach (IPauseable pauseable in _pausablesList)
            {
                pauseable.Pause();
            }

            Debug.Log("Game is paused!");
            _gamePaused = true;
        }

        private void UnpauseGame()
        {
            if (!_gamePaused)
            {
                Debug.LogError("Game is already unpaused!");
                return;
            }

            foreach (IPauseable pauseable in _pausablesList)
            {
                pauseable.Unpause();
            }

            Debug.Log("Unpausing game!");
            _gamePaused = false;
        }

        private void GameState_OnGameStateChanged(EGameState gameState)
        {
            if(gameState == EGameState.Pause || gameState == EGameState.GameOver)
            {
                PauseGame();
            }
            else
            {
                if(_gamePaused)
                    UnpauseGame();
            }
        }
    }
}