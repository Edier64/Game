using UnityEngine;
using TMPro;  // o UnityEngine.UI si usas Text normal

public class KeyItem : MonoBehaviour
{
    [Header("Configuración")]
    public string promptText = "Presiona E para recoger la llave";
    public GameObject promptUI;   // UI de texto "Presiona E"
    public AudioClip pickupSound;

    private bool playerNearby = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
            PickUp();
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
        }
    }

    void PickUp()
    {
        GameManager.Instance.hasKey = true;

        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        if (promptUI != null) promptUI.SetActive(false);

        // Ocultar el objeto (pero no destruir para que el sonido termine)
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 1f);
    }
}