using UnityEngine;

public class BarnDoor : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptUI;         // "Presiona E para abrir el granero"
    public GameObject noKeyUI;          // "Necesitas la llave"
    public GameObject winUI;            // Panel de Victoria
    public AudioClip openSound;
    public AudioClip winSound;

    private bool playerNearby = false;
    private bool isOpen = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (promptUI != null) promptUI.SetActive(false);
        if (noKeyUI != null) noKeyUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);
    }

    void Update()
    {
        if (isOpen || !playerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.hasKey)
                OpenBarn();
            else
                ShowNoKeyFeedback();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptUI != null) promptUI.SetActive(false);
            if (noKeyUI != null) noKeyUI.SetActive(false);
        }
    }

    void OpenBarn()
    {
        isOpen = true;
        if (promptUI != null) promptUI.SetActive(false);

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        // Girar puerta (animación simple)
        StartCoroutine(OpenDoorAnimation());
    }

    System.Collections.IEnumerator OpenDoorAnimation()
    {
        float elapsed = 0f;
        float duration = 1.5f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, -100f, 0f);

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;

        // Mostrar pantalla de victoria
        if (winUI != null) winUI.SetActive(true);
        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        // Bloquear al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) player.GetComponent<PlayerMovement>().enabled = false;

        // Volver al inicio después de 4 segundos
        Invoke(nameof(BackToMenu), 4f);
    }

    void ShowNoKeyFeedback()
    {
        if (noKeyUI != null)
        {
            noKeyUI.SetActive(true);
            Invoke(nameof(HideNoKey), 2f);
        }
    }

    void HideNoKey() { if (noKeyUI != null) noKeyUI.SetActive(false); }

    void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}