namespace Interface;

using Data;
/// <summary>
/// Interface for data resources
/// </summary>
public interface IData
{
    public InfoData Info { get; }
    public Metadata MetaData { get; }
    public AssetData Assets { get; }
}