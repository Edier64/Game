using System;

namespace HorrorGame.Models
{
    /// <summary>
    /// Stores the global game state: current level, score, objectives and pause state.
    /// </summary>
    [Serializable]
    public class GameModel
    {
        public enum GameState
        {
            MainMenu,
            Loading,
            Playing,
            Paused,
            GameOver,
            Victory
        }

        // --- Events ---
        public event Action<GameState> OnGameStateChanged;
        public event Action<int>       OnScoreChanged;
        public event Action<int>       OnLevelChanged;

        // --- Private fields ---
        private GameState _currentState;
        private int       _score;
        private int       _currentLevel;

        // --- Properties ---
        public GameState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value) return;
                _currentState = value;
                OnGameStateChanged?.Invoke(_currentState);
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                _score = Math.Max(0, value);
                OnScoreChanged?.Invoke(_score);
            }
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = Math.Max(1, value);
                OnLevelChanged?.Invoke(_currentLevel);
            }
        }

        public int  TotalLevels          { get; private set; }
        public bool IsObjectiveCompleted { get; set; }
        public float SessionTime         { get; set; }

        // --- Constructor ---
        public GameModel(int totalLevels = 5)
        {
            TotalLevels          = totalLevels;
            _currentState        = GameState.MainMenu;
            _score               = 0;
            _currentLevel        = 1;
            IsObjectiveCompleted = false;
            SessionTime          = 0f;
        }

        public void AddScore(int points)
        {
            if (points > 0) Score += points;
        }

        public void AdvanceLevel()
        {
            if (_currentLevel < TotalLevels)
            {
                CurrentLevel++;
                IsObjectiveCompleted = false;
            }
            else
            {
                CurrentState = GameState.Victory;
            }
        }

        public void Reset()
        {
            _score               = 0;
            _currentLevel        = 1;
            _currentState        = GameState.MainMenu;
            IsObjectiveCompleted = false;
            SessionTime          = 0f;
        }
    }
}
