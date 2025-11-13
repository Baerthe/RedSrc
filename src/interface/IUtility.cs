namespace Interface;

public interface IUtility
{
    bool IsInitialized { get; }
    public void OnInit();
}