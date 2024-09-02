using UnityEngine;

namespace Youregone.PlayerComponents
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Config")]
        [SerializeField] private float _moveSpeed = 10f;

        private Rigidbody2D _rigidBody;
        private PlayerInputActions _playerInputActions;
        private Vector2 _lastMovementDirection;

        private void Awake()
        {
            if (_playerInputActions == null)
                _playerInputActions = new PlayerInputActions();

            _playerInputActions.Enable();

            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            MovePlayer();
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

