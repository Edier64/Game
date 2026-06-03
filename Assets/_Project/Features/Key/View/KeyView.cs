using UnityEngine;
using Huye.Features.Key.Model;

namespace Huye.Features.Key.View
{
    public class KeyView : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private GameObject promptUI;

        [Header("Audio")]
        [SerializeField] private AudioClip pickupSound;

        [Header("Referencias visuales")]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Collider objectCollider;

        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void ShowPrompt(bool show)
        {
            if (promptUI != null)
                promptUI.SetActive(show);
        }

        public void PlayPickupSound()
        {
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
        }

        public void HideObject()
        {
            if (meshRenderer != null)
                meshRenderer.enabled = false;

            if (objectCollider != null)
                objectCollider.enabled = false;
        }

        public void DestroyAfterDelay(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}
