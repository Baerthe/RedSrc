namespace Interface;

/// <summary>
/// Interface for entity resources
/// </summary>
public interface IEntity
{
    public void Inject(IData data);
    public void NullCheck();
}
