namespace Data;

using Interface;
using Godot;
/// <summary>
/// TimedEffect is a Resource that represents an effect with a specific duration applied to an entity.
/// </summary>
[GlobalClass]
public sealed partial class TimedEffect : Effect, IEffect
{
	[ExportGroup("Properties")]
	[Export] public uint Duration { get; private set; }
	public void Apply(IEntity entity)
	{
		
	}
}
