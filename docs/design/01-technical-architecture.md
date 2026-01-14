# Technical Architecture Document
## RedGate: Tactical Dungeon RPG

> This document details the technical architecture for the tactical RPG adaptation of RedSrc.

---

## Architecture Overview

The RedGate tactical RPG builds upon the existing RedSrc layered architecture, maintaining the ECS-inspired, data-driven approach while adapting systems for the new game mechanics.

```
┌─────────────────────────────────────────────────────────────┐
│                    ARCHITECTURE LAYERS                       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Layer 6: Utility (Assisting Logic)                          │
│  ─────────────────────────────────────                       │
│  Pure C# classes for algorithms, calculations, helpers       │
│                                                              │
│  Layer 5: System (Controlling Logic)                         │
│  ────────────────────────────────────                        │
│  Node-based systems managing entity groups and game logic    │
│                                                              │
│  Layer 4: Manager (Orchestration)                            │
│  ────────────────────────────────                            │
│  DungeonManager, TownManager - scene root controllers        │
│                                                              │
│  Layer 3: Entity (Building Blocks)                           │
│  ─────────────────────────────────                           │
│  Pre-configured scene templates for game objects             │
│                                                              │
│  Layer 2B: Component (Behavior)                              │
│  ──────────────────────────────                              │
│  Modular node attachments defining entity characteristics    │
│                                                              │
│  Layer 2A: Data (Abstractions)                               │
│  ──────────────────────────────                              │
│  Resource classes for shared, stateless data assets          │
│                                                              │
│  Layer 1B: Service (Primary Services)                        │
│  ────────────────────────────────────                        │
│  Plain C# service classes for core functionality             │
│                                                              │
│  Layer 1A: Core (Infrastructure)                             │
│  ───────────────────────────────                             │
│  Singleton nodes for global orchestration                    │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Layer 1A: Core Infrastructure

The foundation layer provides global orchestration and dependency management.

### MainCore (Root Node)

The scene tree root handling initialization and global state.

```csharp
namespace Core;

public sealed partial class MainCore : Node2D
{
    public static MainCore Instance { get; private set; }
    
    // Core nodes
    [Export] public CameraCore CameraCore { get; private set; }
    [Export] public ContextCore ContextCore { get; private set; }
    [Export] public EventCore EventCore { get; private set; }
    [Export] public ServiceCore ServiceCore { get; private set; }
    [Export] public StateCore StateCore { get; private set; }
    
    // Managers
    [Export] public DungeonManager DungeonManager { get; private set; }
    [Export] public TownManager TownManager { get; private set; }
    [Export] public UiManager UiManager { get; private set; }
    
    // Indices (data registries)
    [Export] public EntityIndex EntityTemplates { get; private set; }
    [Export] public HeroIndex Heroes { get; private set; }
    [Export] public ItemIndex Items { get; private set; }
    [Export] public AbilityIndex Abilities { get; private set; }
    [Export] public DungeonIndex Dungeons { get; private set; }
}
```

### EventCore (Event Bus)

Global publish/subscribe event system with timer-based pulses.

```csharp
namespace Core;

public sealed partial class EventCore : Node2D
{
    public Heart Heart { get; private set; }
    
    // Event subscription dictionaries
    private Dictionary<Type, List<Action>> _subs;
    private Dictionary<Type, List<Action<IEvent>>> _subsData;
    
    // Timer intervals for tactical RPG
    private const float PulseInterval = 0.05f;        // 20 Hz - physics/AI
    private const float SlowPulseInterval = 0.5f;     // 2 Hz - regeneration
    private const float CombatTickInterval = 0.1f;    // 10 Hz - combat calculations
    
    public void Subscribe<T>(Action handler);
    public void Subscribe<T>(Action<IEvent> handler);
    public void Unsubscribe<T>(Action handler);
    public void Unsubscribe<T>(Action<IEvent> handler);
    public void Publish<T>();
    public void Publish<T>(IEvent eventData);
}
```

### StateCore (Game State Management)

Manages game state transitions between dungeon and town modes.

```csharp
namespace Core;

public sealed partial class StateCore : Node2D
{
    public GameState CurrentState { get; private set; }
    
    public void ChangeState(GameState newState);
    public bool IsInDungeon => CurrentState == GameState.Dungeon;
    public bool IsInTown => CurrentState == GameState.Town;
}

public enum GameState : byte
{
    None = 0,
    MainMenu = 1,
    Town = 2,
    Dungeon = 3,
    Paused = 4,
    Loading = 5,
    GameOver = 6,
    Victory = 7
}
```

### ServiceCore (Service Registry)

Manages service registration and dependency resolution.

```csharp
namespace Core;

public sealed partial class ServiceCore : Node2D
{
    internal static Registry ServiceRegistry { get; private set; }
    
    // Service accessors
    public static IAudioService AudioService();
    public static ISaveService SaveService();
    public static IInputService InputService();
    public static ISceneService SceneService();
    public static IHeroService HeroService();
    public static IInventoryService InventoryService();
    public static IQuestService QuestService();
}
```

### Heart (Timer Manager)

Internal class for managing game timers and pulses.

```csharp
namespace Core;

internal sealed partial class Heart : Node
{
    private Dictionary<string, Timer> _timers;
    
    public void BuildTimer<T>(string name, float interval, bool oneShot, bool autoStart);
    public void PauseTimers();
    public void ResumeTimers();
    public void StopTimer(string name);
}
```

### Registry (Dependency Container)

Simple dependency injection container.

```csharp
namespace Core;

internal sealed class Registry
{
    private Dictionary<Type, object> _instances;
    
    public void Register<TInterface, TImplementation>() where TImplementation : TInterface, new();
    public void Register<TInterface>(TInterface instance);
    public TInterface Resolve<TInterface>();
    public bool TryResolve<TInterface>(out TInterface instance);
}
```

---

## Layer 1B: Primary Services

Plain C# service classes providing core functionality.

### Service Definitions

| Service | Responsibility |
|---------|----------------|
| **AudioService** | Sound playback, music management, volume control |
| **SaveService** | Persistent data serialization, slot management |
| **InputService** | Input mapping, action detection, rebinding |
| **SceneService** | Scene transitions, async loading |
| **HeroService** | Active hero state, class data, progression |
| **InventoryService** | Item management, equipment, stash |
| **QuestService** | Quest tracking, objective state, rewards |
| **SettingsService** | User preferences, graphics options |
| **IndexService** | Runtime data indexing and lookup |

### Service Interface Example

```csharp
namespace Interface;

public interface IHeroService
{
    HeroData CurrentHero { get; }
    int CurrentLevel { get; }
    int CurrentExperience { get; }
    
    void LoadHero(HeroData heroData);
    void GainExperience(int amount);
    void LevelUp();
    void AllocateStat(StatType stat, int points);
    void UnlockAbility(AbilityData ability);
}
```

---

## Layer 2A: Data Abstractions

Resource classes inheriting from `Godot.Resource` for shared, stateless data.

### Core Data Types

```csharp
namespace Data;

// Common data structures
[GlobalClass]
public partial class InfoData : Resource
{
    [Export] public string Named { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public string Lore { get; private set; }
}

[GlobalClass]
public partial class StatsData : Resource
{
    [Export] public int MaxHealth { get; private set; } = 100;
    [Export] public int MaxMana { get; private set; } = 50;
    [Export] public float Speed { get; private set; } = 100f;
    [Export] public int Strength { get; private set; } = 10;
    [Export] public int Agility { get; private set; } = 10;
    [Export] public int Intelligence { get; private set; } = 10;
    [Export] public int Wisdom { get; private set; } = 10;
    [Export] public int Vitality { get; private set; } = 10;
}

[GlobalClass]
public partial class AssetData : Resource
{
    [Export] public SpriteFrames Sprite { get; private set; }
    [Export] public AudioStream HitSound { get; set; }
    [Export] public AudioStream DeathSound { get; set; }
    [Export] public Shape2D CollisionShape { get; set; }
    [Export] public Color TintColor { get; set; } = Colors.White;
}
```

### Entity Data Types

```csharp
// Hero definition
[GlobalClass]
public partial class HeroData : Resource, IData
{
    [Export] public InfoData Info { get; private set; }
    [Export] public StatsData BaseStats { get; private set; }
    [Export] public AssetData Assets { get; private set; }
    [Export] public HeroClass Class { get; private set; }
    [Export] public AbilityData[] Abilities { get; private set; }
}

// Enemy definition
[GlobalClass]
public partial class EnemyData : Resource, IData
{
    [Export] public InfoData Info { get; private set; }
    [Export] public StatsData Stats { get; private set; }
    [Export] public AssetData Assets { get; private set; }
    [Export] public EnemyType Type { get; private set; }
    [Export] public AIBehavior Behavior { get; private set; }
    [Export] public LootTable LootTable { get; private set; }
    [Export] public int ExperienceReward { get; private set; }
}

// Item definition
[GlobalClass]
public partial class ItemData : Resource, IData
{
    [Export] public InfoData Info { get; private set; }
    [Export] public AssetData Assets { get; private set; }
    [Export] public ItemType Type { get; private set; }
    [Export] public ItemRarity Rarity { get; private set; }
    [Export] public EquipSlot EquipSlot { get; private set; }
    [Export] public StatModifier[] Modifiers { get; private set; }
    [Export] public int Value { get; private set; }
}

// Ability definition
[GlobalClass]
public partial class AbilityData : Resource
{
    [Export] public InfoData Info { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    [Export] public AbilityType Type { get; private set; }
    [Export] public float Cooldown { get; private set; }
    [Export] public int ManaCost { get; private set; }
    [Export] public int BaseDamage { get; private set; }
    [Export] public float Range { get; private set; }
    [Export] public EffectData[] Effects { get; private set; }
}
```

---

## Layer 2B: Behavior Components

Modular node components attached to entities.

### Component Design Principles

1. **Data Containers**: Components store state, systems process logic
2. **No `_Process`**: Logic handled by systems for performance
3. **Self-Validation**: `_GetConfigurationWarnings()` for editor feedback
4. **System Registration**: Auto-register with systems on tree entry

### Core Components

```csharp
namespace Component;

// Health management
[GlobalClass]
public partial class HealthComponent : Node, IComponent
{
    [Export] public int MaxHealth { get; set; } = 100;
    [Export] public int CurrentHealth { get; set; } = 100;
    [Export] public float HealthRegen { get; set; } = 0f;
    
    public bool IsDead => CurrentHealth <= 0;
    public float HealthPercent => (float)CurrentHealth / MaxHealth;
}

// Mana/resource management
[GlobalClass]
public partial class ManaComponent : Node, IComponent
{
    [Export] public int MaxMana { get; set; } = 50;
    [Export] public int CurrentMana { get; set; } = 50;
    [Export] public float ManaRegen { get; set; } = 1f;
    
    public bool CanCast(int cost) => CurrentMana >= cost;
    public void SpendMana(int cost) => CurrentMana = Math.Max(0, CurrentMana - cost);
}

// Movement/physics
[GlobalClass]
public partial class PhysicsComponent : CharacterBody2D, IComponent
{
    [Export] public float Speed { get; set; } = 100f;
    [Export] public float Acceleration { get; set; } = 500f;
    [Export] public Vector2 TargetVelocity { get; set; }
    [Export] public Vector2 FacingDirection { get; set; } = Vector2.Down;
}

// Combat stats
[GlobalClass]
public partial class CombatComponent : Node, IComponent
{
    [Export] public int BaseDamage { get; set; } = 10;
    [Export] public int Armor { get; set; } = 0;
    [Export] public float CritChance { get; set; } = 0.05f;
    [Export] public float CritMultiplier { get; set; } = 1.5f;
    [Export] public float AttackSpeed { get; set; } = 1f;
    [Export] public float AttackCooldown { get; set; } = 0f;
}

// Ability system
[GlobalClass]
public partial class AbilityComponent : Node, IComponent
{
    [Export] public AbilityData[] Abilities { get; set; }
    public Dictionary<int, float> Cooldowns { get; } = new();
    
    public bool IsOnCooldown(int slot) => Cooldowns.TryGetValue(slot, out var cd) && cd > 0;
}

// AI behavior
[GlobalClass]
public partial class AIComponent : Node, IComponent
{
    [Export] public AIBehavior Behavior { get; set; }
    [Export] public float DetectionRange { get; set; } = 200f;
    [Export] public float AttackRange { get; set; } = 50f;
    
    public Node2D Target { get; set; }
    public AIState CurrentState { get; set; } = AIState.Idle;
}

// Inventory
[GlobalClass]
public partial class InventoryComponent : Node, IComponent
{
    [Export] public int Capacity { get; set; } = 20;
    public List<ItemInstance> Items { get; } = new();
    public Dictionary<EquipSlot, ItemInstance> Equipped { get; } = new();
}
```

---

## Layer 3: Building Block Entities

Pre-configured scene templates instantiated at runtime.

### Entity Structure

```
Entity Scene Structure:
├── Root Node (Entity script)
│   ├── PhysicsComponent (CharacterBody2D)
│   │   └── CollisionShape2D
│   ├── HealthComponent (Node)
│   ├── CombatComponent (Node)
│   ├── RenderComponent (Sprite2D/AnimatedSprite2D)
│   └── [Additional Components]
```

### Entity Interfaces

```csharp
namespace Interface;

public interface IEntity
{
    void Inject(IData data);
    void NullCheck();
}

public interface IDamageable
{
    void TakeDamage(DamageInfo damage);
    void Die();
}

public interface IInteractable
{
    bool IsInteractable { get; }
    void OnInteract(IEntity interactor);
}
```

### Entity Examples

```csharp
// Hero entity
[GlobalClass]
public partial class HeroEntity : Node2D, IEntity, IDamageable
{
    [Export] public PhysicsComponent Physics { get; private set; }
    [Export] public HealthComponent Health { get; private set; }
    [Export] public ManaComponent Mana { get; private set; }
    [Export] public CombatComponent Combat { get; private set; }
    [Export] public AbilityComponent Abilities { get; private set; }
    [Export] public InventoryComponent Inventory { get; private set; }
    [Export] public AnimatedSprite2D Sprite { get; private set; }
    
    public HeroData Data { get; private set; }
}

// Enemy entity
[GlobalClass]
public partial class EnemyEntity : Node2D, IEntity, IDamageable
{
    [Export] public PhysicsComponent Physics { get; private set; }
    [Export] public HealthComponent Health { get; private set; }
    [Export] public CombatComponent Combat { get; private set; }
    [Export] public AIComponent AI { get; private set; }
    [Export] public AnimatedSprite2D Sprite { get; private set; }
    
    public EnemyData Data { get; private set; }
}
```

---

## Layer 4: Orchestration Managers

Root scene nodes controlling game modes.

### DungeonManager

Controls gameplay in the dungeon environment.

```csharp
namespace Manager;

[GlobalClass]
public partial class DungeonManager : Node2D, IManager
{
    // Systems
    public CombatSystem CombatSystem { get; private set; }
    public AISystem AISystem { get; private set; }
    public SpawnSystem SpawnSystem { get; private set; }
    public LootSystem LootSystem { get; private set; }
    public PhysicsSystem PhysicsSystem { get; private set; }
    public AbilitySystem AbilitySystem { get; private set; }
    
    // Dungeon state
    public DungeonData CurrentDungeon { get; private set; }
    public int CurrentFloor { get; private set; }
    public HeroEntity Player { get; private set; }
    
    public void Initialize();
    public void LoadFloor(int floor);
    public void ReturnToTown();
}
```

### TownManager

Controls gameplay in the town hub.

```csharp
namespace Manager;

[GlobalClass]
public partial class TownManager : Node2D, IManager
{
    // Systems
    public DialogueSystem DialogueSystem { get; private set; }
    public CraftSystem CraftSystem { get; private set; }
    public ShopSystem ShopSystem { get; private set; }
    public QuestSystem QuestSystem { get; private set; }
    
    // Town state
    public TownData CurrentTown { get; private set; }
    public HeroEntity Player { get; private set; }
    
    public void Initialize();
    public void OpenShop(NPCEntity vendor);
    public void OpenCrafting(NPCEntity blacksmith);
    public void EnterDungeon();
}
```

---

## Layer 5: Controlling Logic Systems

Node-based systems processing entity groups.

### System Design Principles

1. **Batch Processing**: Iterate over component groups
2. **Centralized Logic**: Systems own the "what", components own the "data"
3. **Timer-Based Updates**: Throttled updates for performance
4. **Event-Driven**: React to EventCore publications

### Core Systems

```csharp
namespace System;

// Combat damage and effects
public partial class CombatSystem : Node, ISystem
{
    private List<CombatComponent> _combatants;
    
    public void ProcessAttack(IEntity attacker, IEntity target);
    public void ApplyDamage(IDamageable target, DamageInfo damage);
    public void ProcessDeath(IEntity entity);
}

// AI decision making
public partial class AISystem : Node, ISystem
{
    private List<AIComponent> _aiEntities;
    
    public void Update(double delta);
    public void ProcessBehavior(AIComponent ai);
    public void FindTarget(AIComponent ai);
}

// Entity movement
public partial class PhysicsSystem : Node, ISystem
{
    private List<PhysicsComponent> _bodies;
    
    public override void _PhysicsProcess(double delta);
    public void ProcessMovement(PhysicsComponent physics);
}

// Ability usage
public partial class AbilitySystem : Node, ISystem
{
    private List<AbilityComponent> _abilityUsers;
    
    public void UseAbility(IEntity caster, int slot);
    public void ProcessCooldowns(double delta);
}

// Loot drops and collection
public partial class LootSystem : Node, ISystem
{
    public void SpawnLoot(Vector2 position, LootTable table);
    public void CollectLoot(IEntity collector, ItemEntity item);
}
```

---

## Layer 6: Assisting Utilities

Pure C# utility classes for algorithms and helpers.

### Utility Classes

```csharp
namespace Utility;

// Dungeon generation
public static class DungeonGenerator
{
    public static DungeonLayout GenerateFloor(DungeonData data, int floor);
    public static Room[] GenerateRooms(int count, Vector2I minSize, Vector2I maxSize);
    public static void ConnectRooms(Room[] rooms);
}

// Pathfinding
public static class PathfindingUtility
{
    public static Vector2[] FindPath(Vector2 start, Vector2 end, TileMap navMesh);
    public static Vector2 GetNextWaypoint(Vector2 current, Vector2[] path);
}

// Damage calculation
public static class DamageCalculator
{
    public static DamageInfo Calculate(CombatComponent attacker, CombatComponent defender);
    public static int ApplyResistances(int damage, DamageType type, int resistance);
    public static bool RollCritical(float critChance);
}

// Loot generation
public static class LootGenerator
{
    public static ItemInstance GenerateItem(LootTable table, int itemLevel);
    public static StatModifier[] RollModifiers(ItemRarity rarity);
}

// Random utilities
public static class RandomUtility
{
    public static T WeightedPick<T>(List<(T item, float weight)> options);
    public static int RollRange(int min, int max);
}
```

---

## Event System

### Event Categories

```csharp
namespace Event;

// Game state events
public record StateChangeEvent(GameState OldState, GameState NewState) : IEvent;
public record PauseEvent(bool IsPaused) : IEvent;

// Combat events
public record DamageEvent(IEntity Source, IEntity Target, DamageInfo Damage) : IEvent;
public record DeathEvent(IEntity Entity) : IEvent;
public record AbilityUsedEvent(IEntity Caster, AbilityData Ability) : IEvent;

// Progression events
public record ExperienceGainedEvent(int Amount, int Total) : IEvent;
public record LevelUpEvent(int NewLevel) : IEvent;
public record ItemPickupEvent(ItemInstance Item) : IEvent;

// Dungeon events
public record FloorChangeEvent(int Floor) : IEvent;
public record RoomEnteredEvent(Room Room) : IEvent;
public record BossDefeatedEvent(EnemyData Boss) : IEvent;

// Town events
public record ShopOpenedEvent(NPCEntity Vendor) : IEvent;
public record ItemPurchasedEvent(ItemData Item, int Cost) : IEvent;
public record QuestAcceptedEvent(QuestData Quest) : IEvent;

// Timer events (from Heart)
public record PulseTimeout : IEvent;
public record SlowPulseTimeout : IEvent;
public record CombatTickTimeout : IEvent;
```

---

## File Organization

```
src/
├── core/
│   ├── MainCore.cs
│   ├── CameraCore.cs
│   ├── ContextCore.cs
│   ├── EventCore.cs
│   ├── ServiceCore.cs
│   ├── StateCore.cs
│   ├── node/
│   │   ├── Heart.cs
│   │   └── Registry.cs
│   └── service/
│       ├── AudioService.cs
│       ├── SaveService.cs
│       ├── InputService.cs
│       ├── HeroService.cs
│       ├── InventoryService.cs
│       ├── QuestService.cs
│       └── interface/
│           ├── IAudioService.cs
│           ├── ISaveService.cs
│           └── ...
├── data/
│   ├── common/
│   │   ├── InfoData.cs
│   │   ├── StatsData.cs
│   │   └── AssetData.cs
│   ├── entity/
│   │   ├── HeroData.cs
│   │   ├── EnemyData.cs
│   │   └── ItemData.cs
│   ├── dungeon/
│   │   ├── DungeonData.cs
│   │   ├── RoomData.cs
│   │   └── LootTable.cs
│   ├── ability/
│   │   ├── AbilityData.cs
│   │   └── EffectData.cs
│   ├── enums/
│   │   ├── HeroEnums.cs
│   │   ├── ItemEnums.cs
│   │   └── AIEnums.cs
│   └── index/
│       ├── EntityIndex.cs
│       ├── HeroIndex.cs
│       └── ItemIndex.cs
├── component/
│   ├── HealthComponent.cs
│   ├── ManaComponent.cs
│   ├── PhysicsComponent.cs
│   ├── CombatComponent.cs
│   ├── AbilityComponent.cs
│   ├── AIComponent.cs
│   ├── InventoryComponent.cs
│   └── interface/
│       └── IComponent.cs
├── entity/
│   ├── HeroEntity.cs
│   ├── EnemyEntity.cs
│   ├── ItemEntity.cs
│   ├── NPCEntity.cs
│   └── interface/
│       ├── IEntity.cs
│       ├── IDamageable.cs
│       └── IInteractable.cs
├── manager/
│   ├── DungeonManager.cs
│   ├── TownManager.cs
│   ├── UiManager.cs
│   └── interface/
│       └── IManager.cs
├── system/
│   ├── CombatSystem.cs
│   ├── AISystem.cs
│   ├── PhysicsSystem.cs
│   ├── AbilitySystem.cs
│   ├── LootSystem.cs
│   ├── SpawnSystem.cs
│   ├── DialogueSystem.cs
│   ├── CraftSystem.cs
│   ├── ShopSystem.cs
│   ├── QuestSystem.cs
│   └── interface/
│       └── ISystem.cs
├── event/
│   ├── GameEvents.cs
│   ├── CombatEvents.cs
│   ├── ProgressionEvents.cs
│   ├── DungeonEvents.cs
│   ├── TownEvents.cs
│   └── interface/
│       └── IEvent.cs
└── utility/
    ├── DungeonGenerator.cs
    ├── PathfindingUtility.cs
    ├── DamageCalculator.cs
    ├── LootGenerator.cs
    └── RandomUtility.cs

scene/
├── Main.tscn
├── dungeon/
│   ├── DungeonScene.tscn
│   └── rooms/
├── town/
│   ├── TownScene.tscn
│   └── buildings/
├── templates/
│   ├── HeroEntity.tscn
│   ├── EnemyEntity.tscn
│   ├── ItemEntity.tscn
│   └── NPCEntity.tscn
└── ui/
    ├── HUD.tscn
    ├── Inventory.tscn
    └── Menu.tscn

data/
├── heroes/
├── enemies/
├── items/
├── abilities/
├── dungeons/
├── quests/
└── indices/
```

---

## Performance Considerations

### Entity Pooling

```csharp
public class EntityPool<T> where T : Node, IEntity
{
    private Queue<T> _pool;
    private PackedScene _template;
    
    public T Spawn(IData data, Vector2 position);
    public void Despawn(T entity);
    public void PreWarm(int count);
}
```

### System Update Frequencies

| System | Update Rate | Trigger |
|--------|-------------|---------|
| PhysicsSystem | 60 Hz | `_PhysicsProcess` |
| CombatSystem | 10 Hz | CombatTickTimeout |
| AISystem | 20 Hz | PulseTimeout |
| AbilitySystem | 60 Hz | `_Process` |
| LootSystem | On-demand | Event-driven |

### Memory Management

- Use object pools for frequently spawned entities
- Share Resource data between entities
- Lazy-load dungeon floors
- Unload unused assets between modes

---

## Related Documents

- [00-game-design-document.md](00-game-design-document.md) - Game design overview
- [02-systems-design.md](02-systems-design.md) - Detailed system specifications
- [03-component-design.md](03-component-design.md) - Component specifications
- [04-data-structures.md](04-data-structures.md) - Data resource definitions
- [05-implementation-roadmap.md](05-implementation-roadmap.md) - Development phases
