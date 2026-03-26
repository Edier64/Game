using UnityEngine;
using HorrorGame.Models;
using HorrorGame.Views;
using HorrorGame.Core;

namespace HorrorGame.Controllers
{
    /// <summary>
    /// Manages the overall game flow: state transitions, scoring, level progression.
    /// Coordinates GameModel and GameView; acts as the main orchestrator.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameView _gameView;
        [SerializeField] private UIView   _uiView;

        private GameModel _model;

        public GameModel Model => _model;

        private void Awake()
        {
            _model = new GameModel();
            _gameView?.Initialize(_model);

            // Subscribe to model events for UI updates
            _model.OnGameStateChanged += HandleGameStateChanged;
            _model.OnScoreChanged     += score => _uiView?.UpdateScore(score);
            _model.OnLevelChanged     += level => _uiView?.UpdateLevel(level);
        }

        private void Start()
        {
            // Register global events
            EventManager.Listen(GameEvents.PlayerDied,    OnPlayerDied);
            EventManager.Listen(GameEvents.TogglePause,   OnTogglePause);
            EventManager.Listen(GameEvents.LevelComplete, OnLevelComplete);

            _model.CurrentState = GameModel.GameState.Playing;
        }

        private void OnDestroy()
        {
            EventManager.Unlisten(GameEvents.PlayerDied,    OnPlayerDied);
            EventManager.Unlisten(GameEvents.TogglePause,   OnTogglePause);
            EventManager.Unlisten(GameEvents.LevelComplete, OnLevelComplete);

            _model.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void Update()
        {
            if (_model.CurrentState == GameModel.GameState.Playing)
                _model.SessionTime += Time.deltaTime;
        }

        // --- Event handlers ---
        private void OnPlayerDied()
        {
            _model.CurrentState = GameModel.GameState.GameOver;
        }

        private void OnTogglePause()
        {
            if (_model.CurrentState == GameModel.GameState.Playing)
            {
                _model.CurrentState  = GameModel.GameState.Paused;
                Time.timeScale       = 0f;
            }
            else if (_model.CurrentState == GameModel.GameState.Paused)
            {
                _model.CurrentState  = GameModel.GameState.Playing;
                Time.timeScale       = 1f;
            }
        }

        private void OnLevelComplete()
        {
            _model.AddScore(1000);
            _model.AdvanceLevel();
        }

        private void HandleGameStateChanged(GameModel.GameState state)
        {
            switch (state)
            {
                case GameModel.GameState.Paused:
                    _uiView?.ShowPauseScreen(true);
                    break;
                case GameModel.GameState.Playing:
                    _uiView?.ShowPauseScreen(false);
                    _uiView?.ShowGameOverScreen(false);
                    break;
                case GameModel.GameState.GameOver:
                    _uiView?.ShowGameOverScreen(true);
                    break;
                case GameModel.GameState.Victory:
                    _uiView?.ShowVictoryScreen(true);
                    break;
            }
        }

        // --- Public API ---
        public void RestartGame()
        {
            Time.timeScale = 1f;
            _model.Reset();
            SceneController.Instance?.ReloadCurrentScene();
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            _model.Reset();
            SceneController.Instance?.LoadScene("MainMenu");
        }

        public void AddScore(int points) => _model.AddScore(points);
    }
}
