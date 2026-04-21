using Huye.Features.GameLoop.Model;
using UnityEngine;

namespace Huye.Features.GameLoop.View
{
    public class GameLoopView : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        public void Render(GameState state)
        {
            bool showGameOver = state == GameState.GameOver;
            bool showVictory = state == GameState.Victory;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(showGameOver);
            }

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(showVictory);
            }
        }
    }
}
