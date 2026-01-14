# AI and Pathfinding Systems

## Overview

This document details the artificial intelligence systems for computer-controlled opponents and the pathfinding mechanisms for unit movement.

## Pathfinding Architecture

### Navigation System

#### Grid-Based Navigation
- **Grid Size**: 1x1 tile cells (matches building placement grid)
- **Grid Update**: Real-time updates when buildings placed/destroyed
- **Cost System**: Different terrain types have movement costs
- **Obstacle Handling**: Dynamic obstacle updates

#### Navigation2D Integration
```csharp
public class PathfindingService : IService
{
    private NavigationRegion2D _navRegion;
    private AStarGrid2D _grid;
    
    public void Initialize(Vector2I mapSize)
    {
        _grid = new AStarGrid2D();
        _grid.Size = mapSize;
        _grid.CellSize = new Vector2(16, 16); // 8-bit tile size
        _grid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoCrossing;
        _grid.Update();
    }
    
    public Vector2[] FindPath(Vector2 from, Vector2 to, UnitData unitData)
    {
        Vector2I startCell = WorldToGrid(from);
        Vector2I endCell = WorldToGrid(to);
        
        return _grid.GetPointPath(startCell, endCell)
            .Select(point => GridToWorld(point))
            .ToArray();
    }
}
```

### Pathfinding Algorithms

#### A* Pathfinding (Default)
- **Use Case**: Single unit movement
- **Performance**: O(n log n)
- **Features**: 
  - Heuristic-based search
  - Optimal path guarantee
  - Dynamic cost calculation

#### Flow Field Pathfinding (Groups)
- **Use Case**: Large groups (10+ units)
- **Performance**: O(n) after initial calculation
- **Features**:
  - Single calculation for many units
  - Natural unit spreading
  - Efficient for RTS crowds

```csharp
public class FlowFieldPathfinding
{
    private Vector2[,] _flowField;
    
    public void CalculateFlowField(Vector2 destination, Vector2I gridSize)
    {
        // 1. Calculate cost field (Dijkstra from destination)
        int[,] costField = CalculateCostField(destination, gridSize);
        
        // 2. Generate flow vectors
        _flowField = new Vector2[gridSize.X, gridSize.Y];
        for (int x = 0; x < gridSize.X; x++)
        {
            for (int y = 0; y < gridSize.Y; y++)
            {
                _flowField[x, y] = CalculateFlowDirection(x, y, costField);
            }
        }
    }
    
    public Vector2 GetFlowDirection(Vector2 worldPosition)
    {
        Vector2I gridPos = WorldToGrid(worldPosition);
        return _flowField[gridPos.X, gridPos.Y];
    }
}
```

### Collision Avoidance

#### Local Avoidance
```csharp
public class CollisionAvoidance
{
    public Vector2 CalculateAvoidance(RTSUnit unit, Vector2 desiredVelocity)
    {
        List<RTSUnit> nearbyUnits = GetNearbyUnits(unit, 2.0f);
        
        Vector2 avoidanceForce = Vector2.Zero;
        
        foreach (var other in nearbyUnits)
        {
            Vector2 toOther = other.GlobalPosition - unit.GlobalPosition;
            float distance = toOther.Length();
            
            if (distance < unit.Data.Radius + other.Data.Radius + 0.5f)
            {
                // Push away from overlapping unit
                avoidanceForce -= toOther.Normalized() * (1.0f / distance);
            }
        }
        
        return (desiredVelocity + avoidanceForce).Normalized();
    }
}
```

#### Formation Movement
```csharp
public class FormationController
{
    public List<Vector2> CalculateFormationPositions(
        List<RTSUnit> units, 
        Vector2 destination, 
        FormationType formation)
    {
        switch (formation)
        {
            case FormationType.Line:
                return CalculateLineFormation(units, destination);
            case FormationType.Box:
                return CalculateBoxFormation(units, destination);
            case FormationType.Column:
                return CalculateColumnFormation(units, destination);
            default:
                return CalculateScatterFormation(units, destination);
        }
    }
    
    private List<Vector2> CalculateLineFormation(List<RTSUnit> units, Vector2 dest)
    {
        int count = units.Count;
        float spacing = 1.5f; // tiles between units
        float totalWidth = (count - 1) * spacing;
        
        Vector2 centerToLeft = new Vector2(-totalWidth / 2, 0);
        
        return units.Select((unit, index) => 
            dest + centerToLeft + new Vector2(index * spacing, 0)
        ).ToList();
    }
}
```

### Dynamic Obstacles

#### Building Placement
```csharp
public void OnBuildingPlaced(RTSBuilding building)
{
    Vector2I topLeft = WorldToGrid(building.GlobalPosition - building.Size / 2);
    
    for (int x = 0; x < building.Data.GridSize.X; x++)
    {
        for (int y = 0; y < building.Data.GridSize.Y; y++)
        {
            _grid.SetPointSolid(new Vector2I(topLeft.X + x, topLeft.Y + y), true);
        }
    }
    
    _grid.Update();
    
    // Invalidate cached paths
    InvalidatePathsNear(building.GlobalPosition, building.Size.Length() + 5);
}
```

### Terrain Costs

#### Movement Cost Map
```csharp
public class TerrainCostMap
{
    private float[,] _costMap;
    
    public enum TerrainType
    {
        Normal = 1,      // 1x cost
        Road = 0,        // 0.5x cost (faster)
        Forest = 2,      // 2x cost (slower)
        Water = 255,     // Impassable
        Cliff = 255      // Impassable
    }
    
    public float GetMovementCost(Vector2I gridPos, UnitData unit)
    {
        TerrainType terrain = GetTerrainType(gridPos);
        
        // Flying units (future) ignore terrain
        if (unit.MovementType == MovementType.Flying)
            return 1.0f;
        
        return terrain switch
        {
            TerrainType.Normal => 1.0f,
            TerrainType.Road => 0.75f,
            TerrainType.Forest => 1.5f,
            _ => float.MaxValue // Impassable
        };
    }
}
```

## AI System Architecture

### AI Controller Hierarchy

```
AIController (per AI player)
├── AIPersonality (defines behavior tendencies)
├── AIStrategist (high-level decisions)
│   ├── Opening Strategy
│   ├── Game Plan (aggressive/defensive/economic)
│   └── Adaptation (responds to scouting)
├── EconomyAI (resource management)
│   ├── Worker Management
│   ├── Expansion Timing
│   └── Resource Allocation
├── MilitaryAI (army control)
│   ├── Army Composition
│   ├── Attack Timing
│   ├── Defense Coordination
│   └── Unit Micro
├── BuildOrderAI (construction planning)
│   ├── Build Queue
│   ├── Tech Path
│   └── Supply Management
└── ScoutingAI (map awareness)
    ├── Scout Unit Control
    ├── Enemy Position Tracking
    └── Threat Assessment
```

### AI Difficulty Levels

#### Easy AI
- **APM**: ~30 actions per minute
- **Response Time**: 2-3 second delay
- **Mistakes**: 
  - Sub-optimal worker count (8-10)
  - Slow expansion (1 base for 15+ minutes)
  - Poor unit composition
  - Minimal micro-management
  - No control groups usage
- **Advantages**: None

#### Medium AI
- **APM**: ~60 actions per minute
- **Response Time**: 1-2 second delay
- **Behavior**:
  - Decent worker count (12-15)
  - Expands around 10-12 minutes
  - Basic army composition
  - Simple unit micro (retreat when low HP)
  - Uses 2-3 control groups
- **Advantages**: None

#### Hard AI
- **APM**: ~100 actions per minute
- **Response Time**: 0.5-1 second delay
- **Behavior**:
  - Optimal worker count (16-18)
  - Expands around 8 minutes, multiple bases
  - Good army composition
  - Advanced micro (focus fire, kiting)
  - Full control group usage
  - Tech tree optimization
- **Advantages**: +10% resource gathering

#### Expert AI
- **APM**: ~150 actions per minute
- **Response Time**: 0.1-0.5 second delay
- **Behavior**:
  - Perfect economy management
  - Aggressive expansion (multiple bases)
  - Optimal army compositions
  - Perfect micro-management
  - Predictive scouting
  - Adaptive strategy
- **Advantages**: 
  - +20% resource gathering
  - +10% build/production speed
  - Perfect fog of war vision within 15 tiles

### AI Implementation

#### Main AI Controller
```csharp
public partial class AIController : Node
{
    public Faction AiFaction { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public AIPersonality Personality { get; set; }
    
    private EconomyAI _economyAI;
    private MilitaryAI _militaryAI;
    private BuildOrderAI _buildOrderAI;
    private ScoutingAI _scoutingAI;
    private AIStrategist _strategist;
    
    private float _updateInterval = 0.5f; // Update 2x per second
    private float _timeSinceUpdate = 0f;
    
    public override void _Process(double delta)
    {
        _timeSinceUpdate += (float)delta;
        
        if (_timeSinceUpdate >= _updateInterval)
        {
            UpdateAI();
            _timeSinceUpdate = 0f;
        }
    }
    
    private void UpdateAI()
    {
        // Update in priority order
        _scoutingAI.Update();
        _strategist.Update();
        _economyAI.Update();
        _buildOrderAI.Update();
        _militaryAI.Update();
    }
}
```

#### Economy AI
```csharp
public class EconomyAI
{
    private const int OPTIMAL_WORKERS_PER_BASE = 16;
    private const int WORKERS_ON_GOLD = 10;
    private const int WORKERS_ON_WOOD = 6;
    
    public void Update()
    {
        ManageWorkerProduction();
        ManageWorkerDistribution();
        ManageExpansion();
        ManageSupply();
    }
    
    private void ManageWorkerProduction()
    {
        int currentWorkers = GetWorkerCount();
        int activeBases = GetActiveBases().Count;
        int targetWorkers = activeBases * OPTIMAL_WORKERS_PER_BASE;
        
        if (currentWorkers < targetWorkers)
        {
            foreach (var townHall in GetActiveBases())
            {
                if (!townHall.ProductionQueue.IsFull)
                {
                    townHall.ProduceUnit(WorkerUnitData);
                }
            }
        }
    }
    
    private void ManageWorkerDistribution()
    {
        foreach (var resource in GetResourceDeposits())
        {
            int currentGatherers = resource.CurrentGatherers.Count;
            int optimal = resource.Type == ResourceType.Gold ? 
                WORKERS_ON_GOLD : WORKERS_ON_WOOD;
            
            if (currentGatherers < optimal)
            {
                RTSUnit idleWorker = FindNearestIdleWorker(resource.GlobalPosition);
                if (idleWorker != null)
                {
                    CommandGatherResource(idleWorker, resource);
                }
            }
        }
    }
    
    private void ManageExpansion()
    {
        ResourcePool resources = GetFactionResources();
        
        // Expand when we have resources and workers
        bool shouldExpand = 
            resources.Gold >= 400 &&
            resources.Wood >= 200 &&
            GetWorkerCount() >= OPTIMAL_WORKERS_PER_BASE &&
            GetActiveBases().Count < 4;
        
        if (shouldExpand)
        {
            Vector2 expansionLocation = FindExpansionLocation();
            if (expansionLocation != Vector2.Zero)
            {
                BuildExpansion(expansionLocation);
            }
        }
    }
    
    private void ManageSupply()
    {
        ResourcePool resources = GetFactionResources();
        
        // Build supply when approaching cap
        if (resources.CurrentSupply >= resources.MaxSupply - 10)
        {
            BuildSupplyStructure();
        }
    }
}
```

#### Military AI
```csharp
public class MilitaryAI
{
    private enum MilitaryState { Building, Defending, Attacking, Retreating }
    private MilitaryState _currentState = MilitaryState.Building;
    
    public void Update()
    {
        UpdateMilitaryState();
        
        switch (_currentState)
        {
            case MilitaryState.Building:
                ProduceArmy();
                break;
            case MilitaryState.Defending:
                DefendBases();
                break;
            case MilitaryState.Attacking:
                ExecuteAttack();
                break;
            case MilitaryState.Retreating:
                RetreatUnits();
                break;
        }
        
        MicroManageUnits();
    }
    
    private void UpdateMilitaryState()
    {
        List<RTSUnit> army = GetArmyUnits();
        float armyStrength = CalculateArmyStrength(army);
        float enemyStrength = EstimateEnemyStrength();
        
        if (IsUnderAttack())
        {
            _currentState = MilitaryState.Defending;
        }
        else if (armyStrength > enemyStrength * 1.3f)
        {
            _currentState = MilitaryState.Attacking;
        }
        else if (armyStrength < enemyStrength * 0.5f && IsEngaged())
        {
            _currentState = MilitaryState.Retreating;
        }
        else
        {
            _currentState = MilitaryState.Building;
        }
    }
    
    private void ProduceArmy()
    {
        // Determine desired army composition
        var composition = DetermineArmyComposition();
        
        foreach (var (unitType, targetCount) in composition)
        {
            int currentCount = GetUnitCount(unitType);
            if (currentCount < targetCount)
            {
                ProduceUnit(unitType);
            }
        }
    }
    
    private Dictionary<UnitData, int> DetermineArmyComposition()
    {
        // Example composition: 60% infantry, 30% ranged, 10% special
        int totalArmySize = 40; // Target army size
        
        return new Dictionary<UnitData, int>
        {
            { InfantryUnitData, (int)(totalArmySize * 0.6f) },
            { RangedUnitData, (int)(totalArmySize * 0.3f) },
            { SpecialUnitData, (int)(totalArmySize * 0.1f) }
        };
    }
    
    private void MicroManageUnits()
    {
        if (Difficulty < DifficultyLevel.Hard)
            return; // Only hard+ AI micro-manages
        
        foreach (var unit in GetArmyUnits())
        {
            // Kiting: ranged units attack and move back
            if (unit.Data.AttackRange > 1)
            {
                var nearestEnemy = FindNearestEnemy(unit.GlobalPosition);
                if (nearestEnemy != null)
                {
                    float distance = unit.GlobalPosition.DistanceTo(nearestEnemy.GlobalPosition);
                    
                    if (distance < unit.Data.AttackRange - 0.5f)
                    {
                        // Too close, kite backward
                        Vector2 retreatDir = (unit.GlobalPosition - nearestEnemy.GlobalPosition).Normalized();
                        CommandMove(unit, unit.GlobalPosition + retreatDir * 2);
                    }
                    else if (distance <= unit.Data.AttackRange)
                    {
                        // In range, attack
                        CommandAttack(unit, nearestEnemy);
                    }
                }
            }
            
            // Retreat low HP units
            if (unit.Health < unit.Data.MaxHealth * 0.3f)
            {
                Vector2 safePosition = FindSafePosition(unit.GlobalPosition);
                CommandMove(unit, safePosition);
            }
        }
    }
    
    private void ExecuteAttack()
    {
        List<RTSUnit> army = GetArmyUnits();
        
        if (army.Count == 0)
        {
            _currentState = MilitaryState.Building;
            return;
        }
        
        // Find attack target
        Vector2 targetLocation = FindAttackTarget();
        
        if (targetLocation != Vector2.Zero)
        {
            // Attack-move entire army
            foreach (var unit in army)
            {
                CommandAttackMove(unit, targetLocation);
            }
        }
    }
    
    private Vector2 FindAttackTarget()
    {
        // Priority: Enemy expansions > enemy army > main base
        
        // 1. Look for vulnerable expansions
        var enemyExpansions = GetEnemyExpansions();
        if (enemyExpansions.Count > 0)
        {
            return enemyExpansions
                .OrderBy(exp => exp.GlobalPosition.DistanceTo(GetMainBase().GlobalPosition))
                .First().GlobalPosition;
        }
        
        // 2. Attack enemy army in our territory
        var enemyUnits = GetEnemyUnitsInTerritory();
        if (enemyUnits.Count > 0)
        {
            return CalculateCentroid(enemyUnits.Select(u => u.GlobalPosition).ToList());
        }
        
        // 3. Attack main enemy base
        return GetEnemyMainBase()?.GlobalPosition ?? Vector2.Zero;
    }
}
```

#### Build Order AI
```csharp
public class BuildOrderAI
{
    private Queue<BuildStep> _buildQueue = new Queue<BuildStep>();
    
    public void Update()
    {
        if (_buildQueue.Count == 0)
        {
            GenerateBuildOrder();
        }
        
        ExecuteNextStep();
    }
    
    private void GenerateBuildOrder()
    {
        // Example opening for Humans
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Unit, Data = PeasantData, Count = 1 });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Unit, Data = PeasantData, Count = 1 });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Building, Data = FarmData });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Unit, Data = PeasantData, Count = 2 });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Building, Data = BarracksData });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Unit, Data = PeasantData, Count = 3 });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Building, Data = FarmData });
        _buildQueue.Enqueue(new BuildStep { Type = BuildType.Unit, Data = FootmanData, Count = 3 });
        // Continue with adaptive strategy...
    }
    
    private void ExecuteNextStep()
    {
        if (_buildQueue.Count == 0)
            return;
        
        var step = _buildQueue.Peek();
        
        if (TryExecuteStep(step))
        {
            _buildQueue.Dequeue();
        }
    }
    
    private bool TryExecuteStep(BuildStep step)
    {
        if (!CanAfford(step))
            return false;
        
        switch (step.Type)
        {
            case BuildType.Unit:
                return ProduceUnit(step.Data, step.Count);
            case BuildType.Building:
                return ConstructBuilding(step.Data);
            case BuildType.Upgrade:
                return ResearchUpgrade(step.Data);
        }
        
        return false;
    }
}
```

### AI Personalities

#### Aggressive
```yaml
Personality: Aggressive
Behavior:
  - Early military production (2-3 barracks by minute 5)
  - Minimal economy (12 workers max)
  - Constant pressure and harassment
  - Prioritize military buildings
  - Attack at 60% target army size
  
Build Tendency:
  - Barracks: Very High
  - Advanced Tech: Low
  - Expansion: Low
  - Defense: Low
```

#### Defensive
```yaml
Personality: Defensive
Behavior:
  - Strong economy (18+ workers)
  - Multiple defensive structures
  - Turtling until late game
  - Counter-attack after defense
  - Attack at 120% target army size
  
Build Tendency:
  - Towers: High
  - Walls: High
  - Advanced Tech: Medium
  - Expansion: Medium (when safe)
```

#### Economic
```yaml
Personality: Economic
Behavior:
  - Maximum worker production
  - Fast expansion (multiple bases)
  - Delayed military
  - Tech rush to advanced units
  - Attack at 150% target army size
  
Build Tendency:
  - Workers: Maximum
  - Expansion: Very High
  - Advanced Tech: Very High
  - Military: Delayed but strong
```

#### Balanced
```yaml
Personality: Balanced
Behavior:
  - Moderate worker production (14-16)
  - Timed expansion (around minute 10)
  - Mixed military and economy
  - Adapts to opponent
  - Attack at 100% target army size
  
Build Tendency:
  - All aspects: Medium
  - Adapts based on scouting
```

## Performance Optimization

### AI Update Throttling
- Update AI systems at reduced frequency (2-4 times per second)
- Stagger updates across multiple AI players
- Cache expensive calculations (pathfinding, threat assessment)

### Pathfinding Optimization
- Path caching with invalidation
- Flow fields for groups > 10 units
- Hierarchical pathfinding for long distances
- Lazy path recalculation (only when needed)

### Spatial Partitioning
- Use quadtree for unit queries
- Grid-based spatial hash for proximity checks
- Reduce search radius for common operations

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
