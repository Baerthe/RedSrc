# Data Structures Document
## RedGate: Tactical Dungeon RPG

> This document defines all data resources (`.tres` files) used throughout the game.

---

## Data Architecture Overview

Data resources in RedGate follow these principles:

1. **Godot Resources**: All data classes inherit from `Godot.Resource`
2. **Stateless**: Data defines templates; runtime state is in components
3. **Shareable**: Multiple entities can reference the same data
4. **Editor-Friendly**: Exported properties editable in Godot Inspector
5. **Indexed**: Data registered in Index resources for runtime lookup

```
data/
├── common/           # Shared data structures
├── heroes/           # Hero definitions
├── enemies/          # Enemy definitions
├── items/            # Item definitions
├── abilities/        # Ability definitions
├── effects/          # Status effect definitions
├── dungeons/         # Dungeon configuration
├── quests/           # Quest definitions
├── dialogues/        # Dialogue trees
└── indices/          # Runtime lookup indices
```

---

## Common Data Structures

### InfoData

Basic information shared by all named entities.

```csharp
namespace Data;

/// <summary>
/// Common information for named entities.
/// </summary>
[GlobalClass]
public sealed partial class InfoData : Resource
{
    [ExportCategory("Identity")]
    [Export] public string Named { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    
    [ExportCategory("Lore")]
    [Export(PropertyHint.MultilineText)] 
    public string Lore { get; private set; } = "";
    
    [ExportCategory("Localization")]
    [Export] public string LocalizationKey { get; private set; } = "";
}
```

### StatsData

Base statistics for entities.

```csharp
namespace Data;

/// <summary>
/// Base statistics for heroes and enemies.
/// </summary>
[GlobalClass]
public sealed partial class StatsData : Resource
{
    [ExportCategory("Primary Stats")]
    [Export] public int Strength { get; private set; } = 10;
    [Export] public int Agility { get; private set; } = 10;
    [Export] public int Intelligence { get; private set; } = 10;
    [Export] public int Wisdom { get; private set; } = 10;
    [Export] public int Vitality { get; private set; } = 10;
    
    [ExportCategory("Derived Stats")]
    [Export] public int MaxHealth { get; private set; } = 100;
    [Export] public int MaxMana { get; private set; } = 50;
    [Export] public float Speed { get; private set; } = 100f;
    [Export] public float HealthRegen { get; private set; } = 0f;
    [Export] public float ManaRegen { get; private set; } = 1f;
    
    [ExportCategory("Combat Stats")]
    [Export] public int BaseDamage { get; private set; } = 10;
    [Export] public int Armor { get; private set; } = 0;
    [Export] public int MagicResist { get; private set; } = 0;
    [Export] public float AttackSpeed { get; private set; } = 1f;
    [Export] public float CritChance { get; private set; } = 0.05f;
    [Export] public float CritMultiplier { get; private set; } = 1.5f;
    
    /// <summary>
    /// Calculate derived health from vitality.
    /// </summary>
    public int CalculatedHealth => MaxHealth + (Vitality * 10);
    
    /// <summary>
    /// Calculate derived mana from intelligence.
    /// </summary>
    public int CalculatedMana => MaxMana + (Intelligence * 5);
}
```

### AssetData

Visual and audio assets for entities.

```csharp
namespace Data;

/// <summary>
/// Visual and audio assets for entities.
/// </summary>
[GlobalClass]
public sealed partial class AssetData : Resource
{
    [ExportCategory("Sprites")]
    [Export] public SpriteFrames AnimatedSprite { get; private set; }
    [Export] public Texture2D Portrait { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    
    [ExportCategory("Audio")]
    [Export] public AudioStream HitSound { get; set; }
    [Export] public AudioStream DeathSound { get; set; }
    [Export] public AudioStream AttackSound { get; set; }
    [Export] public AudioStream FootstepSound { get; set; }
    
    [ExportCategory("Collision")]
    [Export] public Shape2D CollisionShape { get; set; }
    [Export] public Shape2D HurtboxShape { get; set; }
    [Export] public Shape2D HitboxShape { get; set; }
    
    [ExportCategory("Visual")]
    [Export] public Color TintColor { get; set; } = Colors.White;
    [Export] public Vector2 SpriteOffset { get; set; } = Vector2.Zero;
    [Export] public float Scale { get; set; } = 1f;
}
```

### Metadata

Internal metadata for data management.

```csharp
namespace Data;

/// <summary>
/// Internal metadata for data resources.
/// </summary>
[GlobalClass]
public sealed partial class Metadata : Resource
{
    [Export] public string UniqueId { get; private set; } = "";
    [Export] public int Version { get; private set; } = 1;
    [Export] public string[] Tags { get; private set; } = Array.Empty<string>();
    [Export] public RarityTier Rarity { get; private set; } = RarityTier.Common;
    [Export] public bool IsEnabled { get; private set; } = true;
}

public enum RarityTier : byte
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4
}
```

---

## Hero Data

### HeroData

Complete hero definition.

```csharp
namespace Data;

/// <summary>
/// Complete hero character definition.
/// </summary>
[GlobalClass]
public partial class HeroData : Resource, IData
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Class")]
    [Export] public HeroClass Class { get; private set; } = HeroClass.Warrior;
    [Export] public string ClassName { get; private set; } = "Warrior";
    [Export(PropertyHint.MultilineText)] 
    public string ClassDescription { get; private set; } = "";
    
    [ExportCategory("Statistics")]
    [Export] public StatsData BaseStats { get; private set; }
    [Export] public StatsData LevelUpStats { get; private set; }
    
    [ExportCategory("Abilities")]
    [Export] public AbilityData[] BaseAbilities { get; private set; } = new AbilityData[4];
    [Export] public AbilityData UltimateAbility { get; private set; }
    [Export] public AbilityData[] UnlockableAbilities { get; private set; }
    
    [ExportCategory("Equipment")]
    [Export] public ItemData StartingWeapon { get; private set; }
    [Export] public ItemData[] StartingItems { get; private set; }
    [Export] public WeaponType[] AllowedWeapons { get; private set; }
    [Export] public ArmorType[] AllowedArmor { get; private set; }
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
    
    [ExportCategory("Progression")]
    [Export] public int BaseExperienceRequired { get; private set; } = 100;
    [Export] public float ExperienceMultiplier { get; private set; } = 1.5f;
}
```

### Hero Enums

```csharp
namespace Data;

public enum HeroClass : byte
{
    Warrior = 0,
    Mage = 1,
    Rogue = 2,
    Cleric = 3
}

public enum WeaponType : byte
{
    Sword = 0,
    Axe = 1,
    Mace = 2,
    Staff = 3,
    Wand = 4,
    Dagger = 5,
    Bow = 6,
    Shield = 7
}

public enum ArmorType : byte
{
    Cloth = 0,
    Leather = 1,
    Mail = 2,
    Plate = 3
}
```

---

## Enemy Data

### EnemyData

Complete enemy definition.

```csharp
namespace Data;

/// <summary>
/// Complete enemy entity definition.
/// </summary>
[GlobalClass]
public partial class EnemyData : Resource, IData
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Classification")]
    [Export] public EnemyType Type { get; private set; } = EnemyType.Normal;
    [Export] public EnemyTribe Tribe { get; private set; } = EnemyTribe.None;
    [Export] public int Level { get; private set; } = 1;
    
    [ExportCategory("Statistics")]
    [Export] public StatsData Stats { get; private set; }
    
    [ExportCategory("AI")]
    [Export] public AIBehavior Behavior { get; private set; } = AIBehavior.Aggressive;
    [Export] public float DetectionRange { get; private set; } = 200f;
    [Export] public float AttackRange { get; private set; } = 50f;
    [Export] public float FleeHealthPercent { get; private set; } = 0f;
    
    [ExportCategory("Combat")]
    [Export] public DamageType DamageType { get; private set; } = DamageType.Physical;
    [Export] public AbilityData[] Abilities { get; private set; }
    [Export] public float AbilityChance { get; private set; } = 0.2f;
    
    [ExportCategory("Rewards")]
    [Export] public int ExperienceReward { get; private set; } = 10;
    [Export] public int GoldReward { get; private set; } = 5;
    [Export] public LootTable LootTable { get; private set; }
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}
```

### Enemy Enums

```csharp
namespace Data;

public enum EnemyType : byte
{
    Normal = 0,
    Elite = 1,
    MiniBoss = 2,
    Boss = 3
}

public enum EnemyTribe : byte
{
    None = 0,
    Undead = 1,
    Beast = 2,
    Humanoid = 3,
    Demon = 4,
    Elemental = 5,
    Construct = 6,
    Dragon = 7
}
```

---

## Item Data

### ItemData

Base item definition.

```csharp
namespace Data;

/// <summary>
/// Base item definition.
/// </summary>
[GlobalClass]
public partial class ItemData : Resource, IData
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Type")]
    [Export] public ItemType Type { get; private set; } = ItemType.Miscellaneous;
    [Export] public ItemRarity Rarity { get; private set; } = ItemRarity.Common;
    
    [ExportCategory("Equipment")]
    [Export] public bool IsEquippable { get; private set; } = false;
    [Export] public EquipSlot EquipSlot { get; private set; } = EquipSlot.None;
    [Export] public HeroClass[] ClassRestrictions { get; private set; }
    [Export] public int LevelRequirement { get; private set; } = 1;
    
    [ExportCategory("Stats")]
    [Export] public StatModifier[] BaseModifiers { get; private set; }
    [Export] public int PossibleModifierSlots { get; private set; } = 0;
    
    [ExportCategory("Stacking")]
    [Export] public bool IsStackable { get; private set; } = false;
    [Export] public int MaxStackSize { get; private set; } = 1;
    
    [ExportCategory("Value")]
    [Export] public int BuyPrice { get; private set; } = 10;
    [Export] public int SellPrice { get; private set; } = 5;
    
    [ExportCategory("Consumable")]
    [Export] public bool IsConsumable { get; private set; } = false;
    [Export] public EffectData[] ConsumeEffects { get; private set; }
    
    [ExportCategory("Assets")]
    [Export] public AssetData Assets { get; private set; }
}
```

### WeaponData

Extended weapon definition.

```csharp
namespace Data;

/// <summary>
/// Weapon-specific item data.
/// </summary>
[GlobalClass]
public partial class WeaponData : ItemData
{
    [ExportCategory("Weapon")]
    [Export] public WeaponType WeaponType { get; private set; }
    [Export] public int MinDamage { get; private set; } = 5;
    [Export] public int MaxDamage { get; private set; } = 10;
    [Export] public float AttackSpeed { get; private set; } = 1f;
    [Export] public float Range { get; private set; } = 50f;
    [Export] public DamageType DamageType { get; private set; } = DamageType.Physical;
    
    [ExportCategory("Animation")]
    [Export] public SpriteFrames AttackAnimation { get; private set; }
    
    /// <summary>
    /// Calculate average damage.
    /// </summary>
    public float AverageDamage => (MinDamage + MaxDamage) / 2f;
    
    /// <summary>
    /// Calculate DPS.
    /// </summary>
    public float DPS => AverageDamage * AttackSpeed;
}
```

### ArmorData

Extended armor definition.

```csharp
namespace Data;

/// <summary>
/// Armor-specific item data.
/// </summary>
[GlobalClass]
public partial class ArmorData : ItemData
{
    [ExportCategory("Armor")]
    [Export] public ArmorType ArmorType { get; private set; }
    [Export] public int ArmorValue { get; private set; } = 0;
    [Export] public int MagicResist { get; private set; } = 0;
    
    [ExportCategory("Set Bonus")]
    [Export] public string SetName { get; private set; } = "";
    [Export] public SetBonusData SetBonus { get; private set; }
}
```

### StatModifier

Stat modification definition.

```csharp
namespace Data;

/// <summary>
/// Defines a stat modification.
/// </summary>
[GlobalClass]
public partial class StatModifier : Resource
{
    [Export] public StatType Stat { get; private set; }
    [Export] public ModifierType Type { get; private set; } = ModifierType.Flat;
    [Export] public float Value { get; private set; } = 0f;
    
    /// <summary>
    /// Apply this modifier to a base value.
    /// </summary>
    public float Apply(float baseValue)
    {
        return Type switch
        {
            ModifierType.Flat => baseValue + Value,
            ModifierType.Percent => baseValue * (1f + Value / 100f),
            ModifierType.Multiply => baseValue * Value,
            _ => baseValue
        };
    }
}

public enum StatType : byte
{
    MaxHealth = 0,
    MaxMana = 1,
    Strength = 2,
    Agility = 3,
    Intelligence = 4,
    Wisdom = 5,
    Vitality = 6,
    Armor = 7,
    MagicResist = 8,
    AttackSpeed = 9,
    CritChance = 10,
    CritMultiplier = 11,
    MoveSpeed = 12,
    HealthRegen = 13,
    ManaRegen = 14,
    DamageBonus = 15,
    CooldownReduction = 16
}

public enum ModifierType : byte
{
    Flat = 0,       // +10
    Percent = 1,    // +10%
    Multiply = 2    // ×1.5
}
```

### Item Enums

```csharp
namespace Data;

public enum ItemType : byte
{
    Weapon = 0,
    Armor = 1,
    Accessory = 2,
    Consumable = 3,
    Material = 4,
    Quest = 5,
    Currency = 6,
    Miscellaneous = 7
}

public enum ItemRarity : byte
{
    Common = 0,     // White
    Uncommon = 1,   // Green
    Rare = 2,       // Blue
    Epic = 3,       // Purple
    Legendary = 4   // Orange
}
```

### ItemInstance

Runtime item instance with rolled modifiers.

```csharp
namespace Data;

/// <summary>
/// Runtime instance of an item with rolled modifiers.
/// </summary>
public class ItemInstance
{
    public ItemData Data { get; set; }
    public int Quantity { get; set; } = 1;
    public StatModifier[] RolledModifiers { get; set; }
    public int ItemLevel { get; set; } = 1;
    
    /// <summary>
    /// Get total value of a stat from all modifiers.
    /// </summary>
    public float GetStatBonus(StatType stat)
    {
        float total = 0f;
        
        // Base modifiers
        if (Data.BaseModifiers != null)
        {
            foreach (var mod in Data.BaseModifiers)
            {
                if (mod.Stat == stat)
                    total = mod.Apply(total);
            }
        }
        
        // Rolled modifiers
        if (RolledModifiers != null)
        {
            foreach (var mod in RolledModifiers)
            {
                if (mod.Stat == stat)
                    total = mod.Apply(total);
            }
        }
        
        return total;
    }
}
```

---

## Ability Data

### AbilityData

Complete ability definition.

```csharp
namespace Data;

/// <summary>
/// Complete ability definition.
/// </summary>
[GlobalClass]
public partial class AbilityData : Resource
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    
    [ExportCategory("Type")]
    [Export] public AbilityType Type { get; private set; } = AbilityType.Instant;
    [Export] public TargetType TargetType { get; private set; } = TargetType.Enemy;
    [Export] public DamageType DamageType { get; private set; } = DamageType.Magical;
    
    [ExportCategory("Costs")]
    [Export] public int ManaCost { get; private set; } = 10;
    [Export] public float Cooldown { get; private set; } = 5f;
    
    [ExportCategory("Damage")]
    [Export] public int BaseDamage { get; private set; } = 0;
    [Export] public float DamageScaling { get; private set; } = 1f;
    [Export] public StatType ScalingStat { get; private set; } = StatType.Intelligence;
    
    [ExportCategory("Range & Area")]
    [Export] public float Range { get; private set; } = 100f;
    [Export] public float AreaOfEffect { get; private set; } = 0f;
    
    [ExportCategory("Channel")]
    [Export] public bool IsChanneled { get; private set; } = false;
    [Export] public float ChannelTime { get; private set; } = 0f;
    
    [ExportCategory("Effects")]
    [Export] public EffectData[] Effects { get; private set; }
    
    [ExportCategory("Projectile")]
    [Export] public ProjectileData Projectile { get; private set; }
    
    [ExportCategory("Animation")]
    [Export] public string AnimationName { get; private set; } = "attack";
    [Export] public AudioStream CastSound { get; private set; }
    
    [ExportCategory("Unlock")]
    [Export] public int UnlockLevel { get; private set; } = 1;
    [Export] public string[] PrerequisiteAbilities { get; private set; }
}
```

### Ability Enums

```csharp
namespace Data;

public enum AbilityType : byte
{
    Instant = 0,
    Projectile = 1,
    AoE = 2,
    Buff = 3,
    Debuff = 4,
    Summon = 5,
    Channel = 6,
    Toggle = 7,
    Passive = 8
}

public enum TargetType : byte
{
    Self = 0,
    Enemy = 1,
    Ally = 2,
    Ground = 3,
    Direction = 4
}
```

### ProjectileData

Projectile configuration.

```csharp
namespace Data;

/// <summary>
/// Projectile configuration for projectile abilities.
/// </summary>
[GlobalClass]
public partial class ProjectileData : Resource
{
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public float Speed { get; private set; } = 300f;
    [Export] public float Lifetime { get; private set; } = 3f;
    [Export] public float Size { get; private set; } = 8f;
    [Export] public bool Piercing { get; private set; } = false;
    [Export] public int MaxPierceCount { get; private set; } = 1;
    [Export] public bool Homing { get; private set; } = false;
    [Export] public float HomingStrength { get; private set; } = 1f;
    [Export] public AudioStream HitSound { get; private set; }
}
```

---

## Effect Data

### EffectData

Status effect definition.

```csharp
namespace Data;

/// <summary>
/// Status effect definition.
/// </summary>
[GlobalClass]
public partial class EffectData : Resource
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    
    [ExportCategory("Type")]
    [Export] public EffectType Type { get; private set; } = EffectType.Buff;
    [Export] public EffectCategory Category { get; private set; } = EffectCategory.Positive;
    
    [ExportCategory("Duration")]
    [Export] public float Duration { get; private set; } = 5f;
    [Export] public bool IsPermanent { get; private set; } = false;
    
    [ExportCategory("Stacking")]
    [Export] public bool Stackable { get; private set; } = false;
    [Export] public int MaxStacks { get; private set; } = 1;
    [Export] public bool RefreshOnReapply { get; private set; } = true;
    
    [ExportCategory("Tick")]
    [Export] public bool HasTick { get; private set; } = false;
    [Export] public float TickInterval { get; private set; } = 1f;
    [Export] public int TickDamage { get; private set; } = 0;
    [Export] public int TickHeal { get; private set; } = 0;
    
    [ExportCategory("Stat Modifiers")]
    [Export] public StatModifier[] StatModifiers { get; private set; }
    
    [ExportCategory("Control")]
    [Export] public bool Stuns { get; private set; } = false;
    [Export] public bool Silences { get; private set; } = false;
    [Export] public bool Roots { get; private set; } = false;
    [Export] public float SlowPercent { get; private set; } = 0f;
    
    [ExportCategory("Visual")]
    [Export] public Color TintColor { get; private set; } = Colors.White;
    [Export] public PackedScene ParticleEffect { get; private set; }
}

public enum EffectCategory : byte
{
    Positive = 0,  // Buff
    Negative = 1,  // Debuff
    Neutral = 2    // Neither
}
```

---

## Dungeon Data

### DungeonData

Dungeon configuration.

```csharp
namespace Data;

/// <summary>
/// Dungeon configuration and generation parameters.
/// </summary>
[GlobalClass]
public partial class DungeonData : Resource
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    [Export] public Metadata MetaData { get; private set; }
    
    [ExportCategory("Structure")]
    [Export] public int TotalFloors { get; private set; } = 10;
    [Export] public BiomeData[] Biomes { get; private set; }
    
    [ExportCategory("Generation")]
    [Export] public int MinRoomsPerFloor { get; private set; } = 5;
    [Export] public int MaxRoomsPerFloor { get; private set; } = 12;
    [Export] public Vector2I MinRoomSize { get; private set; } = new(10, 10);
    [Export] public Vector2I MaxRoomSize { get; private set; } = new(20, 20);
    
    [ExportCategory("Difficulty")]
    [Export] public int BaseEnemyLevel { get; private set; } = 1;
    [Export] public float LevelScalingPerFloor { get; private set; } = 1f;
    [Export] public float EliteChance { get; private set; } = 0.1f;
    
    [ExportCategory("Loot")]
    [Export] public LootTable FloorLootTable { get; private set; }
    [Export] public float ChestSpawnChance { get; private set; } = 0.3f;
    
    [ExportCategory("Boss")]
    [Export] public EnemyData[] FloorBosses { get; private set; }
}
```

### BiomeData

Dungeon biome/theme configuration.

```csharp
namespace Data;

/// <summary>
/// Dungeon biome/theme configuration.
/// </summary>
[GlobalClass]
public partial class BiomeData : Resource
{
    [ExportCategory("Identity")]
    [Export] public InfoData Info { get; private set; }
    
    [ExportCategory("Floors")]
    [Export] public int StartFloor { get; private set; } = 1;
    [Export] public int EndFloor { get; private set; } = 3;
    
    [ExportCategory("Enemies")]
    [Export] public EnemyData[] CommonEnemies { get; private set; }
    [Export] public EnemyData[] EliteEnemies { get; private set; }
    [Export] public EnemyData BiomeBoss { get; private set; }
    
    [ExportCategory("Environment")]
    [Export] public TileSet Tileset { get; private set; }
    [Export] public Color AmbientColor { get; private set; } = Colors.White;
    [Export] public AudioStream AmbientMusic { get; private set; }
    [Export] public AudioStream BossMusic { get; private set; }
    
    [ExportCategory("Props")]
    [Export] public PackedScene[] DecorationPrefabs { get; private set; }
    [Export] public PackedScene[] HazardPrefabs { get; private set; }
}
```

### RoomData

Room template data.

```csharp
namespace Data;

/// <summary>
/// Room template configuration.
/// </summary>
[GlobalClass]
public partial class RoomData : Resource
{
    [ExportCategory("Type")]
    [Export] public RoomType Type { get; private set; } = RoomType.Combat;
    
    [ExportCategory("Size")]
    [Export] public Vector2I Size { get; private set; } = new(15, 15);
    
    [ExportCategory("Spawns")]
    [Export] public EnemySpawn[] EnemySpawns { get; private set; }
    [Export] public ChestSpawn[] ChestSpawns { get; private set; }
    [Export] public Vector2I[] DoorPositions { get; private set; }
    
    [ExportCategory("Layout")]
    [Export] public PackedScene RoomPrefab { get; private set; }
}

[GlobalClass]
public partial class EnemySpawn : Resource
{
    [Export] public EnemyData EnemyData { get; private set; }
    [Export] public Vector2 LocalPosition { get; private set; }
    [Export] public float SpawnChance { get; private set; } = 1f;
}

[GlobalClass]
public partial class ChestSpawn : Resource
{
    [Export] public ChestType ChestType { get; private set; }
    [Export] public Vector2 LocalPosition { get; private set; }
    [Export] public LootTable LootOverride { get; private set; }
}

public enum RoomType : byte
{
    Combat = 0,
    Treasure = 1,
    Challenge = 2,
    Boss = 3,
    Safe = 4,    // Waypoint/rest
    Shop = 5,
    Secret = 6
}

public enum ChestType : byte
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Boss = 3
}
```

---

## Loot Data

### LootTable

Loot drop configuration.

```csharp
namespace Data;

/// <summary>
/// Loot drop configuration.
/// </summary>
[GlobalClass]
public partial class LootTable : Resource
{
    [ExportCategory("Drops")]
    [Export] public LootEntry[] Entries { get; private set; }
    
    [ExportCategory("Drop Count")]
    [Export] public int MinDrops { get; private set; } = 1;
    [Export] public int MaxDrops { get; private set; } = 3;
    
    [ExportCategory("Chances")]
    [Export] public float NothingChance { get; private set; } = 0.1f;
    [Export] public float BonusDropChance { get; private set; } = 0.05f;
    
    [ExportCategory("Currency")]
    [Export] public int MinGold { get; private set; } = 0;
    [Export] public int MaxGold { get; private set; } = 10;
}

[GlobalClass]
public partial class LootEntry : Resource
{
    [Export] public ItemData Item { get; private set; }
    [Export] public float Weight { get; private set; } = 1f;
    [Export] public int MinQuantity { get; private set; } = 1;
    [Export] public int MaxQuantity { get; private set; } = 1;
    [Export] public int MinItemLevel { get; private set; } = 0;
    [Export] public int MaxItemLevel { get; private set; } = 0; // 0 = use dungeon floor
}
```

---

## Quest Data

### QuestData

Quest definition.

```csharp
namespace Data;

/// <summary>
/// Complete quest definition.
/// </summary>
[GlobalClass]
public partial class QuestData : Resource
{
    [ExportCategory("Identity")]
    [Export] public string QuestId { get; private set; }
    [Export] public InfoData Info { get; private set; }
    [Export] public QuestType Type { get; private set; } = QuestType.Main;
    
    [ExportCategory("Objectives")]
    [Export] public QuestObjective[] Objectives { get; private set; }
    
    [ExportCategory("Rewards")]
    [Export] public QuestReward[] Rewards { get; private set; }
    
    [ExportCategory("Prerequisites")]
    [Export] public string[] RequiredQuests { get; private set; }
    [Export] public int RequiredLevel { get; private set; } = 1;
    
    [ExportCategory("NPC")]
    [Export] public string QuestGiverId { get; private set; }
    [Export] public string TurnInNpcId { get; private set; }
    
    [ExportCategory("Dialogue")]
    [Export] public DialogueData AcceptDialogue { get; private set; }
    [Export] public DialogueData ProgressDialogue { get; private set; }
    [Export] public DialogueData CompleteDialogue { get; private set; }
}

[GlobalClass]
public partial class QuestObjective : Resource
{
    [Export] public string ObjectiveId { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public ObjectiveType Type { get; private set; }
    [Export] public string TargetId { get; private set; }
    [Export] public int RequiredCount { get; private set; } = 1;
    [Export] public bool IsOptional { get; private set; } = false;
}

[GlobalClass]
public partial class QuestReward : Resource
{
    [Export] public RewardType Type { get; private set; }
    [Export] public int Amount { get; private set; }
    [Export] public ItemData ItemReward { get; private set; }
}

public enum QuestType : byte
{
    Main = 0,
    Side = 1,
    Repeatable = 2,
    Hidden = 3
}

public enum ObjectiveType : byte
{
    Kill = 0,
    Collect = 1,
    Explore = 2,
    Talk = 3,
    Deliver = 4,
    DefeatBoss = 5,
    ReachFloor = 6
}

public enum RewardType : byte
{
    Experience = 0,
    Gold = 1,
    Item = 2,
    Ability = 3,
    Reputation = 4
}
```

---

## Dialogue Data

### DialogueData

Dialogue tree definition.

```csharp
namespace Data;

/// <summary>
/// Dialogue tree definition.
/// </summary>
[GlobalClass]
public partial class DialogueData : Resource
{
    [Export] public string DialogueId { get; private set; }
    [Export] public DialogueNode[] Nodes { get; private set; }
    [Export] public string StartNodeId { get; private set; }
}

[GlobalClass]
public partial class DialogueNode : Resource
{
    [Export] public string NodeId { get; private set; }
    [Export] public string SpeakerName { get; private set; }
    [Export] public Texture2D SpeakerPortrait { get; private set; }
    
    [Export(PropertyHint.MultilineText)] 
    public string Text { get; private set; }
    
    [Export] public DialogueChoice[] Choices { get; private set; }
    [Export] public string NextNodeId { get; private set; }
    
    [Export] public string TriggerEvent { get; private set; }
    [Export] public AudioStream VoiceLine { get; private set; }
}

[GlobalClass]
public partial class DialogueChoice : Resource
{
    [Export] public string Text { get; private set; }
    [Export] public string NextNodeId { get; private set; }
    [Export] public string RequiredFlag { get; private set; }
    [Export] public string SetFlag { get; private set; }
    [Export] public int ReputationChange { get; private set; } = 0;
}
```

---

## Index Data

Indices register data for runtime lookup.

### HeroIndex

```csharp
namespace Data;

/// <summary>
/// Registry of all hero definitions.
/// </summary>
[GlobalClass]
public partial class HeroIndex : Resource
{
    [Export] public HeroData[] Heroes { get; private set; }
    [Export] public HeroData DefaultHero { get; private set; }
    [Export] public HeroData DebugHero { get; private set; }
    
    public HeroData GetById(string id)
    {
        return Heroes?.FirstOrDefault(h => h.MetaData?.UniqueId == id);
    }
    
    public HeroData GetByClass(HeroClass heroClass)
    {
        return Heroes?.FirstOrDefault(h => h.Class == heroClass);
    }
}
```

### ItemIndex

```csharp
namespace Data;

/// <summary>
/// Registry of all item definitions.
/// </summary>
[GlobalClass]
public partial class ItemIndex : Resource
{
    [Export] public ItemData[] AllItems { get; private set; }
    [Export] public WeaponData[] Weapons { get; private set; }
    [Export] public ArmorData[] Armors { get; private set; }
    [Export] public ItemData[] Consumables { get; private set; }
    [Export] public ItemData[] Materials { get; private set; }
    
    public ItemData GetById(string id)
    {
        return AllItems?.FirstOrDefault(i => i.MetaData?.UniqueId == id);
    }
    
    public ItemData[] GetByType(ItemType type)
    {
        return AllItems?.Where(i => i.Type == type).ToArray();
    }
    
    public ItemData[] GetByRarity(ItemRarity rarity)
    {
        return AllItems?.Where(i => i.Rarity == rarity).ToArray();
    }
}
```

### EnemyIndex

```csharp
namespace Data;

/// <summary>
/// Registry of all enemy definitions.
/// </summary>
[GlobalClass]
public partial class EnemyIndex : Resource
{
    [Export] public EnemyData[] AllEnemies { get; private set; }
    [Export] public EnemyData[] Bosses { get; private set; }
    
    public EnemyData GetById(string id)
    {
        return AllEnemies?.FirstOrDefault(e => e.MetaData?.UniqueId == id);
    }
    
    public EnemyData[] GetByTribe(EnemyTribe tribe)
    {
        return AllEnemies?.Where(e => e.Tribe == tribe).ToArray();
    }
    
    public EnemyData[] GetByLevel(int minLevel, int maxLevel)
    {
        return AllEnemies?.Where(e => e.Level >= minLevel && e.Level <= maxLevel).ToArray();
    }
}
```

### AbilityIndex

```csharp
namespace Data;

/// <summary>
/// Registry of all ability definitions.
/// </summary>
[GlobalClass]
public partial class AbilityIndex : Resource
{
    [Export] public AbilityData[] AllAbilities { get; private set; }
    
    public AbilityData GetById(string id)
    {
        return AllAbilities?.FirstOrDefault(a => a.Info?.Named == id);
    }
    
    public AbilityData[] GetByClass(HeroClass heroClass)
    {
        // Filter by class tag or other mechanism
        return AllAbilities;
    }
}
```

### EntityIndex

```csharp
namespace Data;

/// <summary>
/// Registry of entity scene templates.
/// </summary>
[GlobalClass]
public partial class EntityIndex : Resource
{
    [Export] public PackedScene HeroTemplate { get; private set; }
    [Export] public PackedScene EnemyTemplate { get; private set; }
    [Export] public PackedScene ItemTemplate { get; private set; }
    [Export] public PackedScene ProjectileTemplate { get; private set; }
    [Export] public PackedScene NPCTemplate { get; private set; }
    [Export] public PackedScene ChestTemplate { get; private set; }
}
```

---

## Related Documents

- [00-game-design-document.md](00-game-design-document.md) - Game design overview
- [01-technical-architecture.md](01-technical-architecture.md) - System architecture
- [02-systems-design.md](02-systems-design.md) - System specifications
- [03-component-design.md](03-component-design.md) - Component specifications
- [05-implementation-roadmap.md](05-implementation-roadmap.md) - Development phases
