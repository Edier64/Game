using UnityEngine;

namespace Huye.Features.Flashlight.View
{
    public class FlashlightView : MonoBehaviour
    {
        [SerializeField] private Light flashlight;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip toggleClip;
        [SerializeField] private AudioClip batteryEmptyClip;
        [SerializeField] private float normalIntensity = 1.5f;
        [SerializeField] private float lowBatteryIntensity = 0.9f;

        public void SetEnabled(bool enabled)
        {
            if (flashlight != null)
            {
                flashlight.enabled = enabled;
            }
        }

        public void SetLowBatteryVisual(bool lowBattery)
        {
            if (flashlight != null)
            {
                flashlight.intensity = lowBattery ? lowBatteryIntensity : normalIntensity;
            }
        }

        public void PlayToggleSound()
        {
            PlayOneShot(toggleClip);
        }

        public void PlayBatteryEmptySound()
        {
            PlayOneShot(batteryEmptyClip);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (audioSource == null || clip == null)
            {
                return;
            }

            audioSource.PlayOneShot(clip);
        }
    }
}
