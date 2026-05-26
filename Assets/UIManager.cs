using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider mouseSensitivitySlider;
    public Slider audioEffectsSlider;

    [Header("Toggles")]
    public Toggle fullscreenToggle;

    [Header("Botones")]
    public Button applyButton;
    public Button exitButton;

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Panel de Confirmación")]
    public GameObject confirmationPanel;
    public Button confirmExitButton;
    public Button cancelExitButton;

    private PlayerMovement playerMovement;
    private float originalMouseSensitivity;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        // Asignar listeners
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);

        if (exitButton != null)
            exitButton.onClick.AddListener(ShowExitConfirmation);

        if (confirmExitButton != null)
            confirmExitButton.onClick.AddListener(ExitGame);

        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(HideExitConfirmation);

        // Inicializar valores
        LoadSettings();

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    void LoadSettings()
    {
        // Cargar música
        if (musicSlider != null && musicSource != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            musicSlider.value = musicVolume;
            musicSource.volume = musicVolume;
        }

        // Cargar sensibilidad del mouse
        if (mouseSensitivitySlider != null && playerMovement != null)
        {
            float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 200f);
            mouseSensitivitySlider.value = sensitivity;
            originalMouseSensitivity = playerMovement.mouseSensitivity;
        }

        // Cargar volumen de efectos
        if (audioEffectsSlider != null)
        {
            float effectsVolume = PlayerPrefs.GetFloat("AudioEffectsVolume", 0.8f);
            audioEffectsSlider.value = effectsVolume;
            AudioListener.volume = effectsVolume;
        }

        // Cargar pantalla completa
        if (fullscreenToggle != null)
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }
    }

    public void ApplySettings()
    {
        // Guardar música
        if (musicSlider != null && musicSource != null)
        {
            float musicVolume = musicSlider.value;
            musicSource.volume = musicVolume;
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }

        // Guardar sensibilidad del mouse
        if (mouseSensitivitySlider != null && playerMovement != null)
        {
            float sensitivity = mouseSensitivitySlider.value;
            playerMovement.mouseSensitivity = sensitivity;
            PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        }

        // Guardar volumen de efectos
        if (audioEffectsSlider != null)
        {
            float effectsVolume = audioEffectsSlider.value;
            AudioListener.volume = effectsVolume;
            PlayerPrefs.SetFloat("AudioEffectsVolume", effectsVolume);
        }

        // Guardar pantalla completa
        if (fullscreenToggle != null)
        {
            Screen.fullScreen = fullscreenToggle.isOn;
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        }

        PlayerPrefs.Save();
        Debug.Log("Configuración guardada");
    }

    public void ShowExitConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
    }

    public void HideExitConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    public void ExitGame()
    {
        ApplySettings();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
