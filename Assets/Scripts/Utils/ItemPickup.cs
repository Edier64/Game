using UnityEngine;
using HorrorGame.Models;
using HorrorGame.Controllers;
using HorrorGame.Core;

namespace HorrorGame.Utils
{
    /// <summary>
    /// Represents a pickup in the world. Attach to any pickup GameObject.
    /// Implements IInteractable so the player can collect it via the E key.
    /// </summary>
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [Header("Item Data")]
        [SerializeField] private string        _itemId;
        [SerializeField] private string        _itemName;
        [SerializeField] private string        _itemDescription;
        [SerializeField] private ItemModel.ItemType _itemType;
        [SerializeField] private bool          _isUsable = true;

        [Header("Health Pack (if applicable)")]
        [SerializeField] private float _healAmount = 30f;

        private ItemModel _itemModel;

        private void Awake()
        {
            _itemModel = new ItemModel(_itemId, _itemName, _itemDescription, _itemType, _isUsable);
        }

        public string GetPromptText() => $"Press E to pick up {_itemName}";

        public void Interact(PlayerController player)
        {
            if (player == null) return;

            // Apply immediate effect for health packs
            if (_itemType == ItemModel.ItemType.HealthPack)
                player.Heal(_healAmount);

            // Try to add to inventory
            if (player.PickupItem(_itemModel))
            {
                EventManager.Trigger(GameEvents.ItemPickedUp);
                Destroy(gameObject);
            }
        }
    }
}
