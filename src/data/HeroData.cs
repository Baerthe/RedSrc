namespace Data;

using Godot;
using Interface;
/// <summary>
/// The Data class for Heroes, stores static data
/// </summary>
[GlobalClass]
public partial class HeroData : Resource, IData
{
	[ExportCategory("Stats")]
	[ExportGroup("Info")]
	[Export] public InfoData Info { get; private set; } = new InfoData();
	[Export] public Metadata MetaData { get; private set; } = new Metadata();
	[ExportGroup("Attributes")]
	[Export] public StatsData Stats { get; private set; }
	[ExportGroup("Modifiers")]
	[Export] public HeroClass Class { get; private set; } = HeroClass.Warrior;
	[Export] public HeroAbility Ability { get; private set; } = HeroAbility.None;
	[Export] public uint AbilityStrength { get; private set; }
	[Export] public HeroMovement Movement { get; private set; } = HeroMovement.Walk;
	[ExportGroup("Assets")]
	[Export] public AssetData Assets { get; private set;  }= new AssetData();
}
