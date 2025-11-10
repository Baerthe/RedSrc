#if TOOLS
namespace Tools;

using Godot;
using Entities;
using System;
/// <summary>
/// MobDataEditorPlugin is an EditorPlugin that provides a custom panel for editing MobData resources.
/// </summary>
[Tool]
public partial class MobDataEditorPlugin : EditorPlugin
{
    private Control _editorPanel;
    public override void _EnterTree()
    {
        _editorPanel = GD.Load<PackedScene>("res://addons/mob_data_editor/MobDataEditorPanel.tscn").Instantiate<Control>();
        AddControlToBottomPanel(_editorPanel, "Mob Editor");
    }
    public override void _ExitTree()
    {
        RemoveControlFromBottomPanel(_editorPanel);
        _editorPanel?.QueueFree();
    }
}
[Tool]
public partial class MobDataEditorPanel : Control
{
    [Export] private TextureRect _spritePreview;
    [Export] private Label _statsLabel;
    [Export] private Button _createFromTemplateButton;
    private MobData _currentMobData;
    public override void _Ready()
    {
        _createFromTemplateButton.Pressed += OnCreateFromTemplate;
    }
    public void LoadMobData(MobData mobData)
    {
        _currentMobData = mobData;
        if (mobData.Sprite != null)
        {
            //_spritePreview.Texture = mobData.Sprite;
            _spritePreview.Show();
        }
        else
        {
            _spritePreview.Hide();
        }
        // Display stats preview
        _statsLabel.Text = $@"
        Name: {mobData.Info.Name ?? "Unnamed"}
        Tribe: {mobData.Tribe}
        Level: {mobData.Level}
        Health: {mobData.Stats?.MaxHealth ?? 0}
        Damage: {mobData.Stats?.Damage ?? 0}
        Speed: {mobData.Stats?.Speed ?? 0}
        ";
    }
    private void OnCreateFromTemplate()
    {
        var dialog = new FileDialog();
        dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        dialog.Filters = new[] { "*.tres ; Resource Files" };
        dialog.FileSelected += (path) =>
        {
            var template = ResourceLoader.Load<MobData>(path);
            if (template != null)
            {
                var newMobData = template.Duplicate() as MobData;
                var savePath = $"res://data/mobs/new_mob_{DateTime.Now.Ticks}.tres";
                ResourceSaver.Save(newMobData, savePath);
                EditorInterface.Singleton.EditResource(newMobData);
                GD.Print($"Created new mob data at {savePath}");
            }
        };
        AddChild(dialog);
        dialog.PopupCentered();
    }
}
#endif