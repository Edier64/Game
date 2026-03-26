using UnityEngine;
using HorrorGame.Models;

namespace HorrorGame.Views
{
    /// <summary>
    /// Handles all visual and audio feedback for an enemy.
    /// Driven by EnemyModel state change events.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class EnemyView : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip _idleClip;
        [SerializeField] private AudioClip _alertClip;
        [SerializeField] private AudioClip _chaseClip;
        [SerializeField] private AudioClip _attackClip;
        [SerializeField] private AudioClip _deathClip;

        [Header("Visual FX")]
        [SerializeField] private GameObject _alertIndicator;
        [SerializeField] private ParticleSystem _deathParticles;

        // --- Animator parameter hashes ---
        private static readonly int StateHash        = Animator.StringToHash("State");
        private static readonly int SpeedHash        = Animator.StringToHash("Speed");
        private static readonly int AttackTrigger    = Animator.StringToHash("Attack");
        private static readonly int DeathTrigger     = Animator.StringToHash("Death");

        private AudioSource _audioSource;
        private EnemyModel  _model;

        private void Awake()
        {
            _animator    = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>Called by EnemyController to bind model events.</summary>
        public void Initialize(EnemyModel model)
        {
            _model = model;
            _model.OnStateChanged += HandleStateChanged;
            _model.OnEnemyDied    += HandleEnemyDied;
        }

        private void OnDestroy()
        {
            if (_model == null) return;
            _model.OnStateChanged -= HandleStateChanged;
            _model.OnEnemyDied    -= HandleEnemyDied;
        }

        public void SetMovementSpeed(float speed)
        {
            _animator.SetFloat(SpeedHash, speed);
        }

        public void PlayAttackAnimation()
        {
            _animator.SetTrigger(AttackTrigger);
            if (_attackClip != null) _audioSource.PlayOneShot(_attackClip);
        }

        private void HandleStateChanged(EnemyModel.EnemyState state)
        {
            _animator.SetInteger(StateHash, (int)state);

            if (_alertIndicator != null)
                _alertIndicator.SetActive(state == EnemyModel.EnemyState.Alert ||
                                          state == EnemyModel.EnemyState.Chase);

            AudioClip clip = state switch
            {
                EnemyModel.EnemyState.Idle   => _idleClip,
                EnemyModel.EnemyState.Alert  => _alertClip,
                EnemyModel.EnemyState.Chase  => _chaseClip,
                _                            => null
            };

            if (clip != null && !_audioSource.isPlaying)
                _audioSource.PlayOneShot(clip);
        }

        private void HandleEnemyDied()
        {
            _animator.SetTrigger(DeathTrigger);
            if (_deathClip != null)    _audioSource.PlayOneShot(_deathClip);
            if (_deathParticles != null) _deathParticles.Play();
        }
    }
}
