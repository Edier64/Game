namespace HorrorGame.Utils
{
    /// <summary>
    /// Interface for any world object the player can interact with (doors, items, switches).
    /// </summary>
    public interface IInteractable
    {
        void Interact(HorrorGame.Controllers.PlayerController player);
        string GetPromptText();
    }
}
