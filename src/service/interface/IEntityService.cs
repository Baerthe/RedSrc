namespace Interface;

public interface IEntityService
{
    public IEntity CreateEntity(IData data);
    public void DestroyEntity(IEntity entity);
}