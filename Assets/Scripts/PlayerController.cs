using Youregone.GameplaySystems;
using UnityEngine;
using Zenject;

namespace Youregone.PlayerComponents
{
    public class PlayerController : MonoBehaviour, IPauseable
    {
        [Header("Movement Config")]
        [SerializeField] private float _moveSpeed = 10f;

        private Rigidbody2D _rigidBody;
        private PlayerInputActions _playerInputActions;
        private Vector2 _lastMovementDirection;
        private bool _isPaused;
        private PauseManager _pauseManager;

        [Inject]
        public void Construct(PauseManager pauseManager)
        {
            _pauseManager = pauseManager;
        }

        private void Awake()
        {
            if (_playerInputActions == null)
                _playerInputActions = new PlayerInputActions();

            _playerInputActions.Enable();
            _rigidBody = GetComponent<Rigidbody2D>();
            _pauseManager.RegisterPausable(this);
        }

        private void FixedUpdate()
        {
            if (_isPaused)
                return;

            MovePlayer();
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Unpause()
        {
            _isPaused = false;
        }

        private void MovePlayer()
        {
            Vector2 movementDirectionNormalized = GetMovementDirection().normalized;

            if (movementDirectionNormalized == Vector2.zero)
            {
                _rigidBody.velocity = Vector2.zero;
                return;
            }

            _rigidBody.velocity = movementDirectionNormalized * _moveSpeed;
            _lastMovementDirection = movementDirectionNormalized;
        }

        private Vector2 GetMovementDirection()
        {
            return _playerInputActions.Player.Movement.ReadValue<Vector2>();
        }
    }
}

