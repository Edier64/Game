using UnityEngine;
using Huye.Features.Key.Model;
using Huye.Features.Key.View;

namespace Huye.Features.Key.Controller
{
    public class KeyController : MonoBehaviour
    {
        private KeyModel model;

        [Header("View")]
        [SerializeField] private KeyView view;

        private bool playerNearby = false;

        void Awake()
        {
            model = new KeyModel();
        }

        void Start()
        {
            view.ShowPrompt(false);
        }

        void Update()
        {
            if (playerNearby && Input.GetKeyDown(KeyCode.E) && !model.IsCollected)
            {
                PickUp();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = true;
                if (!model.IsCollected)
                    view.ShowPrompt(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = false;
                view.ShowPrompt(false);
            }
        }

        void PickUp()
        {
            model.IsCollected = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.hasKey = true;
            }

            view.PlayPickupSound();
            view.ShowPrompt(false);
            view.HideObject();
            view.DestroyAfterDelay(1f);
        }
    }
}
