using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Resetear estado del juego antes de cargar la escena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetState();
        }

        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
