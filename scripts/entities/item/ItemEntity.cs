namespace Entities;

using Godot;
using System;
using Entities.Interfaces;
/// <summary>
/// The Entity class for Items, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class ItemEntity : Node2D, IEntity
{
    public int CurrentStackSize { get; set; } = 1;
    public IData Data { get; private set; }
    public override void _Ready()
    {
        AddToGroup("items");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"ItemEntity {Name} already initialized with data!");
            return;
        }
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
    public void NullCheck()
    {
        // Currently no components to check
    }
}