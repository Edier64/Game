using UnityEngine;
using HorrorGame.Models;
using HorrorGame.Views;
using HorrorGame.Core;
using HorrorGame.Utils;

namespace HorrorGame.Controllers
{
    /// <summary>
    /// Handles player input, updates PlayerModel and drives PlayerView.
    /// Attached to the Player GameObject.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerView))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed    = 4f;
        [SerializeField] private float _runSpeed     = 8f;
        [SerializeField] private float _crouchSpeed  = 2f;
        [SerializeField] private float _gravity      = -9.81f;

        [Header("Look Settings")]
        [SerializeField] private float _mouseSensitivity = 2f;
        /// <summary>First-person camera child Transform of the Player prefab.</summary>
        [SerializeField] private Transform _cameraTransform;

        [Header("Stamina")]
        [SerializeField] private float _staminaDrainRate   = 15f;
        [SerializeField] private float _staminaRegenRate   = 8f;

        [Header("Interaction")]
        [SerializeField] private float _interactionDistance = 2.5f;
        [SerializeField] private LayerMask _interactableLayer;

        // --- Internal ---
        private CharacterController _characterController;
        private PlayerView          _view;
        private PlayerModel         _model;

        private Vector3 _velocity;
        private float   _xRotation;
        private bool    _flashlightOn = true;

        // --- Properties (readable by other controllers) ---
        public PlayerModel Model => _model;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _view                = GetComponent<PlayerView>();
            _model               = new PlayerModel();
            _view.Initialize(_model);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        private void Update()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.GameModel.CurrentState != GameModel.GameState.Playing)
                return;

            HandleMovement();
            HandleMouseLook();
            HandleActions();
        }

        // --- Movement ---
        private void HandleMovement()
        {
            bool isGrounded = _characterController.isGrounded;
            if (isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical   = Input.GetAxis("Vertical");

            bool isCrouching = Input.GetKey(KeyCode.LeftControl);
            bool isRunning   = Input.GetKey(KeyCode.LeftShift) && _model.Stamina > 0f && !isCrouching;

            float speed = isCrouching ? _crouchSpeed :
                          isRunning   ? _runSpeed    : _walkSpeed;

            Vector3 move = transform.right   * horizontal +
                           transform.forward * vertical;
            _characterController.Move(move * (speed * Time.deltaTime));

            // Stamina drain / regen
            if (isRunning && move.magnitude > 0f)
                _model.Stamina -= _staminaDrainRate * Time.deltaTime;
            else
                _model.Stamina += _staminaRegenRate * Time.deltaTime;

            // Gravity
            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);

            _model.IsRunning   = isRunning;
            _model.IsCrouching = isCrouching;

            float speedNorm = move.magnitude * (isRunning ? 1f : 0.5f);
            _view.SetMovementAnimation(speedNorm, isRunning, isCrouching, _model.IsHiding);
        }

        // --- Mouse look ---
        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            _xRotation -= mouseY;
            _xRotation  = Mathf.Clamp(_xRotation, -90f, 90f);

            if (_cameraTransform != null)
                _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }

        // --- Actions ---
        private void HandleActions()
        {
            // Flashlight toggle
            if (Input.GetKeyDown(KeyCode.F))
            {
                _flashlightOn = !_flashlightOn;
                _view.SetFlashlightActive(_flashlightOn);
            }

            // Inventory toggle
            if (Input.GetKeyDown(KeyCode.Tab))
                EventManager.Trigger(GameEvents.ToggleInventory);

            // Pause
            if (Input.GetKeyDown(KeyCode.Escape))
                EventManager.Trigger(GameEvents.TogglePause);

            // Interact
            if (Input.GetKeyDown(KeyCode.E))
                TryInteract();
        }

        private void TryInteract()
        {
            if (_cameraTransform == null) return;

            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward,
                                out RaycastHit hit, _interactionDistance, _interactableLayer))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact(this);
            }
        }

        // --- Public API (called by EnemyController or traps) ---
        public void TakeDamage(float amount) => _model.TakeDamage(amount);
        public void Heal(float amount)       => _model.Heal(amount);
        public bool PickupItem(ItemModel item) => _model.AddItem(item);
    }
}
