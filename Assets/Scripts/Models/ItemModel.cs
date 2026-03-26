using System;

namespace HorrorGame.Models
{
    /// <summary>
    /// Represents a collectible or usable item in the game world.
    /// </summary>
    [Serializable]
    public class ItemModel
    {
        public enum ItemType
        {
            Key,
            HealthPack,
            Battery,
            Document,
            Weapon,
            Misc
        }

        public string   Id          { get; private set; }
        public string   Name        { get; private set; }
        public string   Description { get; private set; }
        public ItemType Type        { get; private set; }
        public bool     IsUsable    { get; private set; }
        public bool     IsConsumed  { get; private set; }

        public ItemModel(string id, string name, string description, ItemType type, bool isUsable = true)
        {
            Id          = id;
            Name        = name;
            Description = description;
            Type        = type;
            IsUsable    = isUsable;
            IsConsumed  = false;
        }

        public void Consume()
        {
            IsConsumed = true;
        }
    }
}
