# Technical Implementation Plan

## Overview

This document provides a detailed technical roadmap for implementing the RTS game systems, building upon the existing RedSrc codebase.

## Phase 1: Foundation (Weeks 1-3)

### Week 1: Core Infrastructure Setup

#### Task 1.1: Project Structure Reorganization
```bash
/src/
  /rts/              # New RTS-specific code
    /core/
    /data/
    /entities/
    /systems/
    /ui/
  /core/             # Keep existing core systems
  /data/             # Keep existing data systems
```

**Implementation:**
```csharp
// Create base RTS namespace structure
namespace Blobs.RTS
{
    namespace Core { }
    namespace Entities { }
    namespace Systems { }
    namespace UI { }
    namespace Data { }
}
```

**Files to Create:**
- `src/rts/core/RTSGameCore.cs` - Main RTS game coordinator
- `src/rts/core/RTSConstants.cs` - Game constants
- `src/rts/data/RTSEnums.cs` - Enumerations

**Checklist:**
- [ ] Create directory structure
- [ ] Set up namespace conventions
- [ ] Create base classes/interfaces
- [ ] Update .csproj if needed
- [ ] Verify compilation

#### Task 1.2: Data Layer Foundation

**Create Resource Classes:**
```csharp
// src/rts/data/UnitData.cs
public partial class UnitData : Resource
{
    [Export] public string UnitName { get; set; }
    [Export] public int MaxHealth { get; set; }
    [Export] public int Armor { get; set; }
    [Export] public ArmorType ArmorType { get; set; }
    [Export] public int AttackDamage { get; set; }
    [Export] public DamageType DamageType { get; set; }
    [Export] public float AttackSpeed { get; set; }
    [Export] public float AttackRange { get; set; }
    [Export] public float MovementSpeed { get; set; }
    [Export] public int VisionRange { get; set; }
    [Export] public ResourceCost Cost { get; set; }
    [Export] public float BuildTime { get; set; }
    [Export] public int SupplyCost { get; set; }
}
```

**Files to Create:**
- `src/rts/data/UnitData.cs`
- `src/rts/data/BuildingData.cs`
- `src/rts/data/FactionData.cs`
- `src/rts/data/ResourceCost.cs`
- `src/rts/data/RTSUnitIndex.cs`
- `src/rts/data/RTSBuildingIndex.cs`

**Checklist:**
- [ ] Create all data classes
- [ ] Add proper Export attributes
- [ ] Create sample .tres resources
- [ ] Test loading in Godot editor
- [ ] Create index resources

#### Task 1.3: Basic Scene Setup

**Create Scene Templates:**
- `scene/rts/RTSMain.tscn` - Main RTS scene
- `scene/rts/templates/RTSUnit.tscn` - Unit template
- `scene/rts/templates/RTSBuilding.tscn` - Building template

**RTSUnit.tscn Structure:**
```
RTSUnit (CharacterBody2D)
├── Sprite2D (unit sprite)
├── CollisionShape2D
├── SelectionIndicator (Node2D)
│   └── Sprite2D (selection circle)
├── HealthBar (Control)
└── NavigationAgent2D
```

**Checklist:**
- [ ] Create scene templates
- [ ] Set up basic node hierarchies
- [ ] Configure collision layers
- [ ] Test instantiation
- [ ] Add placeholder sprites

### Week 2: Core Entity System

#### Task 2.1: Implement RTSUnit Entity

**Create RTSUnit.cs:**
```csharp
public partial class RTSUnit : CharacterBody2D, ISelectable, ICommandable
{
    [Export] public UnitData Data { get; set; }
    
    public Faction Owner { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsSelected { get; set; }
    
    private StateMachine _stateMachine;
    private NavigationAgent2D _navAgent;
    private Queue<ICommand> _commandQueue;
    
    public override void _Ready()
    {
        CurrentHealth = Data.MaxHealth;
        _navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _stateMachine = new StateMachine();
        InitializeStates();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        _stateMachine.Update((float)delta);
    }
    
    public void Select() { IsSelected = true; }
    public void Deselect() { IsSelected = false; }
    
    public void IssueCommand(ICommand command)
    {
        _commandQueue.Clear();
        _commandQueue.Enqueue(command);
    }
}
```

**Checklist:**
- [ ] Implement RTSUnit class
- [ ] Add selection logic
- [ ] Implement health system
- [ ] Add basic movement
- [ ] Test in-game

#### Task 2.2: Implement State Machine

**Create Unit States:**
```csharp
// src/rts/systems/states/UnitStateMachine.cs
public class UnitStateMachine : StateMachine
{
    public IdleState Idle { get; }
    public MoveState Move { get; }
    public AttackState Attack { get; }
    
    public UnitStateMachine(RTSUnit unit)
    {
        Idle = new IdleState(unit);
        Move = new MoveState(unit);
        Attack = new AttackState(unit);
        
        SetState(Idle);
    }
}

// src/rts/systems/states/IdleState.cs
public class IdleState : State
{
    private RTSUnit _unit;
    
    public override void Enter() { }
    public override void Update(float delta) 
    {
        // Idle behavior
    }
    public override void Exit() { }
}
```

**States to Implement:**
- [ ] IdleState
- [ ] MoveState
- [ ] AttackState
- [ ] GatherState (for workers)
- [ ] BuildState (for workers)

#### Task 2.3: Implement RTSBuilding Entity

**Create RTSBuilding.cs:**
```csharp
public partial class RTSBuilding : StaticBody2D, ISelectable, IProductionCapable
{
    [Export] public BuildingData Data { get; set; }
    
    public Faction Owner { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsConstructed { get; set; }
    public float ConstructionProgress { get; set; }
    public ProductionQueue ProductionQueue { get; set; }
    public Vector2 RallyPoint { get; set; }
    
    public override void _Ready()
    {
        CurrentHealth = 1; // Start with minimal HP during construction
        ProductionQueue = new ProductionQueue(5); // Max 5 queue
    }
    
    public override void _Process(double delta)
    {
        if (!IsConstructed)
        {
            UpdateConstruction((float)delta);
        }
        else
        {
            ProductionQueue.Update((float)delta);
        }
    }
}
```

**Checklist:**
- [ ] Implement RTSBuilding class
- [ ] Add construction system
- [ ] Implement production queue
- [ ] Add rally point functionality
- [ ] Test building placement

### Week 3: Selection and Command System

#### Task 3.1: Selection Manager

**Create SelectionManager.cs:**
```csharp
public partial class SelectionManager : Node
{
    private List<ISelectable> _currentSelection = new();
    private SelectionGroup[] _controlGroups = new SelectionGroup[10];
    
    public void SelectUnits(List<ISelectable> units)
    {
        ClearSelection();
        foreach (var unit in units)
        {
            unit.Select();
            _currentSelection.Add(unit);
        }
        EmitSelectionChanged();
    }
    
    public void BoxSelect(Rect2 selectionBox)
    {
        var units = GetUnitsInBox(selectionBox);
        SelectUnits(units);
    }
    
    public void AssignControlGroup(int groupNumber)
    {
        _controlGroups[groupNumber] = new SelectionGroup(_currentSelection);
    }
    
    public void RecallControlGroup(int groupNumber)
    {
        if (_controlGroups[groupNumber] != null)
        {
            SelectUnits(_controlGroups[groupNumber].Units);
        }
    }
}
```

**Checklist:**
- [ ] Implement selection manager
- [ ] Add box selection drawing
- [ ] Implement control groups
- [ ] Add selection highlighting
- [ ] Test with mouse input

#### Task 3.2: Command System

**Create Command Classes:**
```csharp
// src/rts/systems/commands/ICommand.cs
public interface ICommand
{
    CommandType Type { get; }
    void Execute(RTSUnit unit);
    bool IsValid(RTSUnit unit);
}

// src/rts/systems/commands/MoveCommand.cs
public class MoveCommand : ICommand
{
    public CommandType Type => CommandType.Move;
    public Vector2 Destination { get; set; }
    
    public void Execute(RTSUnit unit)
    {
        unit.MoveTo(Destination);
    }
    
    public bool IsValid(RTSUnit unit) => true;
}
```

**Commands to Implement:**
- [ ] MoveCommand
- [ ] AttackCommand
- [ ] AttackMoveCommand
- [ ] GatherCommand
- [ ] BuildCommand
- [ ] StopCommand
- [ ] PatrolCommand

**Checklist:**
- [ ] Create command interface
- [ ] Implement all command types
- [ ] Add command queue system
- [ ] Integrate with input system
- [ ] Test command execution

## Phase 2: Core Gameplay (Weeks 4-6)

### Week 4: Resource System

#### Task 4.1: Resource Manager

**Implementation:**
```csharp
public class ResourceManager : Node
{
    private Dictionary<Faction, ResourcePool> _factionResources = new();
    
    public bool TrySpendResources(Faction faction, ResourceCost cost)
    {
        var pool = _factionResources[faction];
        if (pool.Gold >= cost.Gold && pool.Wood >= cost.Wood)
        {
            pool.Gold -= cost.Gold;
            pool.Wood -= cost.Wood;
            EmitResourceChanged(faction);
            return true;
        }
        return false;
    }
    
    public void AddResources(Faction faction, ResourceType type, int amount)
    {
        _factionResources[faction].Add(type, amount);
        EmitResourceChanged(faction);
    }
}
```

**Checklist:**
- [ ] Implement ResourceManager
- [ ] Create ResourcePool class
- [ ] Add resource deposits
- [ ] Implement resource validation
- [ ] Test resource transactions

#### Task 4.2: Resource Gathering

**Worker Gathering State:**
```csharp
public class GatherState : State
{
    private RTSUnit _worker;
    private ResourceDeposit _targetResource;
    private RTSBuilding _nearestDepot;
    private bool _isCarrying = false;
    
    public override void Update(float delta)
    {
        if (!_isCarrying)
        {
            if (_worker.IsNear(_targetResource))
            {
                GatherResource();
            }
            else
            {
                _worker.MoveTo(_targetResource.Position);
            }
        }
        else
        {
            if (_worker.IsNear(_nearestDepot))
            {
                DepositResources();
            }
            else
            {
                _worker.MoveTo(_nearestDepot.Position);
            }
        }
    }
}
```

**Checklist:**
- [ ] Implement gathering state
- [ ] Create ResourceDeposit nodes
- [ ] Add carrying visual indicator
- [ ] Auto-path to depot
- [ ] Test full gather cycle

#### Task 4.3: Building Construction

**Construction System:**
```csharp
public partial class RTSBuilding : StaticBody2D
{
    private List<RTSUnit> _builders = new();
    
    private void UpdateConstruction(float delta)
    {
        if (_builders.Count == 0) return;
        
        // Multi-builder bonus
        float buildSpeed = CalculateBuildSpeed(_builders.Count);
        ConstructionProgress += buildSpeed * delta;
        
        if (ConstructionProgress >= Data.BuildTime)
        {
            CompleteConstruction();
        }
        
        // Update health during construction
        CurrentHealth = (int)(Data.MaxHealth * (ConstructionProgress / Data.BuildTime));
    }
    
    private float CalculateBuildSpeed(int builderCount)
    {
        return builderCount switch
        {
            1 => 1.0f,
            2 => 1.75f,
            3 => 2.25f,
            _ => 2.5f // Cap at 4+ builders
        };
    }
}
```

**Checklist:**
- [ ] Implement construction system
- [ ] Add multi-worker building
- [ ] Visual construction progress
- [ ] Worker auto-build behavior
- [ ] Test building completion

### Week 5: Combat System

#### Task 5.1: Combat Manager

**Implementation:**
```csharp
public class CombatSystem : Node
{
    private static readonly float[,] _damageModifiers = {
        // [DamageType, ArmorType] = multiplier
        {1.0f, 1.0f, 1.0f, 1.0f, 0.75f}, // Normal
        {1.5f, 1.0f, 0.75f, 0.5f, 0.35f}, // Pierce
        {1.0f, 0.75f, 1.0f, 1.0f, 1.5f},  // Siege
        {1.0f, 1.25f, 1.0f, 1.0f, 0.5f}   // Magic
    };
    
    public void ProcessAttack(RTSUnit attacker, RTSUnit target)
    {
        float baseDamage = attacker.Data.AttackDamage;
        float modifier = _damageModifiers[
            (int)attacker.Data.DamageType, 
            (int)target.Data.ArmorType
        ];
        float armorReduction = target.Data.Armor / (target.Data.Armor + 100f);
        
        float finalDamage = baseDamage * modifier * (1 - armorReduction);
        
        ApplyDamage(target, finalDamage);
    }
    
    private void ApplyDamage(RTSUnit target, float damage)
    {
        target.CurrentHealth -= (int)damage;
        
        if (target.CurrentHealth <= 0)
        {
            target.Die();
        }
    }
}
```

**Checklist:**
- [ ] Implement CombatSystem
- [ ] Add damage calculation
- [ ] Implement armor types
- [ ] Add death handling
- [ ] Test various matchups

#### Task 5.2: Attack State & Targeting

**Attack State:**
```csharp
public class AttackState : State
{
    private RTSUnit _unit;
    private RTSUnit _target;
    private float _attackCooldown = 0f;
    
    public override void Update(float delta)
    {
        if (_target == null || _target.CurrentHealth <= 0)
        {
            AcquireNewTarget();
            if (_target == null)
            {
                _stateMachine.ChangeState(_stateMachine.Idle);
                return;
            }
        }
        
        float distance = _unit.Position.DistanceTo(_target.Position);
        
        if (distance > _unit.Data.AttackRange)
        {
            _unit.MoveTo(_target.Position);
        }
        else
        {
            _unit.StopMoving();
            FaceTarget();
            
            _attackCooldown -= delta;
            if (_attackCooldown <= 0)
            {
                PerformAttack();
                _attackCooldown = _unit.Data.AttackSpeed;
            }
        }
    }
}
```

**Checklist:**
- [ ] Implement attack state
- [ ] Add target acquisition
- [ ] Implement attack cooldown
- [ ] Add unit facing
- [ ] Test melee combat

#### Task 5.3: Projectile System

**Projectile Implementation:**
```csharp
public partial class Projectile : Area2D
{
    public RTSUnit Target { get; set; }
    public float Damage { get; set; }
    public float Speed { get; set; } = 10f;
    
    public override void _PhysicsProcess(double delta)
    {
        if (Target == null || !IsInstanceValid(Target))
        {
            QueueFree();
            return;
        }
        
        Vector2 direction = (Target.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;
        
        if (GlobalPosition.DistanceTo(Target.GlobalPosition) < 5f)
        {
            HitTarget();
            QueueFree();
        }
    }
}
```

**Checklist:**
- [ ] Create projectile scene
- [ ] Implement projectile script
- [ ] Add tracking behavior
- [ ] Handle target death mid-flight
- [ ] Test ranged combat

### Week 6: Pathfinding Integration

#### Task 6.1: Navigation Setup

**Map Navigation:**
```csharp
public class NavigationSetup : Node
{
    private NavigationRegion2D _navRegion;
    private NavigationPolygon _navPoly;
    
    public void Initialize(Vector2I mapSize)
    {
        _navPoly = new NavigationPolygon();
        
        // Create navigation mesh
        var outline = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(mapSize.X * 16, 0),
            new Vector2(mapSize.X * 16, mapSize.Y * 16),
            new Vector2(0, mapSize.Y * 16)
        };
        
        _navPoly.AddOutline(outline);
        _navPoly.MakePolygonsFromOutlines();
        
        _navRegion.NavigationPolygon = _navPoly;
    }
    
    public void AddObstacle(Vector2 position, Vector2 size)
    {
        // Carve out obstacle from navigation mesh
        // Godot handles this with NavigationObstacle2D nodes
    }
}
```

**Checklist:**
- [ ] Set up Navigation2D system
- [ ] Create navigation mesh
- [ ] Add obstacle handling
- [ ] Test pathfinding
- [ ] Optimize for performance

#### Task 6.2: Unit Navigation

**NavigationAgent Integration:**
```csharp
public partial class RTSUnit : CharacterBody2D
{
    private NavigationAgent2D _navAgent;
    
    public void MoveTo(Vector2 destination)
    {
        _navAgent.TargetPosition = destination;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (_navAgent.IsNavigationFinished())
            return;
        
        Vector2 nextPosition = _navAgent.GetNextPathPosition();
        Vector2 direction = (nextPosition - GlobalPosition).Normalized();
        
        Velocity = direction * Data.MovementSpeed * 16; // Convert to pixels
        MoveAndSlide();
    }
}
```

**Checklist:**
- [ ] Integrate NavigationAgent2D
- [ ] Handle path following
- [ ] Add collision avoidance
- [ ] Test group movement
- [ ] Optimize pathfinding calls

## Phase 3: UI and Polish (Weeks 7-9)

### Week 7: HUD Implementation

#### Task 7.1: Resource Display
- [ ] Create resource display UI
- [ ] Add gold/wood counters
- [ ] Add supply indicator
- [ ] Connect to ResourceManager
- [ ] Test real-time updates

#### Task 7.2: Minimap
- [ ] Create minimap viewport
- [ ] Add unit/building icons
- [ ] Implement camera control
- [ ] Add fog of war overlay
- [ ] Test minimap clicks

#### Task 7.3: Unit Info Panel
- [ ] Create selection panel
- [ ] Add unit portrait
- [ ] Display stats
- [ ] Show abilities
- [ ] Test multi-select

### Week 8: Command UI

#### Task 8.1: Command Buttons
- [ ] Create command grid
- [ ] Add button icons
- [ ] Implement hotkeys
- [ ] Add tooltips
- [ ] Test all commands

#### Task 8.2: Build Menu
- [ ] Create build menu UI
- [ ] Add building icons
- [ ] Show costs
- [ ] Implement tabs
- [ ] Test building placement

### Week 9: Menus and Polish

#### Task 9.1: Main Menu
- [ ] Design main menu
- [ ] Add buttons
- [ ] Implement navigation
- [ ] Add settings menu
- [ ] Test all flows

#### Task 9.2: In-Game Menus
- [ ] Create pause menu
- [ ] Add game over screen
- [ ] Victory/defeat screens
- [ ] Save/load functionality
- [ ] Test all scenarios

## Phase 4: AI and Content (Weeks 10-12)

### Week 10: Basic AI

#### Task 10.1: AI Controller
- [ ] Create AI controller class
- [ ] Implement update loop
- [ ] Add difficulty levels
- [ ] Basic decision making
- [ ] Test AI in match

#### Task 10.2: Economy AI
- [ ] Worker production
- [ ] Resource gathering
- [ ] Worker distribution
- [ ] Supply management
- [ ] Test economy scaling

### Week 11: Military AI

#### Task 11.1: Army Building
- [ ] Unit production logic
- [ ] Army composition
- [ ] Tech progression
- [ ] Test army growth

#### Task 11.2: Combat AI
- [ ] Attack timing
- [ ] Target selection
- [ ] Unit micro
- [ ] Retreat logic
- [ ] Test in combat

### Week 12: Content Creation

#### Task 12.1: Units and Buildings
- [ ] Create all unit data files
- [ ] Create all building data files
- [ ] Balance stats
- [ ] Create sprites (placeholder OK)
- [ ] Test all units

#### Task 12.2: Maps
- [ ] Create 3 test maps
- [ ] Add resource placement
- [ ] Balance spawn positions
- [ ] Test map balance

## Testing Strategy

### Unit Tests
- Resource transaction validation
- Damage calculation accuracy
- Pathfinding correctness
- Command processing logic

### Integration Tests
- Full match playthrough
- Save/load functionality
- UI interaction flows
- AI behavior validation

### Performance Tests
- 200 unit stress test
- Pathfinding performance
- UI responsiveness
- Memory usage profiling

## Risk Mitigation

### Technical Risks
1. **Pathfinding Performance**: Implement flow fields for groups
2. **Multiplayer Sync**: Deterministic simulation from start
3. **Balance Issues**: Iterative testing and data-driven approach
4. **Scope Creep**: Stick to MVP, features in phases

### Dependencies
- Godot 4.5 stability
- .NET 8.0 compatibility
- Asset availability (using Kenney assets)

## Success Metrics

### Phase 1 Success
- Units can be selected and moved
- Basic combat works
- Resources can be gathered
- Buildings can be placed

### Phase 2 Success
- Full RTS mechanics functional
- AI can play basic match
- UI displays all information
- Performance targets met

### MVP Complete
- 2 factions playable
- 5+ units per faction
- 5+ buildings per faction
- AI opponent
- 3 playable maps
- Basic campaign (3 missions)

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
