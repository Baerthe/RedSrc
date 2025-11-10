namespace Game.Interface;

public interface IGameSystem
{
    bool IsInitialized { get; }
    public void OnInit();
}