namespace Core.Interface
{
    public interface IInteractable
    {
        void OnInteract(Actors.Player.PlayerController player);
        string GetInteractText();
    }
}