using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        // Mostrar cursor en el menú principal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        // Resetear estado del juego antes de cargar la escena (si existe GameManager)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetState();
        }

        // Cargar la escena del juego por nombre
        SceneManager.LoadScene("main scene");
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
