using UnityEngine;
using Huye.Features.Map.Model;

namespace Huye.Features.Map.View
{
    public class MapView : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private GameObject mapUI;
        [SerializeField] private GameObject promptUI;

        public void ShowMap(bool show)
        {
            if (mapUI != null)
                mapUI.SetActive(show);
        }

        public void ShowPrompt(bool show)
        {
            if (promptUI != null)
                promptUI.SetActive(show);
        }

        public void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        public void SetPlayerMovementEnabled(bool enabled)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMovement pm = player.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.enabled = enabled;
            }
        }
    }
}
