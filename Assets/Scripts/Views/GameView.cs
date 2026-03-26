using UnityEngine;
using HorrorGame.Models;

namespace HorrorGame.Views
{
    /// <summary>
    /// Handles global scene-level visual and audio changes:
    /// ambient sound, background music, post-processing, camera effects.
    /// </summary>
    public class GameView : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private AudioClip   _mainMenuMusic;
        [SerializeField] private AudioClip   _gameplayAmbient;
        [SerializeField] private AudioClip   _chaseMusic;
        [SerializeField] private AudioClip   _gameOverClip;
        [SerializeField] private AudioClip   _victoryClip;

        [Header("Loading")]
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private UnityEngine.UI.Slider _loadingBar;

        private GameModel _model;

        /// <summary>Called by GameController to bind model events.</summary>
        public void Initialize(GameModel model)
        {
            _model = model;
            _model.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            if (_model != null)
                _model.OnGameStateChanged -= HandleGameStateChanged;
        }

        public void ShowLoadingScreen(bool show, float progress = 0f)
        {
            if (_loadingScreen != null) _loadingScreen.SetActive(show);
            if (_loadingBar    != null) _loadingBar.value = progress;
        }

        public void PlayChaseMusic()
        {
            if (_musicSource == null || _chaseMusic == null) return;
            if (_musicSource.clip == _chaseMusic) return;
            _musicSource.clip = _chaseMusic;
            _musicSource.Play();
        }

        public void StopChaseMusic()
        {
            if (_musicSource == null) return;
            _musicSource.Stop();
            PlayAmbientForGameplay();
        }

        // --- Private helpers ---
        private void HandleGameStateChanged(GameModel.GameState state)
        {
            switch (state)
            {
                case GameModel.GameState.MainMenu:
                    PlayMusic(_mainMenuMusic);
                    break;
                case GameModel.GameState.Playing:
                    PlayAmbientForGameplay();
                    break;
                case GameModel.GameState.GameOver:
                    StopAllAudio();
                    PlayMusic(_gameOverClip);
                    break;
                case GameModel.GameState.Victory:
                    StopAllAudio();
                    PlayMusic(_victoryClip);
                    break;
            }
        }

        private void PlayMusic(AudioClip clip)
        {
            if (_musicSource == null || clip == null) return;
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        private void PlayAmbientForGameplay()
        {
            if (_ambientSource == null || _gameplayAmbient == null) return;
            _ambientSource.clip = _gameplayAmbient;
            _ambientSource.loop = true;
            _ambientSource.Play();
        }

        private void StopAllAudio()
        {
            _musicSource?.Stop();
            _ambientSource?.Stop();
        }
    }
}
