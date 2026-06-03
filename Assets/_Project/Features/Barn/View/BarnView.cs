using UnityEngine;
using System.Collections;
using Huye.Features.Barn.Model;

namespace Huye.Features.Barn.View
{
    public class BarnView : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private GameObject promptUI;
        [SerializeField] private GameObject noKeyUI;
        [SerializeField] private GameObject winUI;

        [Header("Audio")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip winSound;

        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            HideAllUI();
        }

        public void ShowPrompt(bool show)
        {
            if (promptUI != null)
                promptUI.SetActive(show);
        }

        public void ShowNoKeyFeedback()
        {
            if (noKeyUI != null)
                noKeyUI.SetActive(true);
        }

        public void HideNoKeyFeedback()
        {
            if (noKeyUI != null)
                noKeyUI.SetActive(false);
        }

        public void ShowWinScreen()
        {
            if (winUI != null)
                winUI.SetActive(true);
        }

        public void PlayOpenSound()
        {
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
        }

        public void PlayWinSound()
        {
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
        }

        public IEnumerator AnimateDoorOpen(float angle, float duration)
        {
            float elapsed = 0f;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(0f, angle, 0f);

            while (elapsed < duration)
            {
                transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = endRot;
        }

        public void DisablePlayerMovement()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMovement pm = player.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.enabled = false;
            }
        }

        private void HideAllUI()
        {
            if (promptUI != null)
                promptUI.SetActive(false);

            if (noKeyUI != null)
                noKeyUI.SetActive(false);

            if (winUI != null)
                winUI.SetActive(false);
        }
    }
}
