using UnityEngine;
using HorrorGame.Models;
using HorrorGame.Views;
using HorrorGame.Core;

namespace HorrorGame.Controllers
{
    /// <summary>
    /// Manages all UI interactions: pause menu buttons, HUD updates,
    /// inventory display, and in-game prompts.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIView   _uiView;
        [SerializeField] private GameController _gameController;

        private PlayerModel _playerModel;
        private bool _inventoryOpen;

        // Store delegate references for proper unsubscription
        private System.Action<float> _onHealthChanged;
        private System.Action<float> _onStaminaChanged;
        private System.Action<float> _onSanityChanged;

        private void Start()
        {
            // Get player model from player controller in scene
            var playerCtrl = FindFirstObjectByType<PlayerController>();
            if (playerCtrl != null)
            {
                _playerModel      = playerCtrl.Model;
                _onHealthChanged  = h => _uiView?.UpdateHealth(h, PlayerModel.MaxHealth);
                _onStaminaChanged = s => _uiView?.UpdateStamina(s, PlayerModel.MaxStamina);
                _onSanityChanged  = s => _uiView?.UpdateSanity(s, PlayerModel.MaxSanity);

                _playerModel.OnHealthChanged  += _onHealthChanged;
                _playerModel.OnStaminaChanged += _onStaminaChanged;
                _playerModel.OnSanityChanged  += _onSanityChanged;
                _playerModel.OnPlayerDied     += OnPlayerDied;

                // Initialise bars at full values
                _uiView?.UpdateHealth(_playerModel.Health,   PlayerModel.MaxHealth);
                _uiView?.UpdateStamina(_playerModel.Stamina, PlayerModel.MaxStamina);
                _uiView?.UpdateSanity(_playerModel.Sanity,   PlayerModel.MaxSanity);
            }

            EventManager.Listen(GameEvents.ToggleInventory, OnToggleInventory);
        }

        private void OnDestroy()
        {
            EventManager.Unlisten(GameEvents.ToggleInventory, OnToggleInventory);

            if (_playerModel != null)
            {
                _playerModel.OnHealthChanged  -= _onHealthChanged;
                _playerModel.OnStaminaChanged -= _onStaminaChanged;
                _playerModel.OnSanityChanged  -= _onSanityChanged;
                _playerModel.OnPlayerDied     -= OnPlayerDied;
            }
        }

        // --- Event handlers ---
        private void OnPlayerDied()
        {
            _uiView?.ShowGameOverScreen(true);
        }

        private void OnToggleInventory()
        {
            _inventoryOpen = !_inventoryOpen;
            _uiView?.ShowInventory(_inventoryOpen);
            Time.timeScale = _inventoryOpen ? 0f : 1f;
        }

        // --- Button callbacks (wired in Inspector) ---
        public void OnResumeButtonClicked()  => EventManager.Trigger(GameEvents.TogglePause);
        public void OnRestartButtonClicked() => _gameController?.RestartGame();
        public void OnMainMenuButtonClicked()=> _gameController?.ReturnToMainMenu();
        public void OnQuitButtonClicked()    => Application.Quit();

        // --- Prompts ---
        public void ShowInteractionPrompt(string text)  => _uiView?.ShowInteractionPrompt(text);
        public void HideInteractionPrompt()             => _uiView?.ShowInteractionPrompt(string.Empty);
        public void SetObjectiveText(string objective)  => _uiView?.SetObjectiveText(objective);
    }
}
