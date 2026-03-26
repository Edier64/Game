using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HorrorGame.Models;

namespace HorrorGame.Views
{
    /// <summary>
    /// Manages all HUD elements: health bar, stamina bar, sanity bar, objective text,
    /// inventory panel, pause screen, game-over screen and victory screen.
    /// </summary>
    public class UIView : MonoBehaviour
    {
        [Header("HUD Bars")]
        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _staminaBar;
        [SerializeField] private Slider _sanityBar;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _objectiveText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _interactionPrompt;

        [Header("Screens")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _victoryPanel;
        [SerializeField] private GameObject _inventoryPanel;

        [Header("Sanity FX")]
        [SerializeField] private CanvasGroup _sanityVignette;

        // -------------------------------------------------------
        public void UpdateHealth(float current, float max)
        {
            if (_healthBar != null)
            {
                _healthBar.maxValue = max;
                _healthBar.value    = current;
            }
        }

        public void UpdateStamina(float current, float max)
        {
            if (_staminaBar != null)
            {
                _staminaBar.maxValue = max;
                _staminaBar.value    = current;
            }
        }

        public void UpdateSanity(float current, float max)
        {
            if (_sanityBar != null)
            {
                _sanityBar.maxValue = max;
                _sanityBar.value    = current;
            }

            // Drive vignette alpha based on sanity
            if (_sanityVignette != null)
                _sanityVignette.alpha = 1f - (current / max);
        }

        public void UpdateScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = $"Score: {score}";
        }

        public void UpdateLevel(int level)
        {
            if (_levelText != null)
                _levelText.text = $"Level: {level}";
        }

        public void SetObjectiveText(string text)
        {
            if (_objectiveText != null)
                _objectiveText.text = text;
        }

        public void ShowInteractionPrompt(string prompt)
        {
            if (_interactionPrompt != null)
            {
                _interactionPrompt.text = prompt;
                _interactionPrompt.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
            }
        }

        public void ShowPauseScreen(bool show)
        {
            if (_pausePanel != null) _pausePanel.SetActive(show);
            if (_hudPanel   != null) _hudPanel.SetActive(!show);
        }

        public void ShowGameOverScreen(bool show)
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(show);
            if (_hudPanel      != null) _hudPanel.SetActive(!show);
        }

        public void ShowVictoryScreen(bool show)
        {
            if (_victoryPanel != null) _victoryPanel.SetActive(show);
            if (_hudPanel     != null) _hudPanel.SetActive(!show);
        }

        public void ShowInventory(bool show)
        {
            if (_inventoryPanel != null) _inventoryPanel.SetActive(show);
        }
    }
}
