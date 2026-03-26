using System;
using UnityEngine;

namespace HorrorGame.Models
{
    /// <summary>
    /// Stores all data for an enemy (health, state machine, detection).
    /// Pure data class – no GameObject logic.
    /// </summary>
    [Serializable]
    public class EnemyModel
    {
        public enum EnemyState
        {
            Idle,
            Patrol,
            Alert,
            Chase,
            Attack,
            Search,
            Dead
        }

        // --- Events ---
        public event Action<EnemyState> OnStateChanged;
        public event Action             OnEnemyDied;

        // --- Constants ---
        public const float MaxHealth = 100f;

        // --- Private fields ---
        private float       _health;
        private EnemyState  _currentState;
        private bool        _isAlive;

        // --- Properties ---
        public string      Id              { get; private set; }
        public string      EnemyName       { get; private set; }
        public float       DetectionRadius { get; set; }
        public float       AttackRadius    { get; set; }
        public float       MovementSpeed   { get; set; }
        public float       ChaseSpeed      { get; set; }
        public float       AttackDamage    { get; set; }
        public bool        IsAlive         => _isAlive;
        public Vector3     LastKnownPlayerPosition { get; set; }

        public float Health
        {
            get => _health;
            set
            {
                _health = Math.Clamp(value, 0f, MaxHealth);
                if (_health <= 0f && _isAlive) Die();
            }
        }

        public EnemyState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value) return;
                _currentState = value;
                OnStateChanged?.Invoke(_currentState);
            }
        }

        // --- Constructor ---
        public EnemyModel(string id, string name,
                          float detectionRadius = 10f, float attackRadius = 2f,
                          float movementSpeed   = 3f,  float chaseSpeed   = 6f,
                          float attackDamage    = 20f)
        {
            Id              = id;
            EnemyName       = name;
            DetectionRadius = detectionRadius;
            AttackRadius    = attackRadius;
            MovementSpeed   = movementSpeed;
            ChaseSpeed      = chaseSpeed;
            AttackDamage    = attackDamage;
            _health         = MaxHealth;
            _isAlive        = true;
            _currentState   = EnemyState.Idle;
        }

        public void TakeDamage(float amount)
        {
            if (!_isAlive || amount <= 0f) return;
            Health -= amount;
        }

        private void Die()
        {
            _isAlive      = false;
            CurrentState  = EnemyState.Dead;
            OnEnemyDied?.Invoke();
        }

        public void Reset()
        {
            _health       = MaxHealth;
            _isAlive      = true;
            _currentState = EnemyState.Idle;
        }
    }
}
