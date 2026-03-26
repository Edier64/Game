using System;
using System.Collections.Generic;

namespace HorrorGame.Core
{
    /// <summary>
    /// Simple type-safe event bus. Decouples MVC layers without direct references.
    /// Usage:
    ///   EventManager.Listen(GameEvents.PlayerDied, MyCallback);
    ///   EventManager.Trigger(GameEvents.PlayerDied);
    ///   EventManager.Unlisten(GameEvents.PlayerDied, MyCallback);
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<string, Action> _listeners = new();

        public static void Listen(string eventName, Action callback)
        {
            if (callback == null) return;
            if (!_listeners.TryAdd(eventName, callback))
                _listeners[eventName] += callback;
        }

        public static void Unlisten(string eventName, Action callback)
        {
            if (callback == null || !_listeners.ContainsKey(eventName)) return;
            _listeners[eventName] -= callback;
        }

        public static void Trigger(string eventName)
        {
            if (_listeners.TryGetValue(eventName, out Action action))
                action?.Invoke();
        }

        public static void Clear()
        {
            _listeners.Clear();
        }
    }

    /// <summary>
    /// Centralised list of event name constants to avoid magic strings.
    /// </summary>
    public static class GameEvents
    {
        public const string PlayerDied      = "PlayerDied";
        public const string EnemyDied       = "EnemyDied";
        public const string ItemPickedUp    = "ItemPickedUp";
        public const string LevelComplete   = "LevelComplete";
        public const string TogglePause     = "TogglePause";
        public const string ToggleInventory = "ToggleInventory";
        public const string ObjectiveUpdate = "ObjectiveUpdate";
        public const string PlayerSpotted   = "PlayerSpotted";
        public const string PlayerHidden    = "PlayerHidden";
    }
}
