using UnityEngine;
using HorrorGame.Models;
using HorrorGame.Views;
using HorrorGame.Controllers;

namespace HorrorGame.Core
{
    /// <summary>
    /// Singleton that acts as the central access point for the top-level MVC components.
    /// Persists across scene loads.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // --- Singleton ---
        public static GameManager Instance { get; private set; }

        // --- References (set in Inspector or auto-found) ---
        [Header("Controllers")]
        [SerializeField] private GameController _gameController;
        [SerializeField] private UIController   _uiController;

        // --- Public accessors ---
        public GameModel GameModel => _gameController != null ? _gameController.Model : null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
