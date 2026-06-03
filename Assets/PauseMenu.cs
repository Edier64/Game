using UnityEngine;
using UnityEngine.SceneManagement;
using Huye.Features.Map.Controller;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Referencias")]
    [SerializeField] private MapController mapController;

    private bool isPaused = false;

    void Start()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Verificar si el mapa está abierto (si existe MapController)
            if (mapController != null && mapController.IsMapOpen)
            {
                // Si el mapa está abierto, cerrar el mapa en vez de pausar
                // MapController ya maneja ESC para cerrar el mapa
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
