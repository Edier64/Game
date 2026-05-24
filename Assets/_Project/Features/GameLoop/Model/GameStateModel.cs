namespace Huye.Features.GameLoop.Model
{
    public enum GameState
    {
        MainMenu = 0,
        Playing = 1,
        Paused = 2,
        GameOver = 3,
        Victory = 4
    }

    public class GameStateModel
    {
        public GameState CurrentState = GameState.MainMenu;
        public int CollectedPuzzlePieces;
        public int TotalPuzzlePieces = 3;

        public bool ObjectiveComplete => CollectedPuzzlePieces >= TotalPuzzlePieces;
    }
}
