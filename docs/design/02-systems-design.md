# Systems Design Document
## RedGate: Tactical Dungeon RPG

> This document provides detailed specifications for each game system in the tactical RPG.

---

## System Overview

Systems are the "brains" of the game, processing groups of components and orchestrating game logic. Each system is a Node attached to its parent Manager (DungeonManager or TownManager).

```
┌─────────────────────────────────────────────────────────────┐
│                     SYSTEM HIERARCHY                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  DungeonManager Systems:           TownManager Systems:      │
│  ─────────────────────────         ─────────────────────     │
│  ├── CombatSystem                  ├── DialogueSystem        │
│  ├── AISystem                      ├── ShopSystem            │
│  ├── PhysicsSystem                 ├── CraftSystem           │
│  ├── AbilitySystem                 ├── QuestSystem           │
│  ├── SpawnSystem                   └── NPCSystem             │
│  ├── LootSystem                                              │
│  ├── ProjectileSystem              Shared Systems:           │
│  └── EffectSystem                  ───────────────           │
│                                    ├── UISystem              │
│                                    ├── InventorySystem       │
│                                    └── ProgressionSystem     │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Combat System

### Purpose
Manages all combat interactions including damage calculation, hit detection, and combat resolution.

### Responsibilities
- Process attack actions
- Calculate damage with modifiers
- Apply damage to targets
- Handle critical hits
- Manage combat effects (DoT, HoT)
- Trigger death sequences

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class CombatSystem : Node, ISystem
{
    // Registered combatants
    private List<CombatComponent> _combatants = new();
    private IEventService _eventService;
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
        _eventService.Subscribe<CombatTickTimeout>(OnCombatTick);
    }
    
    public void RegisterCombatant(CombatComponent combatant)
    {
        if (!_combatants.Contains(combatant))
            _combatants.Add(combatant);
    }
    
    public void UnregisterCombatant(CombatComponent combatant)
    {
        _combatants.Remove(combatant);
    }
    
    /// <summary>
    /// Process an attack from attacker to target.
    /// </summary>
    public DamageResult ProcessAttack(IEntity attacker, IEntity target)
    {
        if (attacker is not Node2D attackerNode || target is not Node2D targetNode)
            return new DamageResult(false, 0, false);
            
        var attackerCombat = GetComponent<CombatComponent>(attackerNode);
        var targetCombat = GetComponent<CombatComponent>(targetNode);
        var targetHealth = GetComponent<HealthComponent>(targetNode);
        
        if (attackerCombat == null || targetHealth == null)
            return new DamageResult(false, 0, false);
        
        // Check if attack is on cooldown
        if (attackerCombat.AttackCooldown > 0)
            return new DamageResult(false, 0, false);
        
        // Calculate damage
        var damage = DamageCalculator.Calculate(attackerCombat, targetCombat);
        
        // Apply damage
        ApplyDamage(target as IDamageable, damage);
        
        // Set attack cooldown
        attackerCombat.AttackCooldown = 1f / attackerCombat.AttackSpeed;
        
        // Publish combat event
        _eventService.Publish<DamageEvent>(new DamageEvent(attacker, target, damage));
        
        return new DamageResult(true, damage.FinalDamage, damage.IsCritical);
    }
    
    /// <summary>
    /// Apply damage to a damageable target.
    /// </summary>
    public void ApplyDamage(IDamageable target, DamageInfo damage)
    {
        if (target == null) return;
        
        target.TakeDamage(damage);
        
        // Check for death
        if (target is Node2D node)
        {
            var health = GetComponent<HealthComponent>(node);
            if (health != null && health.IsDead)
            {
                ProcessDeath(target as IEntity);
            }
        }
    }
    
    /// <summary>
    /// Handle entity death.
    /// </summary>
    public void ProcessDeath(IEntity entity)
    {
        if (entity == null) return;
        
        // Unregister from combat
        if (entity is Node2D node)
        {
            var combat = GetComponent<CombatComponent>(node);
            if (combat != null)
                UnregisterCombatant(combat);
        }
        
        // Trigger death on entity
        if (entity is IDamageable damageable)
            damageable.Die();
        
        // Publish death event
        _eventService.Publish<DeathEvent>(new DeathEvent(entity));
    }
    
    private void OnCombatTick()
    {
        float delta = 0.1f; // CombatTickInterval
        
        foreach (var combatant in _combatants)
        {
            // Reduce attack cooldowns
            if (combatant.AttackCooldown > 0)
                combatant.AttackCooldown = Mathf.Max(0, combatant.AttackCooldown - delta);
        }
    }
    
    private T GetComponent<T>(Node2D node) where T : Node
    {
        foreach (var child in node.GetChildren())
        {
            if (child is T component)
                return component;
        }
        return null;
    }
}

public record DamageInfo(
    int BaseDamage,
    int FinalDamage,
    DamageType Type,
    bool IsCritical,
    IEntity Source
);

public record DamageResult(bool Hit, int Damage, bool IsCritical);
```

### Damage Calculation Formula

```
Base Damage = Weapon Damage + (Strength × 0.5) + (Intelligence × 0.3 for magic)
Modified Damage = Base Damage × (1 + Damage Bonus%)
Critical Check = Random(0,1) < CritChance
Critical Damage = Modified Damage × CritMultiplier (if critical)
Armor Reduction = Armor / (Armor + 100)
Final Damage = Critical Damage × (1 - Armor Reduction)
```

---

## AI System

### Purpose
Manages enemy AI decision-making and behavior execution.

### AI States

```csharp
public enum AIState : byte
{
    Idle = 0,       // Standing still, no target
    Patrol = 1,     // Following patrol path
    Chase = 2,      // Moving toward target
    Attack = 3,     // In attack range, attacking
    Flee = 4,       // Running away (low health)
    Dead = 5        // Death animation playing
}
```

### AI Behaviors

```csharp
public enum AIBehavior : byte
{
    Aggressive = 0,    // Always chases and attacks
    Defensive = 1,     // Only attacks when attacked
    Coward = 2,        // Flees when health is low
    Ranged = 3,        // Maintains distance, uses ranged attacks
    Support = 4,       // Heals/buffs allies
    Boss = 5           // Complex multi-phase behavior
}
```

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class AISystem : Node, ISystem
{
    private List<AIComponent> _aiEntities = new();
    private HeroEntity _playerTarget;
    private IEventService _eventService;
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
        _eventService.Subscribe<PulseTimeout>(OnPulse);
    }
    
    public void RegisterAI(AIComponent ai)
    {
        if (!_aiEntities.Contains(ai))
            _aiEntities.Add(ai);
    }
    
    public void SetPlayerTarget(HeroEntity player)
    {
        _playerTarget = player;
    }
    
    private void OnPulse()
    {
        foreach (var ai in _aiEntities.ToList())
        {
            if (ai == null || !ai.IsInsideTree())
            {
                _aiEntities.Remove(ai);
                continue;
            }
            
            ProcessAI(ai);
        }
    }
    
    private void ProcessAI(AIComponent ai)
    {
        var entity = ai.GetParent<Node2D>();
        if (entity == null) return;
        
        // State machine
        switch (ai.CurrentState)
        {
            case AIState.Idle:
                ProcessIdle(ai, entity);
                break;
            case AIState.Patrol:
                ProcessPatrol(ai, entity);
                break;
            case AIState.Chase:
                ProcessChase(ai, entity);
                break;
            case AIState.Attack:
                ProcessAttack(ai, entity);
                break;
            case AIState.Flee:
                ProcessFlee(ai, entity);
                break;
        }
    }
    
    private void ProcessIdle(AIComponent ai, Node2D entity)
    {
        // Look for target
        if (_playerTarget != null && IsInDetectionRange(entity, _playerTarget, ai.DetectionRange))
        {
            ai.Target = _playerTarget;
            ai.CurrentState = AIState.Chase;
        }
    }
    
    private void ProcessChase(AIComponent ai, Node2D entity)
    {
        if (ai.Target == null)
        {
            ai.CurrentState = AIState.Idle;
            return;
        }
        
        var distance = entity.GlobalPosition.DistanceTo(ai.Target.GlobalPosition);
        
        // In attack range?
        if (distance <= ai.AttackRange)
        {
            ai.CurrentState = AIState.Attack;
            return;
        }
        
        // Lost target?
        if (distance > ai.DetectionRange * 1.5f)
        {
            ai.Target = null;
            ai.CurrentState = AIState.Idle;
            return;
        }
        
        // Move toward target
        var physics = entity.GetNode<PhysicsComponent>("Physics");
        if (physics != null)
        {
            var direction = (ai.Target.GlobalPosition - entity.GlobalPosition).Normalized();
            physics.TargetVelocity = direction * physics.Speed;
        }
    }
    
    private void ProcessAttack(AIComponent ai, Node2D entity)
    {
        if (ai.Target == null)
        {
            ai.CurrentState = AIState.Idle;
            return;
        }
        
        var distance = entity.GlobalPosition.DistanceTo(ai.Target.GlobalPosition);
        
        // Out of range?
        if (distance > ai.AttackRange * 1.2f)
        {
            ai.CurrentState = AIState.Chase;
            return;
        }
        
        // Stop moving and attack
        var physics = entity.GetNode<PhysicsComponent>("Physics");
        if (physics != null)
            physics.TargetVelocity = Vector2.Zero;
        
        // Attack logic handled by CombatSystem via events
    }
    
    private void ProcessPatrol(AIComponent ai, Node2D entity)
    {
        // Patrol path following logic
        // Check for player detection to transition to Chase
    }
    
    private void ProcessFlee(AIComponent ai, Node2D entity)
    {
        if (ai.Target == null)
        {
            ai.CurrentState = AIState.Idle;
            return;
        }
        
        // Move away from target
        var physics = entity.GetNode<PhysicsComponent>("Physics");
        if (physics != null)
        {
            var direction = (entity.GlobalPosition - ai.Target.GlobalPosition).Normalized();
            physics.TargetVelocity = direction * physics.Speed;
        }
    }
    
    private bool IsInDetectionRange(Node2D entity, Node2D target, float range)
    {
        return entity.GlobalPosition.DistanceTo(target.GlobalPosition) <= range;
    }
}
```

---

## Ability System

### Purpose
Manages hero abilities, cooldowns, and ability execution.

### Ability Types

```csharp
public enum AbilityType : byte
{
    Instant = 0,      // Immediate effect
    Projectile = 1,   // Spawns a projectile
    AoE = 2,          // Area of effect
    Buff = 3,         // Applies buff to self/ally
    Debuff = 4,       // Applies debuff to enemy
    Summon = 5,       // Spawns allied entity
    Channel = 6,      // Requires channeling time
    Toggle = 7        // On/off ability
}
```

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class AbilitySystem : Node, ISystem
{
    private List<AbilityComponent> _abilityUsers = new();
    private IEventService _eventService;
    private CombatSystem _combatSystem;
    private ProjectileSystem _projectileSystem;
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
    }
    
    public override void _Process(double delta)
    {
        // Update cooldowns
        foreach (var user in _abilityUsers)
        {
            UpdateCooldowns(user, (float)delta);
        }
    }
    
    /// <summary>
    /// Attempt to use an ability from a slot.
    /// </summary>
    public bool UseAbility(IEntity caster, int slot)
    {
        if (caster is not Node2D casterNode)
            return false;
            
        var abilities = GetComponent<AbilityComponent>(casterNode);
        var mana = GetComponent<ManaComponent>(casterNode);
        
        if (abilities == null || slot < 0 || slot >= abilities.Abilities.Length)
            return false;
        
        var ability = abilities.Abilities[slot];
        if (ability == null)
            return false;
        
        // Check cooldown
        if (abilities.IsOnCooldown(slot))
            return false;
        
        // Check mana
        if (mana != null && !mana.CanCast(ability.ManaCost))
            return false;
        
        // Execute ability
        ExecuteAbility(caster, ability, slot);
        
        // Consume mana
        mana?.SpendMana(ability.ManaCost);
        
        // Start cooldown
        abilities.Cooldowns[slot] = ability.Cooldown;
        
        // Publish event
        _eventService.Publish<AbilityUsedEvent>(new AbilityUsedEvent(caster, ability));
        
        return true;
    }
    
    private void ExecuteAbility(IEntity caster, AbilityData ability, int slot)
    {
        var casterNode = caster as Node2D;
        if (casterNode == null) return;
        
        switch (ability.Type)
        {
            case AbilityType.Instant:
                ExecuteInstant(caster, ability);
                break;
            case AbilityType.Projectile:
                ExecuteProjectile(caster, ability);
                break;
            case AbilityType.AoE:
                ExecuteAoE(caster, ability);
                break;
            case AbilityType.Buff:
                ExecuteBuff(caster, ability);
                break;
            case AbilityType.Debuff:
                ExecuteDebuff(caster, ability);
                break;
        }
    }
    
    private void ExecuteInstant(IEntity caster, AbilityData ability)
    {
        var casterNode = caster as Node2D;
        var combat = GetComponent<CombatComponent>(casterNode);
        
        // Find target in range
        var target = FindNearestEnemy(casterNode.GlobalPosition, ability.Range);
        if (target == null) return;
        
        // Apply damage
        var damage = new DamageInfo(
            ability.BaseDamage,
            CalculateAbilityDamage(caster, ability),
            DamageType.Magical,
            false,
            caster
        );
        
        _combatSystem?.ApplyDamage(target as IDamageable, damage);
    }
    
    private void ExecuteProjectile(IEntity caster, AbilityData ability)
    {
        var casterNode = caster as Node2D;
        var physics = GetComponent<PhysicsComponent>(casterNode);
        
        var direction = physics?.FacingDirection ?? Vector2.Down;
        _projectileSystem?.SpawnProjectile(
            casterNode.GlobalPosition,
            direction,
            ability,
            caster
        );
    }
    
    private void ExecuteAoE(IEntity caster, AbilityData ability)
    {
        var casterNode = caster as Node2D;
        
        // Find all enemies in range
        var enemies = FindEnemiesInRange(casterNode.GlobalPosition, ability.Range);
        
        foreach (var enemy in enemies)
        {
            var damage = new DamageInfo(
                ability.BaseDamage,
                CalculateAbilityDamage(caster, ability),
                DamageType.Magical,
                false,
                caster
            );
            
            _combatSystem?.ApplyDamage(enemy as IDamageable, damage);
        }
    }
    
    private void ExecuteBuff(IEntity caster, AbilityData ability)
    {
        foreach (var effect in ability.Effects)
        {
            ApplyEffect(caster, effect);
        }
    }
    
    private void ExecuteDebuff(IEntity caster, AbilityData ability)
    {
        var casterNode = caster as Node2D;
        var target = FindNearestEnemy(casterNode.GlobalPosition, ability.Range);
        
        if (target != null)
        {
            foreach (var effect in ability.Effects)
            {
                ApplyEffect(target as IEntity, effect);
            }
        }
    }
    
    private int CalculateAbilityDamage(IEntity caster, AbilityData ability)
    {
        // Base damage + scaling from stats
        var casterNode = caster as Node2D;
        var stats = GetStats(casterNode);
        
        return ability.BaseDamage + (int)(stats.Intelligence * 0.5f);
    }
    
    private void UpdateCooldowns(AbilityComponent abilities, float delta)
    {
        var keys = abilities.Cooldowns.Keys.ToList();
        foreach (var key in keys)
        {
            if (abilities.Cooldowns[key] > 0)
                abilities.Cooldowns[key] -= delta;
        }
    }
    
    private void ApplyEffect(IEntity target, EffectData effect)
    {
        // Effect application logic
    }
    
    private IEntity FindNearestEnemy(Vector2 position, float range)
    {
        // Implementation
        return null;
    }
    
    private List<IEntity> FindEnemiesInRange(Vector2 position, float range)
    {
        // Implementation
        return new List<IEntity>();
    }
    
    private StatsData GetStats(Node2D entity)
    {
        // Get stats from entity
        return new StatsData();
    }
    
    private T GetComponent<T>(Node2D node) where T : Node
    {
        foreach (var child in node.GetChildren())
        {
            if (child is T component)
                return component;
        }
        return null;
    }
}
```

---

## Loot System

### Purpose
Manages item drops, loot tables, and item collection.

### Loot Table Structure

```csharp
[GlobalClass]
public partial class LootTable : Resource
{
    [Export] public LootEntry[] Entries { get; private set; }
    [Export] public int MinDrops { get; private set; } = 1;
    [Export] public int MaxDrops { get; private set; } = 3;
    [Export] public float NothingChance { get; private set; } = 0.1f;
}

[GlobalClass]
public partial class LootEntry : Resource
{
    [Export] public ItemData Item { get; private set; }
    [Export] public float Weight { get; private set; } = 1f;
    [Export] public int MinQuantity { get; private set; } = 1;
    [Export] public int MaxQuantity { get; private set; } = 1;
}
```

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class LootSystem : Node, ISystem
{
    [Export] public PackedScene ItemEntityTemplate { get; private set; }
    
    private List<ItemEntity> _groundItems = new();
    private IEventService _eventService;
    private RandomNumberGenerator _random = new();
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
        _eventService.Subscribe<DeathEvent>(OnEntityDeath);
        _random.Randomize();
    }
    
    /// <summary>
    /// Spawn loot at a position based on a loot table.
    /// </summary>
    public void SpawnLoot(Vector2 position, LootTable table)
    {
        if (table == null) return;
        
        // Check for no drops
        if (_random.Randf() < table.NothingChance)
            return;
        
        // Determine number of drops
        int dropCount = _random.RandiRange(table.MinDrops, table.MaxDrops);
        
        for (int i = 0; i < dropCount; i++)
        {
            var item = RollLootItem(table);
            if (item != null)
            {
                SpawnItemEntity(position, item);
            }
        }
    }
    
    /// <summary>
    /// Spawn a specific item at a position.
    /// </summary>
    public void SpawnItemEntity(Vector2 position, ItemInstance item)
    {
        if (ItemEntityTemplate == null || item == null) return;
        
        var entity = ItemEntityTemplate.Instantiate<ItemEntity>();
        entity.Inject(item);
        entity.GlobalPosition = position + RandomOffset();
        
        GetParent().AddChild(entity);
        _groundItems.Add(entity);
    }
    
    /// <summary>
    /// Player collects an item from the ground.
    /// </summary>
    public void CollectItem(HeroEntity collector, ItemEntity item)
    {
        if (collector == null || item == null) return;
        
        var inventory = collector.GetNode<InventoryComponent>("Inventory");
        if (inventory == null) return;
        
        // Try to add to inventory
        if (inventory.Items.Count < inventory.Capacity)
        {
            inventory.Items.Add(item.ItemInstance);
            
            _groundItems.Remove(item);
            item.QueueFree();
            
            _eventService.Publish<ItemPickupEvent>(new ItemPickupEvent(item.ItemInstance));
        }
    }
    
    private ItemInstance RollLootItem(LootTable table)
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (var entry in table.Entries)
        {
            totalWeight += entry.Weight;
        }
        
        // Roll for item
        float roll = _random.RandfRange(0, totalWeight);
        float cumulative = 0f;
        
        foreach (var entry in table.Entries)
        {
            cumulative += entry.Weight;
            if (roll <= cumulative)
            {
                return GenerateItemInstance(entry);
            }
        }
        
        return null;
    }
    
    private ItemInstance GenerateItemInstance(LootEntry entry)
    {
        int quantity = _random.RandiRange(entry.MinQuantity, entry.MaxQuantity);
        
        return new ItemInstance
        {
            Data = entry.Item,
            Quantity = quantity,
            Modifiers = LootGenerator.RollModifiers(entry.Item.Rarity)
        };
    }
    
    private Vector2 RandomOffset()
    {
        return new Vector2(
            _random.RandfRange(-20, 20),
            _random.RandfRange(-20, 20)
        );
    }
    
    private void OnEntityDeath(IEvent eventData)
    {
        if (eventData is not DeathEvent death) return;
        
        if (death.Entity is EnemyEntity enemy)
        {
            var lootTable = enemy.Data?.LootTable;
            if (lootTable != null)
            {
                SpawnLoot((enemy as Node2D).GlobalPosition, lootTable);
            }
        }
    }
}

public class ItemInstance
{
    public ItemData Data { get; set; }
    public int Quantity { get; set; }
    public StatModifier[] Modifiers { get; set; }
}
```

---

## Spawn System

### Purpose
Manages entity spawning in the dungeon, including enemies and objects.

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class SpawnSystem : Node, ISystem
{
    [Export] public PackedScene EnemyTemplate { get; private set; }
    
    private List<EnemyEntity> _pooledEnemies = new();
    private List<EnemyEntity> _activeEnemies = new();
    private Queue<SpawnRequest> _spawnQueue = new();
    
    private IEventService _eventService;
    private AISystem _aiSystem;
    private CombatSystem _combatSystem;
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
        _eventService.Subscribe<RoomEnteredEvent>(OnRoomEntered);
    }
    
    /// <summary>
    /// Queue enemies to spawn in a room.
    /// </summary>
    public void QueueSpawn(EnemyData enemyData, Vector2 position)
    {
        _spawnQueue.Enqueue(new SpawnRequest(enemyData, position));
    }
    
    /// <summary>
    /// Process spawn queue.
    /// </summary>
    public void ProcessSpawns()
    {
        while (_spawnQueue.Count > 0)
        {
            var request = _spawnQueue.Dequeue();
            SpawnEnemy(request.EnemyData, request.Position);
        }
    }
    
    /// <summary>
    /// Spawn an enemy at a position.
    /// </summary>
    public EnemyEntity SpawnEnemy(EnemyData data, Vector2 position)
    {
        // Try to get from pool
        var enemy = GetFromPool(data);
        
        if (enemy == null)
        {
            // Create new instance
            enemy = EnemyTemplate.Instantiate<EnemyEntity>();
            enemy.Inject(data);
            GetParent().AddChild(enemy);
        }
        
        enemy.GlobalPosition = position;
        enemy.Show();
        _activeEnemies.Add(enemy);
        
        // Register with systems
        var ai = enemy.GetNode<AIComponent>("AI");
        var combat = enemy.GetNode<CombatComponent>("Combat");
        
        _aiSystem?.RegisterAI(ai);
        _combatSystem?.RegisterCombatant(combat);
        
        return enemy;
    }
    
    /// <summary>
    /// Return enemy to pool.
    /// </summary>
    public void DespawnEnemy(EnemyEntity enemy)
    {
        if (enemy == null) return;
        
        _activeEnemies.Remove(enemy);
        enemy.Hide();
        enemy.GlobalPosition = Vector2.Zero;
        
        // Reset state
        var health = enemy.GetNode<HealthComponent>("Health");
        if (health != null)
        {
            health.CurrentHealth = health.MaxHealth;
        }
        
        var ai = enemy.GetNode<AIComponent>("AI");
        if (ai != null)
        {
            ai.CurrentState = AIState.Idle;
            ai.Target = null;
        }
        
        _pooledEnemies.Add(enemy);
    }
    
    /// <summary>
    /// Populate a room with enemies based on room data.
    /// </summary>
    public void PopulateRoom(RoomData room, Vector2 roomOrigin)
    {
        foreach (var spawn in room.EnemySpawns)
        {
            var worldPos = roomOrigin + spawn.LocalPosition;
            QueueSpawn(spawn.EnemyData, worldPos);
        }
        
        ProcessSpawns();
    }
    
    /// <summary>
    /// Clear all active enemies.
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (var enemy in _activeEnemies.ToList())
        {
            DespawnEnemy(enemy);
        }
    }
    
    private EnemyEntity GetFromPool(EnemyData data)
    {
        foreach (var enemy in _pooledEnemies)
        {
            if (enemy.Data == data)
            {
                _pooledEnemies.Remove(enemy);
                return enemy;
            }
        }
        return null;
    }
    
    private void OnRoomEntered(IEvent eventData)
    {
        if (eventData is RoomEnteredEvent roomEvent)
        {
            // Spawn room enemies
        }
    }
    
    private record SpawnRequest(EnemyData EnemyData, Vector2 Position);
}
```

---

## Physics System

### Purpose
Handles entity movement and collision response.

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class PhysicsSystem : Node, ISystem
{
    private List<PhysicsComponent> _bodies = new();
    
    public void RegisterBody(PhysicsComponent body)
    {
        if (!_bodies.Contains(body))
            _bodies.Add(body);
    }
    
    public void UnregisterBody(PhysicsComponent body)
    {
        _bodies.Remove(body);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        foreach (var body in _bodies.ToList())
        {
            if (body == null || !body.IsInsideTree())
            {
                _bodies.Remove(body);
                continue;
            }
            
            ProcessMovement(body, (float)delta);
        }
    }
    
    private void ProcessMovement(PhysicsComponent body, float delta)
    {
        // Smooth acceleration toward target velocity
        body.Velocity = body.Velocity.MoveToward(
            body.TargetVelocity,
            body.Acceleration * delta
        );
        
        // Update facing direction
        if (body.Velocity.Length() > 0.1f)
        {
            body.FacingDirection = body.Velocity.Normalized();
        }
        
        // Move with collision
        body.MoveAndSlide();
    }
}
```

---

## Dialogue System

### Purpose
Manages NPC conversations and dialogue trees.

### Dialogue Structure

```csharp
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
    [Export] public string Text { get; private set; }
    [Export] public DialogueChoice[] Choices { get; private set; }
    [Export] public string NextNodeId { get; private set; } // If no choices
}

[GlobalClass]
public partial class DialogueChoice : Resource
{
    [Export] public string Text { get; private set; }
    [Export] public string NextNodeId { get; private set; }
    [Export] public string RequiredFlag { get; private set; }
    [Export] public string SetFlag { get; private set; }
}
```

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class DialogueSystem : Node, ISystem
{
    public bool IsInDialogue { get; private set; }
    public DialogueData CurrentDialogue { get; private set; }
    public DialogueNode CurrentNode { get; private set; }
    
    private IEventService _eventService;
    private Dictionary<string, bool> _flags = new();
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
    }
    
    /// <summary>
    /// Start a dialogue conversation.
    /// </summary>
    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null || IsInDialogue) return;
        
        CurrentDialogue = dialogue;
        IsInDialogue = true;
        
        // Find start node
        CurrentNode = FindNode(dialogue.StartNodeId);
        
        _eventService.Publish<DialogueStartedEvent>(new DialogueStartedEvent(dialogue));
        DisplayCurrentNode();
    }
    
    /// <summary>
    /// Select a dialogue choice.
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        if (!IsInDialogue || CurrentNode == null) return;
        
        if (choiceIndex < 0 || choiceIndex >= CurrentNode.Choices.Length)
            return;
        
        var choice = CurrentNode.Choices[choiceIndex];
        
        // Set flag if specified
        if (!string.IsNullOrEmpty(choice.SetFlag))
        {
            _flags[choice.SetFlag] = true;
        }
        
        // Navigate to next node
        AdvanceToNode(choice.NextNodeId);
    }
    
    /// <summary>
    /// Advance to next node (for nodes without choices).
    /// </summary>
    public void AdvanceDialogue()
    {
        if (!IsInDialogue || CurrentNode == null) return;
        
        if (CurrentNode.Choices.Length > 0) return; // Has choices, don't auto-advance
        
        AdvanceToNode(CurrentNode.NextNodeId);
    }
    
    /// <summary>
    /// End the current dialogue.
    /// </summary>
    public void EndDialogue()
    {
        if (!IsInDialogue) return;
        
        var dialogue = CurrentDialogue;
        
        CurrentDialogue = null;
        CurrentNode = null;
        IsInDialogue = false;
        
        _eventService.Publish<DialogueEndedEvent>(new DialogueEndedEvent(dialogue));
    }
    
    private void AdvanceToNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
        {
            EndDialogue();
            return;
        }
        
        CurrentNode = FindNode(nodeId);
        
        if (CurrentNode == null)
        {
            EndDialogue();
            return;
        }
        
        DisplayCurrentNode();
    }
    
    private void DisplayCurrentNode()
    {
        // Filter choices by required flags
        var availableChoices = CurrentNode.Choices
            .Where(c => string.IsNullOrEmpty(c.RequiredFlag) || HasFlag(c.RequiredFlag))
            .ToArray();
        
        _eventService.Publish<DialogueNodeEvent>(
            new DialogueNodeEvent(CurrentNode, availableChoices)
        );
    }
    
    private DialogueNode FindNode(string nodeId)
    {
        return CurrentDialogue.Nodes.FirstOrDefault(n => n.NodeId == nodeId);
    }
    
    private bool HasFlag(string flag)
    {
        return _flags.TryGetValue(flag, out var value) && value;
    }
}
```

---

## Shop System

### Purpose
Manages buying and selling items with NPCs.

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class ShopSystem : Node, ISystem
{
    public bool IsShopOpen { get; private set; }
    public NPCEntity CurrentVendor { get; private set; }
    public List<ItemData> CurrentStock { get; private set; } = new();
    
    private IEventService _eventService;
    private IInventoryService _inventoryService;
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
        _inventoryService = ServiceProvider.InventoryService();
    }
    
    /// <summary>
    /// Open shop with a vendor.
    /// </summary>
    public void OpenShop(NPCEntity vendor)
    {
        if (vendor == null || IsShopOpen) return;
        
        CurrentVendor = vendor;
        CurrentStock = GenerateStock(vendor.ShopData);
        IsShopOpen = true;
        
        _eventService.Publish<ShopOpenedEvent>(new ShopOpenedEvent(vendor));
    }
    
    /// <summary>
    /// Close the current shop.
    /// </summary>
    public void CloseShop()
    {
        if (!IsShopOpen) return;
        
        CurrentVendor = null;
        CurrentStock.Clear();
        IsShopOpen = false;
        
        _eventService.Publish<ShopClosedEvent>();
    }
    
    /// <summary>
    /// Purchase an item from the shop.
    /// </summary>
    public bool BuyItem(int stockIndex, int quantity = 1)
    {
        if (!IsShopOpen || stockIndex < 0 || stockIndex >= CurrentStock.Count)
            return false;
        
        var item = CurrentStock[stockIndex];
        int totalCost = item.Value * quantity;
        
        // Check if player has enough gold
        if (!_inventoryService.HasGold(totalCost))
            return false;
        
        // Check inventory space
        if (!_inventoryService.HasSpace())
            return false;
        
        // Process transaction
        _inventoryService.SpendGold(totalCost);
        _inventoryService.AddItem(new ItemInstance { Data = item, Quantity = quantity });
        
        _eventService.Publish<ItemPurchasedEvent>(new ItemPurchasedEvent(item, totalCost));
        
        return true;
    }
    
    /// <summary>
    /// Sell an item to the shop.
    /// </summary>
    public bool SellItem(ItemInstance item)
    {
        if (!IsShopOpen || item == null)
            return false;
        
        int sellValue = (int)(item.Data.Value * 0.5f * item.Quantity);
        
        // Process transaction
        _inventoryService.RemoveItem(item);
        _inventoryService.AddGold(sellValue);
        
        _eventService.Publish<ItemSoldEvent>(new ItemSoldEvent(item.Data, sellValue));
        
        return true;
    }
    
    private List<ItemData> GenerateStock(ShopData shopData)
    {
        // Generate shop inventory based on shop configuration
        return shopData?.AvailableItems?.ToList() ?? new List<ItemData>();
    }
}
```

---

## Quest System

### Purpose
Manages quest tracking, objectives, and completion.

### Quest Structure

```csharp
[GlobalClass]
public partial class QuestData : Resource
{
    [Export] public string QuestId { get; private set; }
    [Export] public InfoData Info { get; private set; }
    [Export] public QuestObjective[] Objectives { get; private set; }
    [Export] public QuestReward[] Rewards { get; private set; }
    [Export] public string[] PrerequisiteQuests { get; private set; }
}

[GlobalClass]
public partial class QuestObjective : Resource
{
    [Export] public string ObjectiveId { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public ObjectiveType Type { get; private set; }
    [Export] public string TargetId { get; private set; } // Enemy ID, Item ID, etc.
    [Export] public int RequiredCount { get; private set; } = 1;
}

public enum ObjectiveType : byte
{
    Kill = 0,        // Kill X enemies
    Collect = 1,     // Collect X items
    Explore = 2,     // Reach location
    Talk = 3,        // Talk to NPC
    Deliver = 4,     // Deliver item to NPC
    Boss = 5         // Defeat boss
}
```

### Implementation

```csharp
namespace System;

[GlobalClass]
public partial class QuestSystem : Node, ISystem
{
    private Dictionary<string, QuestProgress> _activeQuests = new();
    private HashSet<string> _completedQuests = new();
    
    private IEventService _eventService;
    private IQuestService _questService;
    
    public override void _Ready()
    {
        _eventService = ServiceProvider.EventService();
        _questService = ServiceProvider.QuestService();
        
        // Subscribe to relevant events
        _eventService.Subscribe<DeathEvent>(OnEntityDeath);
        _eventService.Subscribe<ItemPickupEvent>(OnItemPickup);
    }
    
    /// <summary>
    /// Accept a new quest.
    /// </summary>
    public bool AcceptQuest(QuestData quest)
    {
        if (quest == null) return false;
        if (_activeQuests.ContainsKey(quest.QuestId)) return false;
        if (_completedQuests.Contains(quest.QuestId)) return false;
        
        // Check prerequisites
        if (quest.PrerequisiteQuests != null)
        {
            foreach (var prereq in quest.PrerequisiteQuests)
            {
                if (!_completedQuests.Contains(prereq))
                    return false;
            }
        }
        
        // Initialize quest progress
        var progress = new QuestProgress(quest);
        _activeQuests[quest.QuestId] = progress;
        
        _eventService.Publish<QuestAcceptedEvent>(new QuestAcceptedEvent(quest));
        
        return true;
    }
    
    /// <summary>
    /// Check and update quest objective progress.
    /// </summary>
    public void UpdateObjective(string questId, string objectiveId, int progress)
    {
        if (!_activeQuests.TryGetValue(questId, out var quest))
            return;
        
        quest.UpdateObjective(objectiveId, progress);
        
        // Check if quest is complete
        if (quest.IsComplete)
        {
            CompleteQuest(questId);
        }
    }
    
    /// <summary>
    /// Complete a quest and grant rewards.
    /// </summary>
    private void CompleteQuest(string questId)
    {
        if (!_activeQuests.TryGetValue(questId, out var progress))
            return;
        
        var quest = progress.QuestData;
        
        // Grant rewards
        foreach (var reward in quest.Rewards)
        {
            GrantReward(reward);
        }
        
        // Move to completed
        _activeQuests.Remove(questId);
        _completedQuests.Add(questId);
        
        _eventService.Publish<QuestCompletedEvent>(new QuestCompletedEvent(quest));
    }
    
    private void GrantReward(QuestReward reward)
    {
        switch (reward.Type)
        {
            case RewardType.Experience:
                _questService.AddExperience(reward.Amount);
                break;
            case RewardType.Gold:
                _questService.AddGold(reward.Amount);
                break;
            case RewardType.Item:
                _questService.AddItem(reward.ItemReward);
                break;
        }
    }
    
    private void OnEntityDeath(IEvent eventData)
    {
        if (eventData is not DeathEvent death) return;
        if (death.Entity is not EnemyEntity enemy) return;
        
        // Check kill objectives
        foreach (var quest in _activeQuests.Values)
        {
            foreach (var objective in quest.Objectives)
            {
                if (objective.Type == ObjectiveType.Kill && 
                    objective.TargetId == enemy.Data.Info.Named)
                {
                    quest.IncrementObjective(objective.ObjectiveId);
                }
            }
        }
    }
    
    private void OnItemPickup(IEvent eventData)
    {
        if (eventData is not ItemPickupEvent pickup) return;
        
        // Check collect objectives
        foreach (var quest in _activeQuests.Values)
        {
            foreach (var objective in quest.Objectives)
            {
                if (objective.Type == ObjectiveType.Collect && 
                    objective.TargetId == pickup.Item.Data.Info.Named)
                {
                    quest.IncrementObjective(objective.ObjectiveId);
                }
            }
        }
    }
}

public class QuestProgress
{
    public QuestData QuestData { get; }
    public Dictionary<string, int> ObjectiveProgress { get; } = new();
    public QuestObjective[] Objectives => QuestData.Objectives;
    
    public bool IsComplete => Objectives.All(o => 
        ObjectiveProgress.TryGetValue(o.ObjectiveId, out var p) && 
        p >= o.RequiredCount);
    
    public QuestProgress(QuestData quest)
    {
        QuestData = quest;
        foreach (var obj in quest.Objectives)
        {
            ObjectiveProgress[obj.ObjectiveId] = 0;
        }
    }
    
    public void UpdateObjective(string objectiveId, int progress)
    {
        if (ObjectiveProgress.ContainsKey(objectiveId))
            ObjectiveProgress[objectiveId] = progress;
    }
    
    public void IncrementObjective(string objectiveId, int amount = 1)
    {
        if (ObjectiveProgress.ContainsKey(objectiveId))
            ObjectiveProgress[objectiveId] += amount;
    }
}
```

---

## Related Documents

- [00-game-design-document.md](00-game-design-document.md) - Game design overview
- [01-technical-architecture.md](01-technical-architecture.md) - System architecture
- [03-component-design.md](03-component-design.md) - Component specifications
- [04-data-structures.md](04-data-structures.md) - Data resource definitions
- [05-implementation-roadmap.md](05-implementation-roadmap.md) - Development phases
