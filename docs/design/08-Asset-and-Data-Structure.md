# Asset and Data Structure Document

## Overview

This document defines the asset pipeline, data organization, and resource management for the RTS game, building on the existing Kenney.nl 1-bit asset pack.

## Visual Asset Structure

### Existing Assets (Kenney 1-Bit Pack)

#### Available Resources
- **1-Bit Pack (1.2)**: Sprites and tilesets
- **1-Bit Input Prompts**: UI elements
- **Location**: `/asset/tilesheet/`

### Required RTS Assets

#### Unit Sprites (16x16 pixels)

**Human Faction:**
```
/asset/tilesheet/units/human/
├── peasant_idle.png (4 frames)
├── peasant_walk.png (4 frames)
├── peasant_gather.png (4 frames)
├── footman_idle.png (4 frames)
├── footman_walk.png (4 frames)
├── footman_attack.png (4 frames)
├── archer_idle.png (4 frames)
├── archer_walk.png (4 frames)
├── archer_attack.png (4 frames)
├── knight_idle.png (4 frames)
├── knight_walk.png (4 frames)
├── knight_attack.png (4 frames)
└── [additional units...]
```

**Orc Faction:**
```
/asset/tilesheet/units/orc/
├── peon_idle.png
├── peon_walk.png
├── grunt_idle.png
├── grunt_walk.png
├── raider_idle.png
├── raider_walk.png
└── [additional units...]
```

**Animation Specifications:**
- **Frame Count**: 4 frames for walk cycles, 4-6 for attacks
- **Frame Rate**: 10 FPS (100ms per frame)
- **Directions**: 8 directions (N, NE, E, SE, S, SW, W, NW)
- **Alternative**: 4 directions (N, E, S, W) for MVP

#### Building Sprites (32x32, 48x48, 64x64)

**Human Buildings:**
```
/asset/tilesheet/buildings/human/
├── town_hall.png (64x64)
├── farm.png (32x32)
├── barracks.png (48x48)
├── blacksmith.png (48x48)
├── tower.png (32x32)
└── [additional buildings...]
```

**Building States:**
- Base sprite (completed)
- Construction state (75% transparent, scaffold overlay)
- Damaged state (smoke particle effect trigger)
- Destroyed state (rubble sprite)

#### Terrain Tiles (16x16)

```
/asset/tilesheet/terrain/
├── grass.png
├── dirt.png
├── stone.png
├── water.png
├── tree.png (multiple variations)
├── gold_mine.png
├── cliff_edges.png (autotile set)
└── roads.png (autotile set)
```

**Autotile Configuration:**
- Use Godot's TileMap autotiling
- 3x3 bitmask for edges
- Variations for visual diversity

#### UI Assets (8x8, 16x16, 32x32)

```
/asset/ui/
├── icons/
│   ├── gold_icon.png (16x16)
│   ├── wood_icon.png (16x16)
│   ├── supply_icon.png (16x16)
│   ├── unit_icons/ (32x32 per unit)
│   └── building_icons/ (32x32 per building)
├── buttons/
│   ├── button_normal.png
│   ├── button_hover.png
│   ├── button_pressed.png
│   └── button_disabled.png
├── panels/
│   ├── panel_bg.png (9-slice)
│   ├── panel_border.png
│   └── tooltip_bg.png
├── cursors/
│   ├── cursor_default.png
│   ├── cursor_select.png
│   ├── cursor_attack.png
│   └── cursor_build.png
└── selection/
    ├── selection_circle.png
    ├── selection_box.png
    └── health_bar.png
```

#### Effects and Particles (16x16, 32x32)

```
/asset/effects/
├── combat/
│   ├── sword_slash.png (4 frames)
│   ├── arrow_projectile.png
│   ├── impact_small.png (4 frames)
│   ├── impact_large.png (4 frames)
│   └── blood_splatter.png (4 frames)
├── construction/
│   ├── hammer_strike.png (4 frames)
│   └── construction_dust.png (particle)
├── gathering/
│   ├── gold_sparkle.png (particle)
│   └── wood_chips.png (particle)
└── building/
    ├── smoke.png (particle)
    ├── fire.png (particle - 4 frames)
    └── explosion.png (8 frames)
```

### Asset Creation Guidelines

#### 1-Bit Style Requirements
- **Color Limit**: 2 colors (black and one accent)
- **Pixel Perfect**: Align to 16x16 grid
- **No Anti-Aliasing**: Sharp edges only
- **Dithering**: Use for shading effects
- **Consistency**: Match Kenney pack style

#### Sprite Creation Workflow
1. Use existing Kenney sprites as base when possible
2. Modify or combine sprites for new units
3. Create new sprites following 1-bit style guide
4. Export as PNG with transparency
5. Import to Godot and configure

#### Alternative: Godot Built-In Generation
```gdscript
# Example: Generate simple unit sprite
func generate_unit_sprite(color: Color) -> Texture2D:
    var image = Image.create(16, 16, false, Image.FORMAT_RGBA8)
    # Draw simple shape using pixels
    for x in range(6, 10):
        for y in range(6, 10):
            image.set_pixel(x, y, color)
    return ImageTexture.create_from_image(image)
```

## Audio Assets

### Existing Audio (RPG Audio Pack)

Available at `/asset/audio/`:
- Ambience sounds
- Impact sounds
- UI sounds

### Required RTS Audio

#### Sound Effects

```
/asset/audio/sfx/
├── units/
│   ├── select/
│   │   ├── unit_select_1.wav
│   │   ├── unit_select_2.wav
│   │   └── unit_select_3.wav
│   ├── move/
│   │   ├── move_confirm_1.wav
│   │   └── move_confirm_2.wav
│   ├── attack/
│   │   ├── sword_hit.wav
│   │   ├── arrow_shoot.wav
│   │   ├── arrow_hit.wav
│   │   └── impact.wav
│   └── death/
│       ├── death_1.wav
│       └── death_2.wav
├── buildings/
│   ├── construction_start.wav
│   ├── construction_loop.wav
│   ├── construction_complete.wav
│   ├── building_damage.wav
│   └── building_destroy.wav
├── resources/
│   ├── gold_mine.wav (looping)
│   ├── wood_chop.wav
│   └── resource_deposit.wav
└── ui/
    ├── button_click.wav
    ├── button_hover.wav
    ├── error.wav
    ├── notification.wav
    └── alert.wav
```

#### Music Tracks

```
/asset/audio/music/
├── menu_theme.ogg
├── game_theme_1.ogg (peaceful)
├── game_theme_2.ogg (tension)
├── game_theme_3.ogg (battle)
├── victory.ogg
└── defeat.ogg
```

**Music Specifications:**
- Format: OGG Vorbis (compressed)
- Loop points: Seamless loops
- Volume: Normalized to -12dB
- Length: 1-3 minutes per track

### Audio Configuration

**Audio Bus Layout:**
```
Master
├── Music (-6dB)
├── SFX
│   ├── UI (0dB)
│   ├── Units (0dB)
│   └── Combat (0dB)
└── Ambience (-12dB)
```

## Data Structure

### Resource Files Organization

```
/data/rts/
├── factions/
│   ├── FactionIndex.tres
│   ├── HumanFaction.tres
│   └── OrcFaction.tres
├── units/
│   ├── human/
│   │   ├── UnitIndex.tres
│   │   ├── Peasant.tres
│   │   ├── Footman.tres
│   │   ├── Archer.tres
│   │   ├── Knight.tres
│   │   └── [...]
│   └── orc/
│       ├── UnitIndex.tres
│       ├── Peon.tres
│       ├── Grunt.tres
│       └── [...]
├── buildings/
│   ├── human/
│   │   ├── BuildingIndex.tres
│   │   ├── TownHall.tres
│   │   ├── Farm.tres
│   │   ├── Barracks.tres
│   │   └── [...]
│   └── orc/
│       ├── BuildingIndex.tres
│       ├── GreatHall.tres
│       ├── PigFarm.tres
│       └── [...]
├── abilities/
│   ├── AbilityIndex.tres
│   ├── Heal.tres
│   ├── Charge.tres
│   └── [...]
├── upgrades/
│   ├── UpgradeIndex.tres
│   ├── WeaponUpgrade1.tres
│   ├── ArmorUpgrade1.tres
│   └── [...]
└── config/
    ├── GameSettings.tres
    ├── BalanceConfig.tres
    └── DifficultySettings.tres
```

### Data Class Specifications

#### Example: UnitData Resource

```gdscript
# data/rts/units/human/Footman.tres
[gd_resource type="Resource" script_class="UnitData" load_steps=2 format=3]

[ext_resource type="Script" path="res://src/rts/data/UnitData.cs" id="1"]

[resource]
script = ExtResource("1")
UnitName = "Footman"
MaxHealth = 60
Armor = 2
ArmorType = 2  # Medium
AttackDamage = 12
DamageType = 0  # Normal
AttackSpeed = 1.5
AttackRange = 1.0
MovementSpeed = 2.8
VisionRange = 7
SupplyCost = 1
BuildTime = 20.0
Cost = {
    "Gold": 100,
    "Wood": 0
}
UnitSprite = preload("res://asset/tilesheet/units/human/footman.png")
SelectionRadius = 8.0
```

### Scene Template Organization

```
/scene/rts/
├── RTSMain.tscn               # Main game scene
├── RTSMatch.tscn              # Match container
├── maps/
│   ├── TestMap01.tscn
│   ├── TestMap02.tscn
│   └── TestMap03.tscn
├── templates/
│   ├── units/
│   │   ├── RTSUnit.tscn       # Base unit template
│   │   ├── RTSWorker.tscn     # Worker variant
│   │   └── RTSRanged.tscn     # Ranged variant
│   ├── buildings/
│   │   ├── RTSBuilding.tscn   # Base building
│   │   ├── ProductionBuilding.tscn
│   │   └── DefensiveBuilding.tscn
│   ├── effects/
│   │   ├── Projectile.tscn
│   │   ├── ImpactEffect.tscn
│   │   └── AuraEffect.tscn
│   └── resources/
│       ├── GoldMine.tscn
│       └── TreeCluster.tscn
└── ui/
    ├── HUD.tscn
    ├── Minimap.tscn
    ├── SelectionPanel.tscn
    ├── CommandPanel.tscn
    ├── BuildMenu.tscn
    ├── MainMenu.tscn
    ├── PauseMenu.tscn
    └── GameOverScreen.tscn
```

## Asset Import Settings

### Texture Import Configuration

**Pixel Art Settings (Default for all sprites):**
```
Filter: Nearest
Mipmaps: Off
Compression: Lossless
Repeat: Disabled
```

**UI Textures:**
```
Filter: Nearest
Mipmaps: Off
Compression: Lossless
sRGB: On
```

### Audio Import Settings

**Sound Effects:**
```
Format: WAV (16-bit, 44.1kHz)
Compression: Compressed (ADPCM)
Loop: Disabled
```

**Music:**
```
Format: OGG Vorbis
Compression: Quality 7
Loop: Enabled
Stream: Enabled
```

## Spritesheet Management

### AnimatedSprite2D Configuration

```csharp
// Example: Configure unit animations
public void SetupAnimations()
{
    var animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    
    // Configure idle animation
    var idleFrames = new SpriteFrames();
    idleFrames.AddAnimation("idle");
    idleFrames.SetAnimationSpeed("idle", 10); // 10 FPS
    // Add frames...
    
    // Configure walk animation
    idleFrames.AddAnimation("walk");
    idleFrames.SetAnimationSpeed("walk", 10);
    // Add frames...
    
    animSprite.SpriteFrames = idleFrames;
    animSprite.Play("idle");
}
```

### Texture Atlas Generation

For performance, combine related sprites into atlases:

```
/asset/tilesheet/atlas/
├── units_human_atlas.png (256x256)
├── units_orc_atlas.png (256x256)
├── buildings_atlas.png (512x512)
└── ui_atlas.png (256x256)
```

## Performance Optimization

### Asset Loading Strategy

**Preload Critical Assets:**
```csharp
public class AssetLoader : Node
{
    private Dictionary<string, PackedScene> _unitTemplates = new();
    private Dictionary<string, Texture2D> _uiTextures = new();
    
    public override void _Ready()
    {
        PreloadUnitTemplates();
        PreloadUITextures();
    }
    
    private void PreloadUnitTemplates()
    {
        _unitTemplates["Footman"] = GD.Load<PackedScene>("res://scene/rts/templates/units/Footman.tscn");
        _unitTemplates["Archer"] = GD.Load<PackedScene>("res://scene/rts/templates/units/Archer.tscn");
        // ...
    }
}
```

**Lazy Load Non-Critical:**
- Effect sprites
- Music tracks (stream from disk)
- Campaign-specific assets

### Memory Budget

**Target Memory Usage:**
- Units/Buildings: 50MB
- Terrain: 20MB
- UI: 10MB
- Audio: 30MB (streaming)
- Total: ~110MB base + dynamic

## Asset Pipeline Workflow

### 1. Design Phase
- Create asset specifications
- Define sprite requirements
- Plan animation frames

### 2. Creation Phase
- Create sprites in pixel art editor (Aseprite, Piskel)
- Follow 1-bit style guide
- Export individual frames

### 3. Import Phase
- Import to Godot
- Configure import settings
- Create AnimatedSprite2D resources

### 4. Integration Phase
- Link sprites to data files
- Configure animations in scenes
- Test in-game

### 5. Optimization Phase
- Generate texture atlases
- Compress audio
- Profile memory usage

## Placeholder Assets

For rapid prototyping, use colored rectangles:

```csharp
public static class PlaceholderAssets
{
    public static Texture2D CreateUnitPlaceholder(Color color)
    {
        var image = Image.Create(16, 16, false, Image.FORMAT_RGBA8);
        image.Fill(color);
        return ImageTexture.CreateFromImage(image);
    }
    
    public static Texture2D CreateBuildingPlaceholder(Color color, int size)
    {
        var image = Image.Create(size, size, false, Image.FORMAT_RGBA8);
        image.Fill(color);
        return ImageTexture.CreateFromImage(image);
    }
}
```

## Version Control

### .gitignore Additions

```
# Asset work files
*.aseprite
*.psd
*.kra

# Temporary exports
asset/temp/
asset/**/temp/

# Large audio source files
asset/audio/source/

# Godot import cache (already in existing .gitignore)
.godot/
```

### Asset Source Files

Store working files separately:
```
/asset_source/
├── sprites/
│   ├── units/ (Aseprite files)
│   └── buildings/ (Aseprite files)
└── audio/
    └── source/ (uncompressed WAV)
```

## Modding Support (Future)

### Asset Override System

```csharp
public class ModLoader
{
    public void LoadMod(string modPath)
    {
        // Override data files
        string unitsPath = $"{modPath}/data/units/";
        if (DirAccess.DirExistsAbsolute(unitsPath))
        {
            LoadCustomUnits(unitsPath);
        }
        
        // Override sprites
        string spritesPath = $"{modPath}/asset/sprites/";
        if (DirAccess.DirExistsAbsolute(spritesPath))
        {
            LoadCustomSprites(spritesPath);
        }
    }
}
```

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
