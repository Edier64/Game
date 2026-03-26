using UnityEngine;
using HorrorGame.Models;

namespace HorrorGame.Views
{
    /// <summary>
    /// Handles all visual and audio feedback for the player.
    /// Listens to PlayerModel events and drives Animator / AudioSource.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerView : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip _footstepClip;
        [SerializeField] private AudioClip _hurtClip;
        [SerializeField] private AudioClip _deathClip;
        [SerializeField] private AudioClip _breathingClip;

        [Header("Visual Effects")]
        [SerializeField] private GameObject _flashlightObject;
        [SerializeField] private ParticleSystem _bloodParticles;

        // --- Animator parameter hashes ---
        private static readonly int SpeedHash      = Animator.StringToHash("Speed");
        private static readonly int IsRunningHash  = Animator.StringToHash("IsRunning");
        private static readonly int IsCrouchHash   = Animator.StringToHash("IsCrouching");
        private static readonly int IsHidingHash   = Animator.StringToHash("IsHiding");
        private static readonly int HurtTrigger    = Animator.StringToHash("Hurt");
        private static readonly int DeathTrigger   = Animator.StringToHash("Death");

        private AudioSource _audioSource;
        private PlayerModel _model;
        private float       _previousHealth;

        private void Awake()
        {
            _animator    = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>Called by PlayerController to bind model events to view callbacks.</summary>
        public void Initialize(PlayerModel model)
        {
            _model          = model;
            _previousHealth = model.Health;
            _model.OnHealthChanged  += HandleHealthChanged;
            _model.OnSanityChanged  += HandleSanityChanged;
            _model.OnPlayerDied     += HandlePlayerDied;
        }

        private void OnDestroy()
        {
            if (_model == null) return;
            _model.OnHealthChanged  -= HandleHealthChanged;
            _model.OnSanityChanged  -= HandleSanityChanged;
            _model.OnPlayerDied     -= HandlePlayerDied;
        }

        // --- Animation helpers ---
        public void SetMovementAnimation(float speed, bool isRunning, bool isCrouching, bool isHiding)
        {
            _animator.SetFloat(SpeedHash,     speed);
            _animator.SetBool(IsRunningHash,  isRunning);
            _animator.SetBool(IsCrouchHash,   isCrouching);
            _animator.SetBool(IsHidingHash,   isHiding);
        }

        public void PlayFootstep()
        {
            if (_footstepClip != null)
                _audioSource.PlayOneShot(_footstepClip);
        }

        public void SetFlashlightActive(bool active)
        {
            if (_flashlightObject != null)
                _flashlightObject.SetActive(active);
        }

        // --- Model event handlers ---
        private void HandleHealthChanged(float health)
        {
            if (health < _previousHealth)
            {
                _animator.SetTrigger(HurtTrigger);
                if (_hurtClip != null) _audioSource.PlayOneShot(_hurtClip);
                if (_bloodParticles != null) _bloodParticles.Play();
            }
            _previousHealth = health;
        }

        private void HandleSanityChanged(float sanity)
        {
            // Loop heavy breathing when sanity is critically low
            if (sanity < 30f && _breathingClip != null && !_audioSource.isPlaying)
            {
                _audioSource.clip  = _breathingClip;
                _audioSource.loop  = true;
                _audioSource.Play();
            }
            else if (sanity >= 30f && _audioSource.clip == _breathingClip && _audioSource.isPlaying)
            {
                _audioSource.Stop();
                _audioSource.loop = false;
            }
        }

        private void HandlePlayerDied()
        {
            _animator.SetTrigger(DeathTrigger);
            if (_deathClip != null) _audioSource.PlayOneShot(_deathClip);
        }
    }
}
