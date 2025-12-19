namespace Interface;
/// <summary>
/// An interface for interactable entities.
/// </summary>
public interface IInteractable
{
    public bool IsInteractable { get; }
    public void OnInteract();
}