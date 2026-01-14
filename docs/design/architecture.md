# Technical Architecture

> **Version**: 0.1.0  
> **Last Updated**: 2026-01-14

## Overview

RedSrc employs a **data-driven, ECS-inspired architecture** that leverages Godot's scene-based workflow while maximizing C#'s capabilities for modularity, code reuse, and performance. This is not a full Entity-Component-System implementation but rather an adaptation that fits Godot's paradigms.

## Design Principles

### Data-Oriented Design
- **Components** are data containers with minimal logic
- **Systems** operate on component groups for batch processing
- **Resources** define stateless, shareable data assets

### Separation of Concerns
- Each layer has specific responsibilities
- Inter-layer communication follows strict patterns
- Dependencies flow downward through layers

### Dependency Injection
- `ServiceCore.Registry` provides service location
- No static coupling between systems
- Manual injection during initialization (Godot doesn't support constructor injection for Nodes)

## Layered Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│ Layer 6: Utilities                                               │
│ (Helper classes, algorithms, math utilities)                     │
├─────────────────────────────────────────────────────────────────┤
│ Layer 5: Systems                                                 │
│ (Logic controllers: AISystem, PhysicsSystem, CombatSystem, etc.) │
├─────────────────────────────────────────────────────────────────┤
│ Layer 4: Managers                                                │
│ (GameManager, MenuManager - root scene orchestrators)            │
├─────────────────────────────────────────────────────────────────┤
│ Layer 3: Entities                                                │
│ (Pre-configured scenes: Mobs, Heroes, Items, Levels)             │
├─────────────────────────────────────────────────────────────────┤
│ Layer 2B: Components                                             │
│ (Behavior nodes: HealthComponent, PhysicsComponent, etc.)        │
├─────────────────────────────────────────────────────────────────┤
│ Layer 2A: Data                                                   │
│ (Resources: HeroData, MobData, ItemData, etc.)                   │
├─────────────────────────────────────────────────────────────────┤
│ Layer 1B: Services                                               │
│ (AudioService, SaveService, InputService, etc.)                  │
├─────────────────────────────────────────────────────────────────┤
│ Layer 1A: Core Infrastructure                                    │
│ (MainCore, EventCore, ServiceCore, StateCore, CameraCore)        │
└─────────────────────────────────────────────────────────────────┘
```

## Layer Details

### Layer 1A: Core Infrastructure (`Core` namespace)

Singleton nodes attached to or being `MainCore`. These provide fundamental services to all other layers.

| Component | Responsibility |
|-----------|---------------|
| **MainCore** | Root node, initialization, global orchestration |
| **CameraCore** | Main camera management and behavior |
| **EventCore** | Global event bus with pub/sub messaging |
| **ServiceCore** | Service registration and dependency resolution |
| **StateCore** | Game state management and transitions |
| **ContextCore** | Global game data references and context |

#### EventCore Subsystem

EventCore manages the **Heart** timing system:

```csharp
// Timer intervals (configurable)
PulseInterval = 0.05f        // 20 Hz - high frequency updates
SlowPulseInterval = 0.2f     // 5 Hz - lower frequency updates  
MobSpawnInterval = 5f        // Mob wave spawning
ChestSpawnInterval = 10f     // Loot container spawning
GameInterval = 60f           // Per-minute game events
```

#### ServiceCore Registry

```csharp
// Service registration pattern
ServiceRegistry.Register<IAudioService, AudioService>();
ServiceRegistry.Register<IHeroService, HeroService>();

// Service resolution
var audio = ServiceCore.AudioService();
```

### Layer 1B: Primary Services (`Core.Service` namespace)

Plain C# classes (non-Nodes) providing core functionality. Registered with `ServiceCore.Registry`.

| Service | Responsibility |
|---------|---------------|
| **AudioService** | Audio playback, music, SFX management |
| **ContextService** | Global game data, loaded level references |
| **InputService** | Player input mapping and handling |
| **IndexService** | Entity and data lookup management |
| **SaveService** | Save/load game state persistence |
| **SceneService** | Scene transitions and loading |
| **SettingsService** | User preferences and settings |
| **StateService** | Game state transitions |
| **HeroService** | Currently loaded hero/prospector data |
| **LevelService** | Currently loaded level data |

### Layer 2A: Data Abstractions (`Data` namespace)

Godot `Resource` classes for stateless, shareable data. Created as `.tres` files in the editor.

#### Common Data Resources
```csharp
InfoData     // Name, description, lore
AssetData    // Sprites, sounds, shapes, colors
StatsData    // Health, damage, speed
Metadata     // Icon, rarity, unlock state
```

#### Entity-Specific Data
```csharp
HeroData       // Prospector definitions
MobData        // Enemy definitions
ItemData       // Item properties
WeaponData     // Weapon configurations
LevelData      // Level layouts and properties
ChestData      // Loot container definitions
EffectData     // Status effects, buffs
CraftData      // Crafting recipes
QuestData      // Quest objectives and rewards
```

### Layer 2B: Behavior Components (`Component` namespace)

Specialized Godot nodes that act as **data containers**. They expose properties via `[Export]` for editor configuration.

#### Key Principles
- **No `_Process`/`_PhysicsProcess`**: Logic handled by Systems
- **Self-validation**: `_GetConfigurationWarnings()` for editor feedback
- **System registration**: Components register with relevant systems on instantiation

#### Planned Components
```csharp
HealthComponent       // Health pool data
PhysicsComponent      // Movement properties (CharacterBody2D)
InventoryComponent    // Inventory slots and items
RenderComponent       // Visual representation
InteractionComponent  // Interaction triggers
AIComponent           // AI state and steering data
CombatComponent       // Attack data and cooldowns
BuffComponent         // Active status effects
```

### Layer 3: Entities (`Entity` namespace)

Pre-configured Godot scenes (`.tscn`) composed of components. Instantiated by `FactorySystem`.

#### Scene Inheritance Pattern
```
BaseMob.tscn (template)
├── Physics (PhysicsComponent)
├── Health (HealthComponent)
├── Render (RenderComponent)
├── Hitbox (HitboxComponent)
└── AI (AIComponent)

Goblin.tscn (inherits BaseMob.tscn)
├── Physics: Speed = 50
├── Health: MaxHealth = 100
└── Custom properties...
```

#### Godot Groups
| Entity Type | Group Name |
|-------------|------------|
| GameEntity | `"Game"` |
| MenuEntity | `"Menu"` |
| MobEntity | `"mobs"` |
| HeroEntity | `"player"` |
| ChestEntity | `"chests"` |
| LevelEntity | `"level"` |

### Layer 4: Orchestration Managers (`Manager` namespace)

Root-node scenes controlling game modes. Switch via `StateCore`.

#### GameManager
- Controls **extraction/gameplay** state
- Initializes game systems (Physics, Combat, AI, Spawn, etc.)
- Manages level loading and entity spawning
- Handles game-loop timing

#### MenuManager
- Controls **downtime/menu** state
- Manages UI systems and Fantasy OS interface
- Handles crafting, inventory, narrative systems
- Controls idle/passive progression

### Layer 5: Logic Systems (`System` namespace)

Nodes that process component groups. Owned and updated by their parent Manager.

#### Update Patterns
```csharp
// High-frequency (every frame or physics frame)
PhysicsSystem    // Movement via _PhysicsProcess
RenderSystem     // Visual updates via _Process

// Throttled (timer-based)
AISystem         // Decision-making at 5-10 Hz
SpawnSystem      // Entity spawning at configured intervals
```

#### Planned Systems
| System | Purpose |
|--------|---------|
| **PhysicsSystem** | Movement, collision handling |
| **AISystem** | Enemy behavior, targeting |
| **CombatSystem** | Damage, combat interactions |
| **FactorySystem** | Entity instantiation, object pooling |
| **SpawnSystem** | Wave spawning, difficulty scaling |
| **LootSystem** | Loot drops, auto-collection |
| **InventorySystem** | Item management |
| **XPSystem** | Experience, leveling |
| **MapSystem** | Map generation, boundaries |
| **UISystem** | HUD, menus |
| **CraftSystem** | Crafting recipes |
| **DialogueSystem** | NPC conversations |
| **QuestSystem** | Quest tracking, completion |

### Layer 6: Utilities (`Utility` namespace)

Plain C# helper classes for common operations.

```csharp
MathUtility      // Vector math, calculations
RandomUtility    // Seeded random generation
DataUtility      // Resource parsing, loading
TimerUtility     // Custom timing helpers
PoolUtility      // Object pooling utilities
```

## Communication Patterns

### Event-Based Communication

```csharp
// Subscribe to events
EventCore.Subscribe<MobSpawnTimeout>(OnMobSpawn);
EventCore.Subscribe<StateEvent>(OnStateChange);

// Publish events
EventCore.Publish<PulseTimeout>();
EventCore.Publish<DamageEvent>(new DamageEvent(target, amount));
```

### System-to-Component Flow

```
System → reads → Component.Data
System → writes → Component.State
System → calls → Component.ApplyChange()
```

### Inter-Component Communication

Components **never** communicate directly. Systems mediate:

```csharp
// AISystem sets target
aiComponent.TargetDirection = directionToPlayer;

// PhysicsSystem reads target
var direction = entity.AIComponent.TargetDirection;
physicsComponent.Velocity = direction * speed;
```

## Conventions

### Naming
- Folders: `snake_case` (e.g., `core_service`)
- Classes: `PascalCase` with role suffix (e.g., `MainCore`, `HeroData`, `PhysicsComponent`)
- Interfaces: `I` prefix, in `/interface/` folders (e.g., `IComponent`, `IService`)

### Code Organization
```
src/
├── core/
│   ├── MainCore.cs
│   ├── EventCore.cs
│   ├── ServiceCore.cs
│   ├── node/
│   │   ├── Heart.cs
│   │   └── Registry.cs
│   └── service/
│       ├── AudioService.cs
│       └── interface/
│           └── IAudioService.cs
├── component/
├── data/
├── entity/
├── manager/
├── system/
└── utility/
```

### Resource Organization
```
data/
├── heros/           # HeroData .tres files
├── mobs/            # MobData .tres files
├── levels/          # LevelData .tres files
├── items/           # ItemData .tres files
├── effects/         # EffectData .tres files
├── indices/         # Index .tres files
└── shaders/         # Visual shaders
```

## Performance Considerations

### Entity Scaling
Target: **5,000+ active entities** with stable frame rate

### Optimization Strategies
1. **Centralized system loops** instead of per-entity `_Process`
2. **Object pooling** for frequently spawned entities
3. **Spatial partitioning** for collision/proximity checks
4. **Throttled updates** for non-critical systems
5. **Component data locality** for cache efficiency

### Frame Budget
- PhysicsSystem: Every physics frame (60 Hz)
- RenderSystem: Every frame (60 FPS target)
- AISystem: 5-10 Hz throttled
- SpawnSystem: Configurable intervals

---

*See [Core Systems](core-systems.md) for implementation details of each architectural component.*
