# Architecture and Systems Design

## Overview

This document defines the technical architecture and system design for the RTS game, building upon the existing RedSrc codebase structure.

## Architectural Principles

### 1. Component-Based Design
- Use Godot's node system effectively
- Leverage C# for game logic
- Maintain separation of concerns
- Follow existing RedSrc patterns (Core, Data, Manager, Event)

### 2. Data-Driven Architecture
- Define units, buildings, and abilities in data files (.tres resources)
- Use ScriptableObjects equivalent (Godot Resources)
- Enable easy balancing and modding
- Minimize hardcoded values

### 3. Event-Driven Communication
- Utilize existing EventCore system
- Decouple systems through events
- Enable easy debugging and testing
- Support replay and networking

## System Architecture

### Core Systems Hierarchy

```
MainCore (Game Entry Point)
├── ServiceCore (Service Locator)
│   ├── GameStateService
│   ├── ResourceService
│   ├── UnitService
│   ├── BuildingService
│   ├── PathfindingService
│   ├── AIService
│   ├── AudioService (existing)
│   └── InputService
├── ContextCore (Game State Management)
│   ├── GameplayContext
│   ├── FactionContext
│   ├── PlayerContext
│   └── MapContext
├── EventCore (Event Bus - existing)
│   ├── GameplayEvents
│   ├── UnitEvents
│   ├── BuildingEvents
│   ├── CombatEvents
│   └── ResourceEvents
└── GameManager (existing - extended)
    ├── MatchManager
    ├── FactionManager
    └── ScenarioManager
```

## Major System Components

### 1. Unit System

#### Unit Manager
**Responsibilities:**
- Unit creation and destruction
- Unit state management
- Unit command queue processing
- Unit selection management

**Key Classes:**
```csharp
// Extends existing entity patterns
public partial class RTSUnit : Node2D, ISelectable, ICommandable
{
    public UnitData Data { get; set; }
    public Faction Owner { get; set; }
    public UnitStateMachine StateMachine { get; set; }
    public CommandQueue Commands { get; set; }
}

public class UnitData : Resource
{
    // Stats
    public int MaxHealth { get; set; }
    public float MovementSpeed { get; set; }
    public int AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackSpeed { get; set; }
    
    // Costs
    public ResourceCost BuildCost { get; set; }
    public float BuildTime { get; set; }
    
    // Behavior
    public UnitType Type { get; set; }
    public ArmorType Armor { get; set; }
    public DamageType Damage { get; set; }
}
```

#### Unit States
- Idle
- Moving
- Attacking
- Gathering
- Building
- Dying

### 2. Building System

#### Building Manager
**Responsibilities:**
- Building placement validation
- Construction progress tracking
- Building functionality (unit production, research, etc.)
- Building destruction

**Key Classes:**
```csharp
public partial class RTSBuilding : StaticBody2D, ISelectable, IProductionCapable
{
    public BuildingData Data { get; set; }
    public Faction Owner { get; set; }
    public ProductionQueue ProductionQueue { get; set; }
    public bool IsConstructed { get; set; }
    public float ConstructionProgress { get; set; }
}

public class BuildingData : Resource
{
    // Stats
    public int MaxHealth { get; set; }
    public Vector2I GridSize { get; set; }
    public int Armor { get; set; }
    
    // Costs
    public ResourceCost BuildCost { get; set; }
    public float BuildTime { get; set; }
    
    // Functionality
    public BuildingType Type { get; set; }
    public List<UnitData> ProducibleUnits { get; set; }
    public List<UpgradeData> AvailableUpgrades { get; set; }
}
```

### 3. Resource System

#### Resource Manager
**Responsibilities:**
- Track faction resources
- Validate resource transactions
- Handle resource gathering
- Manage resource deposits

**Key Classes:**
```csharp
public class ResourceManager
{
    public Dictionary<Faction, ResourcePool> FactionResources { get; set; }
    
    public bool TrySpendResources(Faction faction, ResourceCost cost);
    public void AddResources(Faction faction, ResourceAmount amount);
    public ResourceAmount GetResources(Faction faction);
}

public class ResourcePool
{
    public int Gold { get; set; }
    public int Wood { get; set; }
    public int CurrentSupply { get; set; }
    public int MaxSupply { get; set; }
}

public class ResourceDeposit : Area2D
{
    public ResourceType Type { get; set; }
    public int RemainingAmount { get; set; }
    public List<RTSUnit> CurrentGatherers { get; set; }
}
```

### 4. Pathfinding System

#### Navigation Service
**Responsibilities:**
- Calculate unit paths
- Handle dynamic obstacles
- Group movement coordination
- Terrain cost evaluation

**Implementation:**
```csharp
public class PathfindingService : IService
{
    private Navigation2D _navigation;
    private AStarGrid2D _astarGrid;
    
    public NavigationPath FindPath(Vector2 from, Vector2 to, UnitData unit);
    public void UpdateObstacle(Vector2I gridPos, bool passable);
    public List<Vector2> GetGroupFormation(List<RTSUnit> units, Vector2 target);
}
```

**Considerations:**
- Use Godot's built-in Navigation2D system
- Implement flow fields for large groups
- Cache frequently requested paths
- Update grid dynamically as buildings are placed/destroyed

### 5. Combat System

#### Combat Manager
**Responsibilities:**
- Damage calculation
- Attack range validation
- Target acquisition
- Armor/damage type interactions

**Key Classes:**
```csharp
public class CombatSystem
{
    public void ProcessAttack(RTSUnit attacker, RTSUnit target);
    public float CalculateDamage(DamageType damage, ArmorType armor, int baseDamage);
    public RTSUnit FindBestTarget(RTSUnit attacker, float range);
    public void ApplyDamage(RTSUnit target, float damage);
}

public enum DamageType { Normal, Pierce, Siege, Magic }
public enum ArmorType { Unarmored, Light, Medium, Heavy, Fortified }
```

#### Damage Modifiers
```
              | Unarmored | Light | Medium | Heavy | Fortified
-----------------------------------------------------------------
Normal        |   100%    | 100%  |  100%  |  100% |    75%
Pierce        |   150%    | 100%  |   75%  |   50% |    35%
Siege         |   100%    |  75%  |  100%  |  100% |   150%
Magic         |   100%    | 125%  |  100%  |  100% |    50%
```

### 6. Selection and Command System

#### Selection Manager
**Responsibilities:**
- Handle mouse/touch input for selection
- Maintain selected unit groups
- Process selection box drawing
- Double-click to select unit type

**Key Classes:**
```csharp
public class SelectionManager
{
    public List<ISelectable> CurrentSelection { get; private set; }
    public SelectionGroup[] ControlGroups { get; private set; } // 0-9
    
    public void SelectUnits(List<ISelectable> units);
    public void AddToSelection(ISelectable unit);
    public void RemoveFromSelection(ISelectable unit);
    public void AssignControlGroup(int number);
    public void RecallControlGroup(int number);
}

public interface ISelectable
{
    void Select();
    void Deselect();
    bool IsSelected { get; set; }
    Rect2 SelectionBounds { get; }
}
```

#### Command System
```csharp
public class CommandProcessor
{
    public void IssueCommand(List<ISelectable> units, ICommand command);
    public void QueueCommand(List<ISelectable> units, ICommand command);
}

public interface ICommand
{
    void Execute(RTSUnit unit);
    bool IsValid(RTSUnit unit);
    CommandType Type { get; }
}

public enum CommandType 
{ 
    Move, 
    Attack, 
    Patrol, 
    Hold, 
    Gather, 
    Build, 
    Stop 
}
```

### 7. Fog of War System

#### Vision Manager
**Responsibilities:**
- Track unit/building vision
- Update fog of war visibility
- Handle shared vision in team games
- Optimize visibility calculations

**Implementation:**
```csharp
public class FogOfWarManager
{
    private TileMap _fogTileMap;
    private Dictionary<Vector2I, VisibilityInfo> _visibilityGrid;
    
    public void UpdateVision(RTSUnit unit);
    public void UpdateVision(RTSBuilding building);
    public bool IsVisible(Vector2 worldPos, Faction viewer);
    public bool IsExplored(Vector2 worldPos, Faction viewer);
}

public enum VisibilityState { Hidden, Explored, Visible }
```

### 8. AI System

#### AI Service
**Responsibilities:**
- Control AI players
- Make strategic decisions
- Manage AI economy
- Control AI military

**Key Components:**
```csharp
public class AIController
{
    public AIPersonality Personality { get; set; }
    public EconomyAI Economy { get; set; }
    public MilitaryAI Military { get; set; }
    public BuildOrderAI BuildOrder { get; set; }
    
    public void Update(float delta);
}

public enum AIPersonality { Aggressive, Defensive, Economic, Balanced }
```

## Data Layer Architecture

### Resource-Based Data

Following Godot's resource system and existing RedSrc patterns:

```
/data/
  /rts/
    /units/
      ├── UnitIndex.tres (inherits from Index system)
      ├── Worker.tres
      ├── Soldier.tres
      └── Archer.tres
    /buildings/
      ├── BuildingIndex.tres
      ├── TownHall.tres
      ├── Barracks.tres
      └── Farm.tres
    /factions/
      ├── FactionIndex.tres
      ├── HumanFaction.tres
      └── OrcFaction.tres
    /abilities/
      └── [ability definitions]
    /upgrades/
      └── [upgrade definitions]
```

### Data Classes Structure

```csharp
// In /src/data/rts/
public class UnitData : Resource { }
public class BuildingData : Resource { }
public class FactionData : Resource { }
public class AbilityData : Resource { }
public class UpgradeData : Resource { }

// Indices (following existing pattern)
public class RTSUnitIndex : Resource { }
public class RTSBuildingIndex : Resource { }
```

## Scene Organization

### Scene Structure

```
/scene/
  /rts/
    ├── RTSMain.tscn (main game scene)
    ├── RTSMatch.tscn (match container)
    ├── /ui/
    │   ├── HUD.tscn
    │   ├── BuildMenu.tscn
    │   ├── Minimap.tscn
    │   └── UnitPanel.tscn
    ├── /templates/
    │   ├── RTSUnit.tscn
    │   ├── RTSBuilding.tscn
    │   ├── Projectile.tscn
    │   └── ResourceDeposit.tscn
    ├── /maps/
    │   ├── Map01.tscn
    │   └── TestMap.tscn
    └── /effects/
        ├── AttackEffect.tscn
        └── BuildingEffect.tscn
```

## Performance Considerations

### Optimization Strategies

1. **Object Pooling**
   - Reuse projectile instances
   - Pool common effects
   - Manage unit instances efficiently

2. **Spatial Partitioning**
   - Use Godot's built-in quadtree
   - Optimize collision detection
   - Cull off-screen calculations

3. **Level of Detail**
   - Reduce animation frames for distant units
   - Skip AI updates for far units
   - Throttle pathfinding calculations

4. **Batch Operations**
   - Group similar rendering calls
   - Batch resource updates
   - Aggregate event notifications

## Networking Considerations (Future)

### Architecture for Multiplayer

1. **Deterministic Simulation**
   - Lockstep networking model
   - Command-based synchronization
   - Fixed timestep simulation

2. **State Synchronization**
   - Periodic checksum validation
   - Out-of-sync detection
   - Reconnection support

3. **Client-Server vs P2P**
   - Initial recommendation: Peer-to-peer
   - Fallback: Relay server
   - Future: Dedicated servers

## Testing Strategy

### Unit Testing
- Test unit stats and calculations
- Test resource transactions
- Test pathfinding algorithms
- Test combat formulas

### Integration Testing
- Test system interactions
- Test game flow
- Test save/load functionality

### Performance Testing
- Stress test with max units
- Profile bottlenecks
- Optimize hot paths

## Migration from Existing RedSrc

### Reusable Components
- EventCore system ✓
- ServiceCore pattern ✓
- ContextCore for state ✓
- Data resource structure ✓
- Index system ✓

### New Components Required
- Unit command system
- Building placement system
- Pathfinding service
- Combat system
- Resource gathering
- AI controllers
- RTS-specific UI

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
