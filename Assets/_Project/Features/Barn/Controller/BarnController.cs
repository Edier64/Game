using UnityEngine;
using System.Collections;
using Huye.Features.Barn.Model;
using Huye.Features.Barn.View;
using UnityEngine.SceneManagement;

namespace Huye.Features.Barn.Controller
{
    public class BarnController : MonoBehaviour
    {
        private BarnModel model;

        [Header("View")]
        [SerializeField] private BarnView view;

        private bool playerNearby = false;

        void Awake()
        {
            model = new BarnModel();
        }

        void Start()
        {
            view.ShowPrompt(false);
        }

        void Update()
        {
            if (model.IsOpen || !playerNearby) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (model.RequiresKey && GameManager.Instance != null && !GameManager.Instance.hasKey)
                {
                    ShowNoKeyFeedback();
                }
                else
                {
                    OpenBarn();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = true;
                if (!model.IsOpen)
                    view.ShowPrompt(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerNearby = false;
                view.ShowPrompt(false);
                view.HideNoKeyFeedback();
            }
        }

        void ShowNoKeyFeedback()
        {
            view.ShowNoKeyFeedback();
            Invoke(nameof(HideNoKey), 2f);
        }

        void HideNoKey()
        {
            view.HideNoKeyFeedback();
        }

        void OpenBarn()
        {
            model.IsOpen = true;
            view.ShowPrompt(false);
            view.PlayOpenSound();
            StartCoroutine(OpenBarnSequence());
        }

        IEnumerator OpenBarnSequence()
        {
            // Animar puerta
            yield return view.AnimateDoorOpen(model.DoorOpenAngle, model.AnimationDuration);

            // Mostrar pantalla de victoria
            view.ShowWinScreen();
            view.PlayWinSound();

            // Bloquear al jugador
            view.DisablePlayerMovement();

            // Volver al menú después del tiempo especificado
            yield return new WaitForSeconds(model.WinScreenDuration);
            SceneManager.LoadScene(0);
        }
    }
}
