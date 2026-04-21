using Huye.Features.Enemy.Spider.Controller;
using Huye.Features.Enemy.Wendigo.Controller;
using Huye.Features.GameLoop.Model;
using Huye.Features.GameLoop.View;
using Huye.Features.Player.Controller;
using UnityEngine;

namespace Huye.Features.GameLoop.Controller
{
    public class GameLoopController : MonoBehaviour
    {
        [SerializeField] private GameLoopView view;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpiderController spiderController;
        [SerializeField] private WendigoController wendigoController;

        private readonly GameStateModel _model = new GameStateModel();

        private void Start()
        {
            if (spiderController != null)
            {
                spiderController.PlayerKilled += OnPlayerKilled;
            }

            if (wendigoController != null)
            {
                wendigoController.PlayerKilled += OnPlayerKilled;
            }

            Render();
        }

        private void OnDestroy()
        {
            if (spiderController != null)
            {
                spiderController.PlayerKilled -= OnPlayerKilled;
            }

            if (wendigoController != null)
            {
                wendigoController.PlayerKilled -= OnPlayerKilled;
            }
        }

        public void RegisterPuzzlePiece()
        {
            _model.CollectedPuzzlePieces++;
            if (_model.ObjectiveComplete)
            {
                SetState(GameState.Victory);
            }
        }

        private void OnPlayerKilled()
        {
            SetState(GameState.GameOver);
        }

        private void SetState(GameState newState)
        {
            _model.CurrentState = newState;

            bool isPlaying = newState == GameState.Playing;
            if (playerController != null)
            {
                playerController.SetControlEnabled(isPlaying);
            }

            Time.timeScale = isPlaying ? 1f : 0f;
            Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isPlaying;

            Render();
        }

        private void Render()
        {
            if (view != null)
            {
                view.Render(_model.CurrentState);
            }
        }
    }
}
