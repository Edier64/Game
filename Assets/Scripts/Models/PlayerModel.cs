using System;
using System.Collections.Generic;

namespace HorrorGame.Models
{
    /// <summary>
    /// Stores all data related to the player state (health, stamina, sanity, inventory).
    /// Pure data class – no Unity or MonoBehaviour dependencies.
    /// </summary>
    [Serializable]
    public class PlayerModel
    {
        // --- Events ---
        public event Action<float> OnHealthChanged;
        public event Action<float> OnStaminaChanged;
        public event Action<float> OnSanityChanged;
        public event Action        OnPlayerDied;

        // --- Constants ---
        public const float MaxHealth  = 100f;
        public const float MaxStamina = 100f;
        public const float MaxSanity  = 100f;

        // --- Private backing fields ---
        private float _health;
        private float _stamina;
        private float _sanity;
        private bool  _isAlive;

        // --- Public properties ---
        public float Health
        {
            get => _health;
            set
            {
                _health = Math.Clamp(value, 0f, MaxHealth);
                OnHealthChanged?.Invoke(_health);
                if (_health <= 0f && _isAlive) Die();
            }
        }

        public float Stamina
        {
            get => _stamina;
            set
            {
                _stamina = Math.Clamp(value, 0f, MaxStamina);
                OnStaminaChanged?.Invoke(_stamina);
            }
        }

        public float Sanity
        {
            get => _sanity;
            set
            {
                _sanity = Math.Clamp(value, 0f, MaxSanity);
                OnSanityChanged?.Invoke(_sanity);
            }
        }

        public bool IsAlive     => _isAlive;
        public bool IsRunning   { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsHiding    { get; set; }

        public List<ItemModel> Inventory { get; private set; }
        public int             MaxInventorySlots { get; private set; }

        // --- Constructor ---
        public PlayerModel(int maxInventorySlots = 6)
        {
            _health           = MaxHealth;
            _stamina          = MaxStamina;
            _sanity           = MaxSanity;
            _isAlive          = true;
            MaxInventorySlots = maxInventorySlots;
            Inventory         = new List<ItemModel>();
        }

        // --- Methods ---
        public void TakeDamage(float amount)
        {
            if (!_isAlive || amount <= 0f) return;
            Health -= amount;
        }

        public void Heal(float amount)
        {
            if (!_isAlive || amount <= 0f) return;
            Health += amount;
        }

        public bool AddItem(ItemModel item)
        {
            if (item == null || Inventory.Count >= MaxInventorySlots) return false;
            Inventory.Add(item);
            return true;
        }

        public bool RemoveItem(ItemModel item)
        {
            return Inventory.Remove(item);
        }

        private void Die()
        {
            _isAlive = false;
            OnPlayerDied?.Invoke();
        }

        public void Reset()
        {
            _health  = MaxHealth;
            _stamina = MaxStamina;
            _sanity  = MaxSanity;
            _isAlive = true;
            Inventory.Clear();
        }
    }
}
