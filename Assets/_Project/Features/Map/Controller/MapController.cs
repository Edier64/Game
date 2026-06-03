using UnityEngine;
using Huye.Features.Map.Model;
using Huye.Features.Map.View;

namespace Huye.Features.Map.Controller
{
    public class MapController : MonoBehaviour
    {
        private MapModel model;

        [Header("View")]
        [SerializeField] private MapView view;

        private bool playerNearby = false;

        public bool IsMapOpen => model.IsOpen;

        void Awake()
        {
            model = new MapModel();
        }

        void Start()
        {
            view.ShowMap(false);
            view.ShowPrompt(false);
        }

        void Update()
        {
            // Abrir/cerrar con E si está cerca
            if (playerNearby && Input.GetKeyDown(KeyCode.E))
            {
                ToggleMap();
            }

            // Cerrar con Escape si el mapa está abierto
            if (model.IsOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMap();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = true;
                view.ShowPrompt(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = false;
                view.ShowPrompt(false);
                
                // Si el mapa está abierto, cerrarlo automáticamente
                if (model.IsOpen)
                {
                    ToggleMap();
                }
            }
        }

        void ToggleMap()
        {
            model.IsOpen = !model.IsOpen;
            view.ShowMap(model.IsOpen);

            if (model.IsOpen)
            {
                // Abrir mapa: pausar movimiento y liberar cursor
                view.SetPlayerMovementEnabled(false);
                view.LockCursor(false);
            }
            else
            {
                // Cerrar mapa: reactivar movimiento y bloquear cursor
                view.SetPlayerMovementEnabled(true);
                view.LockCursor(true);
            }
        }
    }
}
