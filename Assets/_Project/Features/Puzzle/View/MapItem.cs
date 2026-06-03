using UnityEngine;

public class MapItem : MonoBehaviour
{
    public GameObject mapUI;  // Panel Canvas con la imagen del mapa
    private bool isMapOpen = false;
    private bool playerNearby = false;
    public GameObject promptUI;

    void Start()
    {
        if (mapUI != null) mapUI.SetActive(false);
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
            ToggleMap();

        // Cerrar con Escape
        if (isMapOpen && Input.GetKeyDown(KeyCode.Escape))
            ToggleMap();
    }

    void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        if (mapUI != null) mapUI.SetActive(isMapOpen);

        // Pausar movimiento mientras se ve el mapa
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.GetComponent<PlayerMovement>().enabled = !isMapOpen;

        Cursor.lockState = isMapOpen ? CursorLockMode.None : CursorLockMode.Locked;
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
            if (isMapOpen) ToggleMap();
        }
    }
}