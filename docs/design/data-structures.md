# Data Structures Design

> **Version**: 0.1.0  
> **Last Updated**: 2026-01-14

## Overview

This document defines the data structures used throughout RedSrc. All data definitions inherit from `Godot.Resource` enabling editor creation as `.tres` files and runtime sharing between objects.

## Common Data Resources

Shared data structures used across multiple entity types.

### InfoData

Basic information for any game element.

```csharp
[GlobalClass]
public sealed partial class InfoData : Resource
{
    [Export] public string Named { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
}
```

### StatsData

Combat and movement statistics.

```csharp
[GlobalClass]
public sealed partial class StatsData : Resource
{
    [Export] public uint MaxHealth { get; private set; } = 100;
    [Export] public uint DamageBonus { get; private set; } = 0;
    [Export] public float DamageMultiplier { get; private set; } = 1.0f;
    [Export] public float Speed { get; private set; } = 100f;
    [Export] public uint Armor { get; private set; } = 0;
    [Export] public float DamageReduction { get; private set; } = 0f;
    [Export] public float CritChance { get; private set; } = 0.05f;
    [Export] public float CritMultiplier { get; private set; } = 2.0f;
}
```

### AssetData

Visual and audio asset references.

```csharp
[GlobalClass]
public sealed partial class AssetData : Resource
{
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream OnInteractSound { get; set; }
    [Export] public AudioStream OnInitSound { get; set; }
    [Export] public AudioStream OnFreeSound { get; set; }
    [Export] public Shape2D HurtBoxShape { get; set; }
    [Export] public Shape2D HitBoxShape { get; set; }
    [Export] public Color TintColor { get; set; } = Colors.White;
    [Export] public Shader AnimationShader { get; private set; }
}
```

### Metadata

Tracking and progression data.

```csharp
[GlobalClass]
public sealed partial class Metadata : Resource
{
    [Export] public AtlasTexture Icon { get; private set; }
    [Export] public RarityType Rarity { get; private set; } = RarityType.Common;
    [Export] public bool Unlocked { get; private set; } = false;
    [Export] public string UniqueId { get; private set; } = "";
    
    public void Unlock() => Unlocked = true;
}
```

### IDCommon

Unique identification for serialization and lookup.

```csharp
[GlobalClass]
public sealed partial class IDCommon : Resource
{
    [Export] public string Guid { get; private set; }
    [Export] public string Category { get; private set; }
    [Export] public int Index { get; private set; }
    
    public string FullId => $"{Category}:{Guid}";
}
```

## Entity Data Resources

### HeroData (Prospector)

Player character definition.

```csharp
[GlobalClass]
public partial class HeroData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Stats")]
    [Export] public StatsData Stats { get; private set; }
    
    [ExportCategory("Modifiers")]
    [Export] public HeroClass Class { get; private set; } = HeroClass.Warrior;
    [Export] public HeroAbility Ability { get; private set; } = HeroAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public HeroMovement Movement { get; private set; } = HeroMovement.Walk;
    
    [ExportCategory("Loadout")]
    [Export] public int MaxChantSlots { get; private set; } = 4;
    [Export] public int MaxInventorySlots { get; private set; } = 20;
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum HeroClass
{
    Warrior,
    Ranger,
    Weaver,    // Magic user
    Engineer
}

public enum HeroAbility
{
    None,
    Dash,
    Shield,
    Blink,
    Rage
}

public enum HeroMovement
{
    Walk,
    Sprint,
    Hover
}
```

### MobData

Enemy definition.

```csharp
[GlobalClass]
public partial class MobData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Stats")]
    [Export] public StatsData Stats { get; private set; }
    [Export] public uint XPValue { get; private set; } = 10;
    [Export] public int VoiceValue { get; private set; } = 1;
    
    [ExportCategory("Behavior")]
    [Export] public MobType Type { get; private set; } = MobType.Basic;
    [Export] public MobBehavior Behavior { get; private set; } = MobBehavior.Aggressive;
    [Export] public float AggroRange { get; private set; } = 200f;
    [Export] public float AttackRange { get; private set; } = 50f;
    [Export] public float AttackCooldown { get; private set; } = 1.0f;
    
    [ExportCategory("Loot")]
    [Export] public LootTable LootTable { get; private set; }
    [Export] public float LootDropChance { get; private set; } = 0.1f;
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum MobType
{
    Basic,
    Ranged,
    Tank,
    Support,
    Elite,
    Boss
}

public enum MobBehavior
{
    Aggressive,
    Defensive,
    Support,
    Flanking,
    Swarming,
    Boss
}
```

### ItemData

Item definition.

```csharp
[GlobalClass]
public partial class ItemData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Properties")]
    [Export] public ItemType Type { get; private set; } = ItemType.Material;
    [Export] public int MaxStackSize { get; set; } = 64;
    [Export] public int BaseValue { get; set; } = 1;
    [Export] public bool IsConsumable { get; set; } = false;
    
    [ExportCategory("Equipment")]
    [Export] public EquipSlot EquipSlot { get; private set; } = EquipSlot.None;
    [Export] public StatsData EquipStats { get; private set; }
    [Export] public EffectData[] EquipEffects { get; private set; }
    
    [ExportCategory("Consumable")]
    [Export] public EffectData UseEffect { get; private set; }
    [Export] public float UseCooldown { get; private set; } = 0f;
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum ItemType
{
    Material,
    Equipment,
    Consumable,
    Quest,
    Currency
}

public enum EquipSlot
{
    None,
    PrimaryWeapon,
    SecondaryWeapon,
    Head,
    Body,
    Accessory1,
    Accessory2,
    Consumable
}
```

### WeaponData

Weapon-specific data (extends ItemData).

```csharp
[GlobalClass]
public partial class WeaponData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Combat")]
    [Export] public WeaponType Type { get; private set; } = WeaponType.Melee;
    [Export] public uint BaseDamage { get; private set; } = 10;
    [Export] public float AttackSpeed { get; private set; } = 1.0f;
    [Export] public float Range { get; private set; } = 50f;
    [Export] public float Knockback { get; private set; } = 0f;
    
    [ExportCategory("Projectile")]
    [Export] public ProjectileData Projectile { get; private set; }
    [Export] public int ProjectileCount { get; private set; } = 1;
    [Export] public float ProjectileSpread { get; private set; } = 0f;
    
    [ExportCategory("Special")]
    [Export] public EffectData[] OnHitEffects { get; private set; }
    [Export] public ChantData[] BuiltInChants { get; private set; }
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum WeaponType
{
    Melee,
    Ranged,
    Magic,
    Hybrid
}
```

### ProjectileData

Projectile definition.

```csharp
[GlobalClass]
public partial class ProjectileData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    
    [ExportCategory("Movement")]
    [Export] public float Speed { get; private set; } = 300f;
    [Export] public float Lifetime { get; private set; } = 2f;
    [Export] public ProjectilePattern Pattern { get; private set; } = ProjectilePattern.Straight;
    [Export] public float CurveStrength { get; private set; } = 0f;
    
    [ExportCategory("Collision")]
    [Export] public bool PiercesEnemies { get; private set; } = false;
    [Export] public int MaxPierceCount { get; private set; } = 1;
    [Export] public bool DestroyOnWall { get; private set; } = true;
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum ProjectilePattern
{
    Straight,
    Arc,
    Homing,
    Boomerang,
    Wave
}
```

### ChestData

Loot container definition.

```csharp
[GlobalClass]
public partial class ChestData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Loot")]
    [Export] public LootTable LootTable { get; private set; }
    [Export] public int MinDrops { get; private set; } = 1;
    [Export] public int MaxDrops { get; private set; } = 3;
    [Export] public float RarityBonus { get; private set; } = 0f;
    
    [ExportCategory("Interaction")]
    [Export] public float OpenTime { get; private set; } = 0.5f;
    [Export] public bool RequiresKey { get; private set; } = false;
    [Export] public string KeyItemId { get; private set; } = "";
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}
```

### LevelData

Level/map definition.

```csharp
[GlobalClass]
public partial class LevelData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Properties")]
    [Export] public LevelType Type { get; set; }
    [Export] public LevelTier Tier { get; set; }
    [Export] public uint MaxLevel { get; set; } = 1;
    [Export] public float MaxTime { get; set; } = 600f;
    
    [ExportCategory("Spawning")]
    [Export] public MobTable MobTable { get; private set; }
    [Export] public float MobSpawnRate { get; private set; } = 1.0f;
    [Export] public int MaxActiveEnemies { get; private set; } = 100;
    
    [ExportCategory("Loot")]
    [Export] public ChestTable ChestTable { get; private set; }
    [Export] public float ChestSpawnRate { get; private set; } = 1.0f;
    
    [ExportCategory("Extraction")]
    [Export] public int LodestoneCount { get; private set; } = 3;
    [Export] public float ExtractionTime { get; private set; } = 5f;
    
    [ExportCategory("Scene")]
    [Export] public PackedScene Map { get; private set; }
    [Export] public AssetData Assets { get; private set; }
}

public enum LevelType
{
    Forest,
    Ruins,
    Settlement,
    Cave,
    Temple
}

public enum LevelTier
{
    Tier1 = 1,  // Easy
    Tier2 = 2,
    Tier3 = 3,
    Tier4 = 4,
    Tier5 = 5   // Extreme
}
```

## Gameplay Data Resources

### ChantData

Auto-attack ability definition.

```csharp
[GlobalClass]
public partial class ChantData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Unlock")]
    [Export] public int VoiceCost { get; private set; } = 50;
    [Export] public int LevelRequired { get; private set; } = 1;
    
    [ExportCategory("Combat")]
    [Export] public ChantPattern Pattern { get; private set; } = ChantPattern.Projectile;
    [Export] public uint BaseDamage { get; private set; } = 10;
    [Export] public float Cooldown { get; private set; } = 1.0f;
    [Export] public float Range { get; private set; } = 100f;
    [Export] public int HitCount { get; private set; } = 1;
    
    [ExportCategory("Projectile")]
    [Export] public ProjectileData Projectile { get; private set; }
    [Export] public int ProjectileCount { get; private set; } = 1;
    [Export] public float Spread { get; private set; } = 0f;
    
    [ExportCategory("Area")]
    [Export] public float AreaRadius { get; private set; } = 50f;
    [Export] public float Duration { get; private set; } = 0f;
    
    [ExportCategory("Effects")]
    [Export] public EffectData[] OnHitEffects { get; private set; }
    
    [ExportCategory("Upgrades")]
    [Export] public ChantUpgrade[] Upgrades { get; private set; }
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum ChantPattern
{
    Projectile,  // Fire projectile(s) in direction
    Nova,        // 360° burst around player
    Orbit,       // Circling player
    Area,        // Ground effect zone
    Chain,       // Jump between targets
    Beam,        // Continuous stream
    Summon       // Spawn allies
}

[GlobalClass]
public partial class ChantUpgrade : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public int Cost { get; private set; }
    [Export] public StatModifier[] Modifiers { get; private set; }
}
```

### EffectData

Status effect/buff definition.

```csharp
[GlobalClass]
public partial class EffectData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    
    [ExportCategory("Properties")]
    [Export] public EffectType Type { get; private set; } = EffectType.Buff;
    [Export] public float Duration { get; private set; } = 5f;
    [Export] public bool IsStackable { get; private set; } = false;
    [Export] public int MaxStacks { get; private set; } = 1;
    
    [ExportCategory("Modifiers")]
    [Export] public StatModifier[] StatModifiers { get; private set; }
    
    [ExportCategory("Periodic")]
    [Export] public float TickInterval { get; private set; } = 1f;
    [Export] public uint DamagePerTick { get; private set; } = 0;
    [Export] public uint HealPerTick { get; private set; } = 0;
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}

public enum EffectType
{
    Buff,
    Debuff,
    DOT,    // Damage over time
    HOT,    // Heal over time
    CC      // Crowd control
}

[GlobalClass]
public partial class StatModifier : Resource
{
    [Export] public StatType Stat { get; private set; }
    [Export] public ModifierType Type { get; private set; }
    [Export] public float Value { get; private set; }
}

public enum StatType
{
    MaxHealth,
    Damage,
    DamageMultiplier,
    Speed,
    Armor,
    DamageReduction,
    CritChance,
    CritMultiplier,
    AttackSpeed,
    CollectionRadius,
    XPBonus,
    LuckBonus
}

public enum ModifierType
{
    Flat,       // +10
    Percent,    // +10%
    Multiplier  // x1.1
}
```

### CraftData

Crafting recipe definition.

```csharp
[GlobalClass]
public partial class CraftData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Requirements")]
    [Export] public WorkbenchType RequiredWorkbench { get; private set; }
    [Export] public CraftIngredient[] Ingredients { get; private set; }
    [Export] public int SkillRequired { get; private set; } = 0;
    [Export] public string SkillType { get; private set; } = "";
    
    [ExportCategory("Result")]
    [Export] public ItemData Result { get; private set; }
    [Export] public int ResultQuantity { get; private set; } = 1;
    
    [ExportCategory("Time")]
    [Export] public float CraftTime { get; private set; } = 1f;
    [Export] public bool IsInstant { get; private set; } = false;
}

[GlobalClass]
public partial class CraftIngredient : Resource
{
    [Export] public ItemData Item { get; private set; }
    [Export] public int Quantity { get; private set; } = 1;
}
```

### QuestData

Quest definition.

```csharp
[GlobalClass]
public partial class QuestData : Resource, IData
{
    [ExportCategory("Info")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Type")]
    [Export] public QuestType Type { get; private set; } = QuestType.Side;
    [Export] public bool IsRepeatable { get; private set; } = false;
    [Export] public float CooldownTime { get; private set; } = 0f;
    
    [ExportCategory("Requirements")]
    [Export] public string[] Prerequisites { get; private set; }
    [Export] public int MinLevel { get; private set; } = 1;
    
    [ExportCategory("Objectives")]
    [Export] public QuestObjective[] Objectives { get; private set; }
    
    [ExportCategory("Rewards")]
    [Export] public QuestReward[] Rewards { get; private set; }
    
    [ExportCategory("Dialogue")]
    [Export] public DialogueData StartDialogue { get; private set; }
    [Export] public DialogueData CompleteDialogue { get; private set; }
}

public enum QuestType
{
    Main,
    Side,
    Daily,
    Guild
}

[GlobalClass]
public partial class QuestObjective : Resource
{
    [Export] public string Id { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public ObjectiveType Type { get; private set; }
    [Export] public string TargetId { get; private set; }
    [Export] public int RequiredCount { get; private set; } = 1;
}

public enum ObjectiveType
{
    Kill,
    Collect,
    Visit,
    Interact,
    Extract,
    Craft,
    Talk
}

[GlobalClass]
public partial class QuestReward : Resource
{
    [Export] public RewardType Type { get; private set; }
    [Export] public ItemData Item { get; private set; }
    [Export] public int Quantity { get; private set; } = 1;
    [Export] public int VoiceAmount { get; private set; } = 0;
    [Export] public int XPAmount { get; private set; } = 0;
}

public enum RewardType
{
    Item,
    Voices,
    XP,
    Unlock,
    Currency
}
```

## Loot Tables

### LootTable

Defines drop pools for chests and enemies.

```csharp
[GlobalClass]
public partial class LootTable : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public LootEntry[] Entries { get; private set; }
    [Export] public int GuaranteedDrops { get; private set; } = 0;
    [Export] public int MaxDrops { get; private set; } = 5;
}

[GlobalClass]
public partial class LootEntry : Resource
{
    [Export] public ItemData Item { get; private set; }
    [Export] public float Weight { get; private set; } = 1f;
    [Export] public int MinQuantity { get; private set; } = 1;
    [Export] public int MaxQuantity { get; private set; } = 1;
    [Export] public RarityType MinRarity { get; private set; } = RarityType.Common;
}
```

### MobTable

Defines enemy spawn pools for levels.

```csharp
[GlobalClass]
public partial class MobTable : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public MobSpawnEntry[] Entries { get; private set; }
    [Export] public int MinWaveSize { get; private set; } = 5;
    [Export] public int MaxWaveSize { get; private set; } = 20;
    [Export] public float SpawnRateMultiplier { get; private set; } = 1f;
}

[GlobalClass]
public partial class MobSpawnEntry : Resource
{
    [Export] public MobData Mob { get; private set; }
    [Export] public float Weight { get; private set; } = 1f;
    [Export] public int MinDifficulty { get; private set; } = 0;
    [Export] public int MaxPerWave { get; private set; } = 10;
}
```

### ChestTable

Defines chest spawn configuration.

```csharp
[GlobalClass]
public partial class ChestTable : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public ChestSpawnEntry[] Entries { get; private set; }
    [Export] public int MaxActiveChests { get; private set; } = 10;
}

[GlobalClass]
public partial class ChestSpawnEntry : Resource
{
    [Export] public ChestData Chest { get; private set; }
    [Export] public float Weight { get; private set; } = 1f;
}
```

## Index Resources

Indices provide lookup tables for game content.

### EntityIndex

```csharp
[GlobalClass]
public partial class EntityIndex : Resource
{
    [Export] public PackedScene HeroTemplate { get; private set; }
    [Export] public PackedScene MobTemplate { get; private set; }
    [Export] public PackedScene ChestTemplate { get; private set; }
    [Export] public PackedScene LevelTemplate { get; private set; }
    [Export] public PackedScene ItemTemplate { get; private set; }
    [Export] public PackedScene ProjectileTemplate { get; private set; }
    [Export] public PackedScene XPTemplate { get; private set; }
}
```

### HeroIndex

```csharp
[GlobalClass]
public partial class HeroIndex : Resource
{
    [Export] public HeroData[] Heroes { get; private set; }
    [Export] public HeroData DebugHero { get; private set; }
    
    public HeroData GetHero(string id);
    public HeroData[] GetUnlockedHeroes();
}
```

### ItemIndex

```csharp
[GlobalClass]
public partial class ItemIndex : Resource
{
    [Export] public ItemData[] Items { get; private set; }
    
    public ItemData GetItem(string id);
    public ItemData[] GetItemsByType(ItemType type);
    public ItemData[] GetItemsByRarity(RarityType rarity);
}
```

### LevelIndex

```csharp
[GlobalClass]
public partial class LevelIndex : Resource
{
    [Export] public LevelData[] Levels { get; private set; }
    [Export] public LevelData DebugLevel { get; private set; }
    
    public LevelData GetLevel(string id);
    public LevelData[] GetLevelsByTier(LevelTier tier);
    public LevelData[] GetUnlockedLevels();
}
```

### WeaponIndex

```csharp
[GlobalClass]
public partial class WeaponIndex : Resource
{
    [Export] public WeaponData[] Weapons { get; private set; }
    
    public WeaponData GetWeapon(string id);
    public WeaponData[] GetWeaponsByType(WeaponType type);
}
```

### ChantIndex

```csharp
[GlobalClass]
public partial class ChantIndex : Resource
{
    [Export] public ChantData[] Chants { get; private set; }
    
    public ChantData GetChant(string id);
    public ChantData[] GetChantsByPattern(ChantPattern pattern);
    public ChantData[] GetUnlockedChants();
}
```

## Save Data

### SaveData

Main save file structure.

```csharp
[GlobalClass]
public partial class SaveData : Resource
{
    // Meta
    [Export] public string SaveName { get; set; }
    [Export] public string SaveVersion { get; set; }
    [Export] public long SaveTimestamp { get; set; }
    [Export] public float TotalPlayTime { get; set; }
    
    // Progression
    [Export] public int TotalExtractions { get; set; }
    [Export] public int SuccessfulExtractions { get; set; }
    [Export] public int TotalVoicesDonated { get; set; }
    [Export] public string[] UnlockedItems { get; set; }
    [Export] public string[] CompletedQuests { get; set; }
    [Export] public string[] UnlockedChants { get; set; }
    
    // Current State
    [Export] public ProspectorSave CurrentProspector { get; set; }
    [Export] public InventorySave Inventory { get; set; }
    [Export] public FacilitySave[] Facilities { get; set; }
    
    // Settings (or separate file)
    [Export] public SettingsSave Settings { get; set; }
}

[GlobalClass]
public partial class ProspectorSave : Resource
{
    [Export] public string HeroId { get; set; }
    [Export] public int Level { get; set; }
    [Export] public int CurrentXP { get; set; }
    [Export] public int BankedVoices { get; set; }
    [Export] public string[] EquippedItems { get; set; }
    [Export] public string[] ActiveChants { get; set; }
    [Export] public SkillSave[] Skills { get; set; }
}

[GlobalClass]
public partial class InventorySave : Resource
{
    [Export] public InventorySlot[] Slots { get; set; }
    [Export] public int Currency { get; set; }
}

[GlobalClass]
public partial class InventorySlot : Resource
{
    [Export] public string ItemId { get; set; }
    [Export] public int Quantity { get; set; }
}
```

## Enumerations Reference

### RarityType

```csharp
public enum RarityType
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4,
    Mythic = 5
}
```

### WorkbenchType

```csharp
public enum WorkbenchType
{
    Basic,
    Forge,
    Alchemy,
    Enchanting,
    Engineering
}
```

---

*See [UI & Presentation](ui-and-presentation.md) for visual design specifications.*
