namespace Data;

using Godot;
using Interfaces;
public sealed partial class XPEntity : Area2D, IEntity
{
    [ExportGroup("XP Entity Nodes")]
    [Export] public Sprite2D XPSprite { get; private set; }
    [Export] public CollisionObject2D XPCollision { get; private set; }
    public XPdata Data { get; private set; }
    public override void _Ready()
    {
        if (Data == null) return;
        AddToGroup("xp");
    }
    public void Inject(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"MobEntity {Name} already initialized with data!");
            return;
        }
        Data = (XPdata)data;
        XPSprite.Texture = Data.XPTexture;
        CollisionShape2D shape = new CollisionShape2D();
        shape.Shape = Data.XPCollisionShape;
        XPCollision.AddChild(shape);
    }
    public void NullCheck(){}
}