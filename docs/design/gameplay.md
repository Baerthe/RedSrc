# Gameplay Design

> **Version**: 0.1.0  
> **Last Updated**: 2026-01-14

## Overview

RedGate features two distinct gameplay modes that form a compelling loop: high-intensity **Extraction** gameplay and strategic **Downtime** management. This document details the mechanics, systems, and player-facing features that make up the game experience.

## The Prospector

The player controls a **Prospector**, a powerful Dwarven Weaver sent to the surface to gather resources and Voices for Greater Dware.

### Character Design

Prospectors are user-created dwarves with:
- **No heroes**: Just loadouts - any prospector can use any equipment
- **Meta-progression**: Permanent upgrades that persist across deaths
- **Equipment slots**: Weapon, armor, accessories, consumables
- **Chants**: Auto-attack abilities unlocked by collecting Voices

### Equipment Slots

| Slot | Description |
|------|-------------|
| **Primary Weapon** | Main auto-attack source |
| **Secondary Weapon** | Alternate attack or utility |
| **Head** | Helmet/hood with defensive stats |
| **Body** | Armor providing protection |
| **Accessory 1** | Ring, amulet, or trinket |
| **Accessory 2** | Secondary accessory slot |
| **Consumable (x3)** | One-time use items |
| **Chant Slots (x4)** | Active auto-attack abilities |

## Extraction Mode

The core action gameplay loop where players venture to surface "Claims" to gather resources.

### Core Loop

```
Enter Claim → Explore → Combat Waves → Collect Loot → Extract or Die
     ↑                                                       │
     └───────────────────── Repeat ──────────────────────────┘
```

### Map Structure

Maps are procedurally generated from templates:

```
┌──────────────────────────────────────┐
│           Spawn Zone                 │
│              ▼                       │
│   ┌──────────────────────┐           │
│   │    Main Area         │           │
│   │  • Enemy spawns      │           │
│   │  • Loot containers   │           │
│   │  • Points of Interest│           │
│   └──────────────────────┘           │
│        │           │                 │
│   ┌────▼────┐ ┌────▼────┐           │
│   │ Branch A│ │ Branch B│           │
│   └────┬────┘ └────┬────┘           │
│        └─────┬─────┘                 │
│              ▼                       │
│         Lodestone                    │
│     (Extraction Point)               │
└──────────────────────────────────────┘
```

### Wave System

Enemies spawn in escalating waves:

| Time | Wave Type | Intensity |
|------|-----------|-----------|
| 0-2 min | Scouts | Low - Learn the area |
| 2-5 min | Patrols | Medium - Sustained combat |
| 5-10 min | Assault | High - Heavy engagement |
| 10-15 min | Siege | Very High - Overwhelming |
| 15+ min | Purge | Extreme - Evacuation critical |

```csharp
// Difficulty scaling formula
float DifficultyMultiplier = 1.0f + (ElapsedMinutes * 0.15f);
int SpawnCount = BaseSpawnCount * (1 + WaveNumber / 3);
```

### Combat System

#### Auto-Attacks
Combat is primarily automatic - player focuses on positioning:

```
Player Movement → Enemies Approach → Auto-Attacks Fire → Damage Applied
                                           ↑
                            Chants provide varied attack patterns
```

#### Chant System

Chants are auto-attack abilities powered by collected Voices:

| Chant Type | Pattern | Example |
|------------|---------|---------|
| **Projectile** | Fire in direction | Stone Bolt |
| **Nova** | 360° burst | Earth Shatter |
| **Orbit** | Circling player | Guardian Spirits |
| **Area** | Ground effect | Corrosive Pool |
| **Chain** | Jump between targets | Lightning Arc |
| **Beam** | Continuous stream | Flame Gout |

```csharp
public class ChantData : Resource
{
    public string Name { get; set; }
    public ChantPattern Pattern { get; set; }
    public float Damage { get; set; }
    public float Cooldown { get; set; }
    public float Range { get; set; }
    public int VoiceCost { get; set; }  // Cost to unlock
    public EffectData[] Effects { get; set; }
}
```

#### Damage Calculation

```csharp
uint CalculateDamage(IEntity attacker, IEntity target, uint baseDamage)
{
    float damage = baseDamage;
    
    // Attacker modifiers
    damage += attacker.DamageBonus;
    damage *= attacker.DamageMultiplier;
    
    // Target modifiers
    damage -= target.Armor;
    damage *= (1 - target.DamageReduction);
    
    // Critical hits
    if (Random.Value < attacker.CritChance)
        damage *= attacker.CritMultiplier;
    
    return Math.Max(1, (uint)damage);
}
```

### Loot System

#### Auto-Collection
Items are collected automatically by proximity:

```
Collection Radius: 50 units (default)
Collection Speed: 200 units/second
Magnet Effect: Pulls items from 150 units (upgraded)
```

#### Slot Machine Mechanic

Loot containers trigger a slot-machine UI:

```
┌─────────────────────────────────┐
│      [ ? ] [ ? ] [ ? ]          │
│        ↓     ↓     ↓            │
│      [◆]   [♦]   [★]           │
│     Common Rare  Epic           │
│                                 │
│      Rolling... [STOP]          │
└─────────────────────────────────┘
```

```csharp
public class SlotMachine
{
    void StartRoll(ChestData chest);
    void StopRoll(); // Manual or auto after delay
    ItemData[] GetResults();
    
    // Rarity weights affected by:
    // - Chest tier
    // - Time survived
    // - Luck stat
    // - Meta-progression bonuses
}
```

### Extraction

#### Lodestones
Extraction points scattered across the map:

```csharp
public class Lodestone
{
    Vector2 Position { get; }
    float ActivationTime { get; }  // Time to charge portal
    bool IsActive { get; }
    
    void BeginActivation();  // Start extraction countdown
    void CancelActivation(); // Interrupted by damage
    void CompleteExtraction();
}
```

#### Extraction Process

1. **Locate Lodestone**: Find extraction point on map
2. **Clear Area**: Defeat nearby enemies
3. **Activate**: Stand in circle to charge (5-10 seconds)
4. **Defend**: Waves intensify during charging
5. **Extract**: Portal opens, safely return to Dagsgard

#### Death Consequences

On death:
- **Lost**: All items gathered this extraction
- **Lost**: Accumulated Voices (unless banked at Lodestone)
- **Kept**: Meta-progression, permanent upgrades
- **Return**: Respawn in Dagsgard with loadout intact

### Voices (Souls)

Voices are the unique extraction currency:

```csharp
public class VoiceSystem
{
    int CarriedVoices { get; }      // Lost on death
    int BankedVoices { get; }       // Safe at Lodestones
    int TotalExtracted { get; }     // Lifetime total
    
    void CollectVoice(int value);
    void BankVoices();              // At Lodestone
    void DonateVoices(int amount);  // To Greater Dware
}
```

**Voice Uses:**
- Unlock new Chants
- Power Dwarevolenn (faction progression)
- Trade with NPCs
- Unlock story content

## Downtime Mode

Strategic management between extractions, presented through a Fantasy OS interface.

### Core Loop

```
Extract Successfully → Return to Dagsgard → Manage Resources → Prepare → Extract Again
                              │
                              ├── Craft equipment
                              ├── Upgrade workbenches
                              ├── Talk to NPCs
                              ├── Check quests
                              └── Idle activities
```

### Fantasy OS Interface

The downtime mode is presented as a retro OS desktop:

```
┌──────────────────────────────────────────────────────┐
│ ═══════════════ DAGSGARD OS v0.8 ════════════════   │
├──────────────────────────────────────────────────────┤
│                                                      │
│   [📁]        [⚒️]        [📜]        [📬]         │
│  Inventory   Forge      Quests     Messages         │
│                                                      │
│   [🗺️]        [🏠]        [📊]        [⚙️]         │
│   Claims    Workshop    Status    Settings          │
│                                                      │
│                                                      │
│  ┌─────────────────────────────────────────────┐    │
│  │ New Message: Prospecting Guild              │    │
│  │ "Your services are required at Claim #47"   │    │
│  └─────────────────────────────────────────────┘    │
│                                                      │
│ [START] ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ 12:34 Day 15  │
└──────────────────────────────────────────────────────┘
```

### Crafting System

#### Workbenches

| Workbench | Unlocks | Crafts |
|-----------|---------|--------|
| **Basic** | Start | Simple gear, consumables |
| **Forge** | 100 Voices | Weapons, armor |
| **Alchemy** | 250 Voices | Potions, buffs |
| **Enchanting** | 500 Voices | Upgrades, enchantments |
| **Engineering** | 750 Voices | Gadgets, traps |

#### Recipe System

```csharp
public class CraftData : Resource
{
    public InfoData Info { get; set; }
    public WorkbenchType RequiredWorkbench { get; set; }
    public CraftIngredient[] Ingredients { get; set; }
    public ItemData Result { get; set; }
    public int ResultQuantity { get; set; }
    public float CraftTime { get; set; }
    public bool IsUnlocked { get; set; }
}

public class CraftIngredient
{
    public ItemData Item { get; set; }
    public int Quantity { get; set; }
}
```

### Idle Systems

#### Gardening
Grow resources over real time:

```csharp
public class GardenPlot
{
    PlantData CurrentPlant { get; }
    float GrowthProgress { get; }    // 0-100%
    float GrowthTime { get; }        // Real seconds
    
    void Plant(PlantData plant);
    ItemData[] Harvest();
}
```

#### Training Grounds
Passive stat increases:

```csharp
public class TrainingActivity
{
    StatType TargetStat { get; }
    float ProgressRate { get; }
    int MaxLevel { get; }
    
    void StartTraining();
    void CollectProgress();
}
```

### NPCs and Narrative

#### NPC Types

| NPC | Role | Location |
|-----|------|----------|
| **Guild Master** | Quest giver, progression | Guild Hall |
| **Forge Master** | Crafting, upgrades | Forge |
| **Lore Keeper** | Story, world building | Library |
| **Merchant** | Buy/sell items | Market |
| **Voice Keeper** | Voice management | Temple |

#### Dialogue System

```csharp
public class DialogueData : Resource
{
    public string NPCId { get; set; }
    public DialogueNode[] Nodes { get; set; }
}

public class DialogueNode
{
    public string Id { get; set; }
    public string SpeakerName { get; set; }
    public string Text { get; set; }
    public DialogueOption[] Options { get; set; }
    public string NextNodeId { get; set; }  // If no options
}

public class DialogueOption
{
    public string Text { get; set; }
    public string NextNodeId { get; set; }
    public DialogueCondition[] Conditions { get; set; }
    public DialogueAction[] Actions { get; set; }
}
```

### Quest System

#### Quest Types

| Type | Description | Example |
|------|-------------|---------|
| **Main** | Story progression | "Investigate the Flock Camp" |
| **Side** | Optional content | "Collect 50 Iron Ore" |
| **Daily** | Repeatable | "Complete 3 Extractions" |
| **Guild** | Faction reputation | "Donate 100 Voices" |

#### Quest Structure

```csharp
public class QuestData : Resource
{
    public InfoData Info { get; set; }
    public QuestType Type { get; set; }
    public QuestObjective[] Objectives { get; set; }
    public QuestReward[] Rewards { get; set; }
    public string[] Prerequisites { get; set; }
    public bool IsRepeatable { get; set; }
}

public class QuestObjective
{
    public string Id { get; set; }
    public string Description { get; set; }
    public ObjectiveType Type { get; set; }
    public string TargetId { get; set; }
    public int RequiredCount { get; set; }
    public int CurrentCount { get; set; }
}
```

## Progression Systems

### Meta-Progression

Permanent upgrades persisting across deaths:

| Category | Examples |
|----------|----------|
| **Stats** | +Max Health, +Speed, +Damage |
| **Unlocks** | New Chants, equipment blueprints |
| **Quality of Life** | Larger inventory, better loot chances |
| **Facilities** | New workbenches, garden plots |

### Voice Donation

Donating Voices to Dwarevolenn:

```csharp
public class DwarevolennProgression
{
    int TotalVoicesDonated { get; }
    int CurrentTier { get; }
    
    void DonateVoices(int amount);
    Blessing[] GetUnlockedBlessings();
    float GetGlobalBonus(StatType stat);
}
```

**Blessing Tiers:**
- Tier 1 (100 Voices): Basic stat bonuses
- Tier 2 (500 Voices): Passive abilities
- Tier 3 (2000 Voices): Major perks
- Tier 4 (10000 Voices): Transformative powers

### Skill System

Inspired by Elder Scrolls - skills improve through use:

```csharp
public class SkillData
{
    public string Name { get; set; }
    public SkillCategory Category { get; set; }
    public int Level { get; set; }
    public float Experience { get; set; }
    
    void AddExperience(float amount);
    int GetLevelUpThreshold();
}

public enum SkillCategory
{
    Combat,     // Weapon skills, defense
    Gathering,  // Mining, harvesting
    Crafting,   // Forging, alchemy
    Movement,   // Speed, evasion
    Magic       // Chant power, efficiency
}
```

## Enemy Design

### The Flock

Primary enemy faction - religious militants hunting non-followers.

#### Enemy Types

| Type | Behavior | Threat |
|------|----------|--------|
| **Initiate** | Basic melee | Low |
| **Acolyte** | Ranged attacks | Low-Medium |
| **Zealot** | Fast, aggressive | Medium |
| **Priest** | Buffs allies | Medium |
| **Inquisitor** | Tanky, dangerous | High |
| **Bishop** | Mini-boss | Very High |
| **Cardinal** | Boss | Extreme |

### AI Behaviors

```csharp
public enum AIBehavior
{
    Aggressive,    // Rush player
    Defensive,     // Keep distance, ranged
    Support,       // Buff allies, heal
    Flanking,      // Circle player
    Swarming,      // Group behavior
    Boss           // Complex patterns
}
```

### Spawn Patterns

```csharp
public class MobTable : Resource
{
    public MobSpawnEntry[] Entries { get; set; }
    public int MinWaveSize { get; set; }
    public int MaxWaveSize { get; set; }
    public float SpawnRateMultiplier { get; set; }
}

public class MobSpawnEntry
{
    public MobData Mob { get; set; }
    public float Weight { get; set; }          // Spawn probability
    public int MinDifficultyLevel { get; set; } // When this mob appears
    public int MaxCount { get; set; }           // Cap per wave
}
```

## World Structure

### Greater Dware (Hub)

Underground dwarven civilization, specifically Dagsgard warren.

**Areas:**
- **Prospector's Lodge**: Player home, storage
- **Guild Hall**: Quests, progression
- **Market**: Trading, merchants
- **Forge District**: Crafting workbenches
- **Temple**: Voice management, blessings
- **Library**: Lore, bestiary

### Surface Claims

Extraction zones on the hostile surface.

**Claim Properties:**
- **Type**: Forest, Ruins, Settlement, Cave
- **Tier**: Difficulty level (1-5)
- **Resources**: Available loot types
- **Hazards**: Environmental dangers
- **Boss**: Optional boss encounter

### Overworld Map

```
┌─────────────────────────────────────┐
│        GRUNWEALD SURFACE            │
│                                     │
│    [?]──[2]──[?]                    │
│     │    │    │                     │
│    [1]──[★]──[3]──[?]              │
│     │    │    │                     │
│    [?]──[1]──[2]──[4]              │
│                                     │
│ [★] = Greater Dware (Safe Zone)     │
│ [#] = Claim Tier                    │
│ [?] = Unexplored                    │
└─────────────────────────────────────┘
```

## Balancing Parameters

### Core Numbers

```csharp
// Player baseline
BaseHealth = 100;
BaseSpeed = 100f;
BaseDamage = 10;

// Scaling
HealthPerLevel = 10;
DamagePerLevel = 2;
XPScaling = 1.15f;  // Each level needs 15% more XP

// Combat
BaseCritChance = 0.05f;
BaseCritMultiplier = 2.0f;

// Economy
VoicePerMinute = 5;        // Expected Voice income
ItemDropChance = 0.1f;     // Per enemy
RareDropChance = 0.01f;
```

### Difficulty Curve

```
Minute 0-2:   Difficulty 1.0x - Introduction
Minute 2-5:   Difficulty 1.3x - Warming up
Minute 5-10:  Difficulty 1.8x - Challenge
Minute 10-15: Difficulty 2.5x - Intense
Minute 15+:   Difficulty 3.5x+ - Evacuation
```

---

*See [Data Structures](data-structures.md) for technical definitions of gameplay elements.*
