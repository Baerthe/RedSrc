# Component Design Document
## RedGate: Tactical Dungeon RPG

> This document specifies all modular components that attach to entities.

---

## Component Design Principles

### Core Philosophy

1. **Data Containers**: Components store state; systems process logic
2. **No Process Loops**: `_Process` and `_PhysicsProcess` avoided in components
3. **Self-Validation**: `_GetConfigurationWarnings()` for editor feedback
4. **System Registration**: Components register with systems on tree entry
5. **Godot-First**: Extend existing Godot nodes when applicable

### Base Interface

```csharp
namespace Interface;

/// <summary>
/// Base interface for all components.
/// Components are data containers that define entity characteristics.
/// </summary>
public interface IComponent
{
    /// <summary>
    /// Called when the component is initialized with entity data.
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Called when the component is reset (e.g., entity pooling).
    /// </summary>
    void Reset();
}
```

---

## Health & Resource Components

### HealthComponent

Manages entity health, damage, and death state.

```csharp
namespace Component;

/// <summary>
/// Data container for entity health and vitality.
/// </summary>
[GlobalClass]
public partial class HealthComponent : Node, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Health")]
    [Export] public int MaxHealth { get; set; } = 100;
    [Export] public int CurrentHealth { get; set; } = 100;
    
    [ExportCategory("Regeneration")]
    [Export] public float HealthRegen { get; set; } = 0f;
    [Export] public float RegenInterval { get; set; } = 1f;
    
    [ExportCategory("Damage Modifiers")]
    [Export] public float DamageReduction { get; set; } = 0f;
    [Export] public bool IsInvulnerable { get; set; } = false;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>True if health is depleted.</summary>
    public bool IsDead => CurrentHealth <= 0;
    
    /// <summary>Health as percentage (0.0 to 1.0).</summary>
    public float HealthPercent => MaxHealth > 0 ? (float)CurrentHealth / MaxHealth : 0f;
    
    /// <summary>Time since last damage taken.</summary>
    public float TimeSinceDamage { get; set; } = 0f;
    
    /// <summary>Flag for recent damage (for UI feedback).</summary>
    public bool WasRecentlyDamaged => TimeSinceDamage < 0.5f;
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        CurrentHealth = MaxHealth;
        TimeSinceDamage = float.MaxValue;
    }
    
    public void Reset()
    {
        CurrentHealth = MaxHealth;
        TimeSinceDamage = float.MaxValue;
        IsInvulnerable = false;
    }
    
    #endregion
    
    #region Editor Validation
    
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        
        if (MaxHealth <= 0)
            warnings.Add("MaxHealth must be greater than 0.");
        
        if (HealthRegen < 0)
            warnings.Add("HealthRegen should not be negative.");
        
        return warnings.ToArray();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Apply damage to this component. Returns actual damage dealt.
    /// </summary>
    public int TakeDamage(int amount)
    {
        if (IsInvulnerable || IsDead)
            return 0;
        
        int actualDamage = Mathf.Max(0, (int)(amount * (1f - DamageReduction)));
        CurrentHealth = Mathf.Max(0, CurrentHealth - actualDamage);
        TimeSinceDamage = 0f;
        
        return actualDamage;
    }
    
    /// <summary>
    /// Heal this component. Returns actual healing done.
    /// </summary>
    public int Heal(int amount)
    {
        if (IsDead)
            return 0;
        
        int previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        
        return CurrentHealth - previousHealth;
    }
    
    /// <summary>
    /// Set health to maximum.
    /// </summary>
    public void FullHeal()
    {
        CurrentHealth = MaxHealth;
    }
    
    #endregion
}
```

### ManaComponent

Manages entity mana for ability casting.

```csharp
namespace Component;

/// <summary>
/// Data container for entity mana and magical resources.
/// </summary>
[GlobalClass]
public partial class ManaComponent : Node, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Mana")]
    [Export] public int MaxMana { get; set; } = 50;
    [Export] public int CurrentMana { get; set; } = 50;
    
    [ExportCategory("Regeneration")]
    [Export] public float ManaRegen { get; set; } = 1f;
    [Export] public float RegenInterval { get; set; } = 1f;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>Mana as percentage (0.0 to 1.0).</summary>
    public float ManaPercent => MaxMana > 0 ? (float)CurrentMana / MaxMana : 0f;
    
    /// <summary>Time accumulator for regeneration.</summary>
    public float RegenAccumulator { get; set; } = 0f;
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        CurrentMana = MaxMana;
        RegenAccumulator = 0f;
    }
    
    public void Reset()
    {
        CurrentMana = MaxMana;
        RegenAccumulator = 0f;
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Check if there's enough mana to cast.
    /// </summary>
    public bool CanCast(int cost) => CurrentMana >= cost;
    
    /// <summary>
    /// Spend mana. Returns true if successful.
    /// </summary>
    public bool SpendMana(int cost)
    {
        if (!CanCast(cost))
            return false;
        
        CurrentMana = Mathf.Max(0, CurrentMana - cost);
        return true;
    }
    
    /// <summary>
    /// Restore mana.
    /// </summary>
    public int RestoreMana(int amount)
    {
        int previous = CurrentMana;
        CurrentMana = Mathf.Min(MaxMana, CurrentMana + amount);
        return CurrentMana - previous;
    }
    
    /// <summary>
    /// Set mana to maximum.
    /// </summary>
    public void FullRestore()
    {
        CurrentMana = MaxMana;
    }
    
    #endregion
}
```

### StaminaComponent

Manages stamina for physical actions (optional system).

```csharp
namespace Component;

/// <summary>
/// Data container for entity stamina (dodge, sprint, etc.).
/// </summary>
[GlobalClass]
public partial class StaminaComponent : Node, IComponent
{
    [Export] public float MaxStamina { get; set; } = 100f;
    [Export] public float CurrentStamina { get; set; } = 100f;
    [Export] public float StaminaRegen { get; set; } = 20f;
    [Export] public float DodgeCost { get; set; } = 25f;
    [Export] public float SprintCostPerSecond { get; set; } = 10f;
    
    public float StaminaPercent => MaxStamina > 0 ? CurrentStamina / MaxStamina : 0f;
    public bool CanDodge => CurrentStamina >= DodgeCost;
    public bool CanSprint => CurrentStamina > 0;
    
    public void Initialize() => CurrentStamina = MaxStamina;
    public void Reset() => CurrentStamina = MaxStamina;
    
    public bool ConsumeDodge()
    {
        if (!CanDodge) return false;
        CurrentStamina -= DodgeCost;
        return true;
    }
}
```

---

## Movement & Physics Components

### PhysicsComponent

Handles entity movement and collision as a CharacterBody2D.

```csharp
namespace Component;

/// <summary>
/// Physics component for entity movement using CharacterBody2D.
/// Extends CharacterBody2D to integrate with Godot's physics.
/// </summary>
[GlobalClass]
public partial class PhysicsComponent : CharacterBody2D, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Movement")]
    [Export] public float Speed { get; set; } = 100f;
    [Export] public float Acceleration { get; set; } = 500f;
    [Export] public float Friction { get; set; } = 800f;
    
    [ExportCategory("Sprint")]
    [Export] public float SprintMultiplier { get; set; } = 1.5f;
    [Export] public bool IsSprinting { get; set; } = false;
    
    [ExportCategory("State")]
    [Export] public bool CanMove { get; set; } = true;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>Target velocity set by input or AI.</summary>
    public Vector2 TargetVelocity { get; set; } = Vector2.Zero;
    
    /// <summary>Current facing direction.</summary>
    public Vector2 FacingDirection { get; set; } = Vector2.Down;
    
    /// <summary>Direction as enum for animation.</summary>
    public Direction4 FacingDirection4 => GetDirection4(FacingDirection);
    
    /// <summary>True if entity is currently moving.</summary>
    public bool IsMoving => Velocity.Length() > 1f;
    
    /// <summary>Current effective speed (with modifiers).</summary>
    public float EffectiveSpeed => Speed * (IsSprinting ? SprintMultiplier : 1f);
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        Velocity = Vector2.Zero;
        TargetVelocity = Vector2.Zero;
        FacingDirection = Vector2.Down;
        CanMove = true;
    }
    
    public void Reset()
    {
        Velocity = Vector2.Zero;
        TargetVelocity = Vector2.Zero;
        FacingDirection = Vector2.Down;
        CanMove = true;
        IsSprinting = false;
    }
    
    #endregion
    
    #region Editor Validation
    
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        
        if (Speed <= 0)
            warnings.Add("Speed must be greater than 0.");
        
        // Check for CollisionShape2D child
        bool hasCollision = false;
        foreach (var child in GetChildren())
        {
            if (child is CollisionShape2D)
            {
                hasCollision = true;
                break;
            }
        }
        
        if (!hasCollision)
            warnings.Add("PhysicsComponent requires a CollisionShape2D child.");
        
        return warnings.ToArray();
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Set movement direction (normalized input).
    /// </summary>
    public void SetMovementInput(Vector2 input)
    {
        if (!CanMove)
        {
            TargetVelocity = Vector2.Zero;
            return;
        }
        
        TargetVelocity = input.Normalized() * EffectiveSpeed;
        
        if (input.Length() > 0.1f)
            FacingDirection = input.Normalized();
    }
    
    /// <summary>
    /// Stop all movement immediately.
    /// </summary>
    public void StopMovement()
    {
        TargetVelocity = Vector2.Zero;
        Velocity = Vector2.Zero;
    }
    
    private static Direction4 GetDirection4(Vector2 direction)
    {
        if (direction == Vector2.Zero)
            return Direction4.Down;
        
        float angle = direction.Angle();
        
        // Convert to 0-360 range
        if (angle < 0) angle += Mathf.Tau;
        
        // 8 sectors of 45 degrees each
        if (angle < Mathf.Pi * 0.25f || angle >= Mathf.Pi * 1.75f)
            return Direction4.Right;
        if (angle < Mathf.Pi * 0.75f)
            return Direction4.Down;
        if (angle < Mathf.Pi * 1.25f)
            return Direction4.Left;
        return Direction4.Up;
    }
    
    #endregion
}

public enum Direction4 : byte
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}
```

### KnockbackComponent

Handles knockback effects from combat.

```csharp
namespace Component;

/// <summary>
/// Handles knockback physics when entity takes damage.
/// </summary>
[GlobalClass]
public partial class KnockbackComponent : Node, IComponent
{
    [Export] public float KnockbackResistance { get; set; } = 0f;
    [Export] public float KnockbackDecay { get; set; } = 10f;
    
    public Vector2 KnockbackVelocity { get; set; } = Vector2.Zero;
    public bool IsKnockedBack => KnockbackVelocity.Length() > 1f;
    
    public void Initialize() => KnockbackVelocity = Vector2.Zero;
    public void Reset() => KnockbackVelocity = Vector2.Zero;
    
    public void ApplyKnockback(Vector2 direction, float force)
    {
        float actualForce = force * (1f - KnockbackResistance);
        KnockbackVelocity = direction.Normalized() * actualForce;
    }
}
```

---

## Combat Components

### CombatComponent

Stores combat-related stats and state.

```csharp
namespace Component;

/// <summary>
/// Data container for entity combat statistics.
/// </summary>
[GlobalClass]
public partial class CombatComponent : Node, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Offense")]
    [Export] public int BaseDamage { get; set; } = 10;
    [Export] public float AttackSpeed { get; set; } = 1f;
    [Export] public float AttackRange { get; set; } = 50f;
    [Export] public DamageType DamageType { get; set; } = DamageType.Physical;
    
    [ExportCategory("Critical")]
    [Export] public float CritChance { get; set; } = 0.05f;
    [Export] public float CritMultiplier { get; set; } = 1.5f;
    
    [ExportCategory("Defense")]
    [Export] public int Armor { get; set; } = 0;
    [Export] public int MagicResist { get; set; } = 0;
    
    [ExportCategory("Modifiers")]
    [Export] public float DamageBonus { get; set; } = 0f;
    [Export] public float AttackSpeedBonus { get; set; } = 0f;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>Time remaining until next attack.</summary>
    public float AttackCooldown { get; set; } = 0f;
    
    /// <summary>True if can perform attack.</summary>
    public bool CanAttack => AttackCooldown <= 0f;
    
    /// <summary>Current target (for AI).</summary>
    public Node2D Target { get; set; }
    
    /// <summary>Effective attack speed with bonuses.</summary>
    public float EffectiveAttackSpeed => AttackSpeed * (1f + AttackSpeedBonus);
    
    /// <summary>Effective damage with bonuses.</summary>
    public int EffectiveDamage => (int)(BaseDamage * (1f + DamageBonus));
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        AttackCooldown = 0f;
        Target = null;
    }
    
    public void Reset()
    {
        AttackCooldown = 0f;
        Target = null;
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Start attack cooldown.
    /// </summary>
    public void StartCooldown()
    {
        AttackCooldown = 1f / EffectiveAttackSpeed;
    }
    
    /// <summary>
    /// Calculate armor reduction percentage.
    /// </summary>
    public float GetArmorReduction()
    {
        return Armor / (float)(Armor + 100);
    }
    
    /// <summary>
    /// Calculate magic resistance percentage.
    /// </summary>
    public float GetMagicReduction()
    {
        return MagicResist / (float)(MagicResist + 100);
    }
    
    #endregion
}

public enum DamageType : byte
{
    Physical = 0,
    Magical = 1,
    Fire = 2,
    Ice = 3,
    Lightning = 4,
    Poison = 5,
    Holy = 6,
    Dark = 7,
    True = 8  // Ignores armor/resistance
}
```

### AbilityComponent

Manages hero abilities and cooldowns.

```csharp
namespace Component;

/// <summary>
/// Data container for entity abilities.
/// </summary>
[GlobalClass]
public partial class AbilityComponent : Node, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Abilities")]
    [Export] public AbilityData[] Abilities { get; set; } = new AbilityData[4];
    [Export] public AbilityData UltimateAbility { get; set; }
    
    [ExportCategory("Modifiers")]
    [Export] public float CooldownReduction { get; set; } = 0f;
    [Export] public float AbilityPowerBonus { get; set; } = 0f;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>Cooldown timers per slot (0-3 for normal, 4 for ultimate).</summary>
    public Dictionary<int, float> Cooldowns { get; } = new()
    {
        { 0, 0f }, { 1, 0f }, { 2, 0f }, { 3, 0f }, { 4, 0f }
    };
    
    /// <summary>Currently channeling ability slot (-1 if none).</summary>
    public int ChannelingSlot { get; set; } = -1;
    
    /// <summary>Channel progress time.</summary>
    public float ChannelProgress { get; set; } = 0f;
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        foreach (var key in Cooldowns.Keys.ToList())
            Cooldowns[key] = 0f;
        ChannelingSlot = -1;
        ChannelProgress = 0f;
    }
    
    public void Reset()
    {
        Initialize();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Check if ability slot is on cooldown.
    /// </summary>
    public bool IsOnCooldown(int slot)
    {
        return Cooldowns.TryGetValue(slot, out var cd) && cd > 0f;
    }
    
    /// <summary>
    /// Get remaining cooldown for slot.
    /// </summary>
    public float GetCooldown(int slot)
    {
        return Cooldowns.TryGetValue(slot, out var cd) ? cd : 0f;
    }
    
    /// <summary>
    /// Get cooldown percentage (0.0 = ready, 1.0 = just used).
    /// </summary>
    public float GetCooldownPercent(int slot)
    {
        var ability = GetAbility(slot);
        if (ability == null) return 0f;
        
        float maxCd = ability.Cooldown * (1f - CooldownReduction);
        float currentCd = GetCooldown(slot);
        
        return maxCd > 0 ? currentCd / maxCd : 0f;
    }
    
    /// <summary>
    /// Start cooldown for ability slot.
    /// </summary>
    public void StartCooldown(int slot)
    {
        var ability = GetAbility(slot);
        if (ability == null) return;
        
        Cooldowns[slot] = ability.Cooldown * (1f - CooldownReduction);
    }
    
    /// <summary>
    /// Get ability from slot (4 = ultimate).
    /// </summary>
    public AbilityData GetAbility(int slot)
    {
        if (slot == 4) return UltimateAbility;
        if (slot >= 0 && slot < Abilities.Length) return Abilities[slot];
        return null;
    }
    
    /// <summary>
    /// Check if currently channeling.
    /// </summary>
    public bool IsChanneling => ChannelingSlot >= 0;
    
    #endregion
}
```

### HitboxComponent

Defines attack hitbox area.

```csharp
namespace Component;

/// <summary>
/// Attack hitbox for dealing damage to enemies.
/// </summary>
[GlobalClass]
public partial class HitboxComponent : Area2D, IComponent
{
    [Export] public bool IsActive { get; set; } = false;
    [Export] public int Damage { get; set; } = 10;
    [Export] public DamageType DamageType { get; set; } = DamageType.Physical;
    [Export] public float KnockbackForce { get; set; } = 100f;
    
    public IEntity Owner { get; set; }
    
    public void Initialize() => IsActive = false;
    public void Reset() => IsActive = false;
    
    public void Activate(float duration = 0.2f)
    {
        IsActive = true;
        GetTree().CreateTimer(duration).Timeout += () => IsActive = false;
    }
}
```

### HurtboxComponent

Defines damageable collision area.

```csharp
namespace Component;

/// <summary>
/// Hurtbox for receiving damage from attacks.
/// </summary>
[GlobalClass]
public partial class HurtboxComponent : Area2D, IComponent
{
    [Export] public bool IsActive { get; set; } = true;
    
    public IEntity Owner { get; set; }
    public HealthComponent Health { get; set; }
    
    public void Initialize() => IsActive = true;
    public void Reset() => IsActive = true;
    
    public override void _Ready()
    {
        // Try to find HealthComponent on parent
        var parent = GetParent<Node2D>();
        Health = parent?.GetNodeOrNull<HealthComponent>("Health");
    }
}
```

---

## AI Components

### AIComponent

Stores AI state and behavior configuration.

```csharp
namespace Component;

/// <summary>
/// Data container for enemy AI behavior.
/// </summary>
[GlobalClass]
public partial class AIComponent : Node, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Behavior")]
    [Export] public AIBehavior Behavior { get; set; } = AIBehavior.Aggressive;
    [Export] public float DetectionRange { get; set; } = 200f;
    [Export] public float AttackRange { get; set; } = 50f;
    [Export] public float FleeHealthPercent { get; set; } = 0.2f;
    
    [ExportCategory("Patrol")]
    [Export] public Vector2[] PatrolPoints { get; set; }
    [Export] public float PatrolWaitTime { get; set; } = 2f;
    
    [ExportCategory("Leash")]
    [Export] public bool HasLeash { get; set; } = false;
    [Export] public float LeashRange { get; set; } = 500f;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>Current AI state.</summary>
    public AIState CurrentState { get; set; } = AIState.Idle;
    
    /// <summary>Previous AI state (for state transitions).</summary>
    public AIState PreviousState { get; set; } = AIState.Idle;
    
    /// <summary>Current target entity.</summary>
    public Node2D Target { get; set; }
    
    /// <summary>Time in current state.</summary>
    public float StateTime { get; set; } = 0f;
    
    /// <summary>Current patrol point index.</summary>
    public int CurrentPatrolIndex { get; set; } = 0;
    
    /// <summary>Home position (for leash).</summary>
    public Vector2 HomePosition { get; set; }
    
    /// <summary>Path to follow (from pathfinding).</summary>
    public Vector2[] CurrentPath { get; set; }
    
    /// <summary>Current path index.</summary>
    public int PathIndex { get; set; } = 0;
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        CurrentState = AIState.Idle;
        PreviousState = AIState.Idle;
        Target = null;
        StateTime = 0f;
        CurrentPatrolIndex = 0;
        PathIndex = 0;
    }
    
    public void Reset()
    {
        Initialize();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Transition to a new state.
    /// </summary>
    public void TransitionTo(AIState newState)
    {
        if (CurrentState == newState) return;
        
        PreviousState = CurrentState;
        CurrentState = newState;
        StateTime = 0f;
    }
    
    /// <summary>
    /// Check if target is in detection range.
    /// </summary>
    public bool IsTargetInDetection(Node2D self)
    {
        if (Target == null) return false;
        return self.GlobalPosition.DistanceTo(Target.GlobalPosition) <= DetectionRange;
    }
    
    /// <summary>
    /// Check if target is in attack range.
    /// </summary>
    public bool IsTargetInAttack(Node2D self)
    {
        if (Target == null) return false;
        return self.GlobalPosition.DistanceTo(Target.GlobalPosition) <= AttackRange;
    }
    
    /// <summary>
    /// Check if entity should flee based on health.
    /// </summary>
    public bool ShouldFlee(HealthComponent health)
    {
        return Behavior == AIBehavior.Coward && health.HealthPercent <= FleeHealthPercent;
    }
    
    #endregion
}

public enum AIState : byte
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Flee = 4,
    Return = 5,  // Returning to leash point
    Dead = 6
}

public enum AIBehavior : byte
{
    Aggressive = 0,   // Always chases
    Defensive = 1,    // Only when attacked
    Coward = 2,       // Flees at low health
    Ranged = 3,       // Maintains distance
    Support = 4,      // Helps allies
    Boss = 5          // Multi-phase
}
```

---

## Inventory Components

### InventoryComponent

Manages entity item storage.

```csharp
namespace Component;

/// <summary>
/// Data container for entity inventory.
/// </summary>
[GlobalClass]
public partial class InventoryComponent : Node, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Capacity")]
    [Export] public int Capacity { get; set; } = 20;
    [Export] public int GoldCapacity { get; set; } = 999999;
    
    #endregion
    
    #region Runtime State
    
    /// <summary>Stored items.</summary>
    public List<ItemInstance> Items { get; } = new();
    
    /// <summary>Equipped items by slot.</summary>
    public Dictionary<EquipSlot, ItemInstance> Equipped { get; } = new();
    
    /// <summary>Current gold.</summary>
    public int Gold { get; set; } = 0;
    
    /// <summary>Number of items.</summary>
    public int ItemCount => Items.Count;
    
    /// <summary>True if inventory is full.</summary>
    public bool IsFull => Items.Count >= Capacity;
    
    /// <summary>Available space.</summary>
    public int FreeSlots => Capacity - Items.Count;
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        Items.Clear();
        Equipped.Clear();
        Gold = 0;
    }
    
    public void Reset()
    {
        Initialize();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Try to add item to inventory.
    /// </summary>
    public bool AddItem(ItemInstance item)
    {
        if (item == null || IsFull)
            return false;
        
        // Check for stackable
        if (item.Data.IsStackable)
        {
            var existing = Items.FirstOrDefault(i => i.Data == item.Data);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                return true;
            }
        }
        
        Items.Add(item);
        return true;
    }
    
    /// <summary>
    /// Remove item from inventory.
    /// </summary>
    public bool RemoveItem(ItemInstance item)
    {
        return Items.Remove(item);
    }
    
    /// <summary>
    /// Get item at index.
    /// </summary>
    public ItemInstance GetItem(int index)
    {
        if (index < 0 || index >= Items.Count)
            return null;
        return Items[index];
    }
    
    /// <summary>
    /// Equip an item to its slot.
    /// </summary>
    public ItemInstance EquipItem(ItemInstance item)
    {
        if (item?.Data?.EquipSlot == null)
            return null;
        
        var slot = item.Data.EquipSlot;
        ItemInstance previous = null;
        
        if (Equipped.TryGetValue(slot, out previous))
        {
            Equipped.Remove(slot);
        }
        
        Equipped[slot] = item;
        Items.Remove(item);
        
        return previous;
    }
    
    /// <summary>
    /// Unequip item from slot.
    /// </summary>
    public ItemInstance UnequipSlot(EquipSlot slot)
    {
        if (!Equipped.TryGetValue(slot, out var item))
            return null;
        
        Equipped.Remove(slot);
        
        if (!IsFull)
            Items.Add(item);
        
        return item;
    }
    
    /// <summary>
    /// Check if has enough gold.
    /// </summary>
    public bool HasGold(int amount) => Gold >= amount;
    
    /// <summary>
    /// Add gold.
    /// </summary>
    public void AddGold(int amount)
    {
        Gold = Mathf.Min(Gold + amount, GoldCapacity);
    }
    
    /// <summary>
    /// Spend gold.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (!HasGold(amount)) return false;
        Gold -= amount;
        return true;
    }
    
    #endregion
}

public enum EquipSlot : byte
{
    None = 0,
    MainHand = 1,
    OffHand = 2,
    Head = 3,
    Chest = 4,
    Gloves = 5,
    Boots = 6,
    Ring1 = 7,
    Ring2 = 8,
    Amulet = 9
}
```

---

## Visual Components

### SpriteComponent

Wrapper for AnimatedSprite2D with animation helpers.

```csharp
namespace Component;

/// <summary>
/// Visual sprite component with animation helpers.
/// </summary>
[GlobalClass]
public partial class SpriteComponent : AnimatedSprite2D, IComponent
{
    #region Exported Properties
    
    [ExportCategory("Animation")]
    [Export] public string IdleAnimation { get; set; } = "idle";
    [Export] public string WalkAnimation { get; set; } = "walk";
    [Export] public string AttackAnimation { get; set; } = "attack";
    [Export] public string HurtAnimation { get; set; } = "hurt";
    [Export] public string DeathAnimation { get; set; } = "death";
    
    [ExportCategory("Directions")]
    [Export] public bool HasDirectionalAnims { get; set; } = true;
    
    #endregion
    
    #region Runtime State
    
    public string CurrentAnimationBase { get; private set; } = "idle";
    public Direction4 CurrentDirection { get; set; } = Direction4.Down;
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        CurrentDirection = Direction4.Down;
        PlayAnimation(IdleAnimation);
    }
    
    public void Reset()
    {
        Initialize();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Play animation with optional direction suffix.
    /// </summary>
    public void PlayAnimation(string animName)
    {
        CurrentAnimationBase = animName;
        
        string fullName = HasDirectionalAnims 
            ? $"{animName}_{GetDirectionSuffix(CurrentDirection)}" 
            : animName;
        
        if (SpriteFrames?.HasAnimation(fullName) == true)
        {
            Play(fullName);
        }
        else if (SpriteFrames?.HasAnimation(animName) == true)
        {
            Play(animName);
        }
    }
    
    /// <summary>
    /// Update facing direction.
    /// </summary>
    public void SetDirection(Direction4 direction)
    {
        if (CurrentDirection == direction) return;
        
        CurrentDirection = direction;
        PlayAnimation(CurrentAnimationBase);
    }
    
    /// <summary>
    /// Update facing direction from vector.
    /// </summary>
    public void SetDirectionFromVector(Vector2 direction)
    {
        if (direction.Length() < 0.1f) return;
        
        SetDirection(VectorToDirection4(direction));
    }
    
    /// <summary>
    /// Play idle animation.
    /// </summary>
    public void PlayIdle() => PlayAnimation(IdleAnimation);
    
    /// <summary>
    /// Play walk animation.
    /// </summary>
    public void PlayWalk() => PlayAnimation(WalkAnimation);
    
    /// <summary>
    /// Play attack animation.
    /// </summary>
    public void PlayAttack() => PlayAnimation(AttackAnimation);
    
    /// <summary>
    /// Play hurt animation.
    /// </summary>
    public void PlayHurt() => PlayAnimation(HurtAnimation);
    
    /// <summary>
    /// Play death animation.
    /// </summary>
    public void PlayDeath() => PlayAnimation(DeathAnimation);
    
    private static string GetDirectionSuffix(Direction4 dir)
    {
        return dir switch
        {
            Direction4.Up => "up",
            Direction4.Right => "right",
            Direction4.Down => "down",
            Direction4.Left => "left",
            _ => "down"
        };
    }
    
    private static Direction4 VectorToDirection4(Vector2 v)
    {
        if (Mathf.Abs(v.X) > Mathf.Abs(v.Y))
            return v.X > 0 ? Direction4.Right : Direction4.Left;
        return v.Y > 0 ? Direction4.Down : Direction4.Up;
    }
    
    #endregion
}
```

---

## Interaction Components

### InteractionComponent

Enables entity to be interacted with.

```csharp
namespace Component;

/// <summary>
/// Makes entity interactable by player.
/// </summary>
[GlobalClass]
public partial class InteractionComponent : Area2D, IComponent
{
    [Export] public bool IsInteractable { get; set; } = true;
    [Export] public string InteractionPrompt { get; set; } = "Interact";
    [Export] public float InteractionRange { get; set; } = 32f;
    
    public bool IsPlayerInRange { get; set; } = false;
    public IEntity Owner { get; set; }
    
    public void Initialize() => IsInteractable = true;
    public void Reset() => IsInteractable = true;
}
```

---

## Effect Components

### EffectReceiverComponent

Manages active status effects on entity.

```csharp
namespace Component;

/// <summary>
/// Tracks active status effects on entity.
/// </summary>
[GlobalClass]
public partial class EffectReceiverComponent : Node, IComponent
{
    #region Runtime State
    
    /// <summary>Currently active effects.</summary>
    public List<ActiveEffect> ActiveEffects { get; } = new();
    
    /// <summary>Effect immunities.</summary>
    public HashSet<EffectType> Immunities { get; } = new();
    
    #endregion
    
    #region IComponent Implementation
    
    public void Initialize()
    {
        ActiveEffects.Clear();
        Immunities.Clear();
    }
    
    public void Reset()
    {
        Initialize();
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Apply an effect to this entity.
    /// </summary>
    public bool ApplyEffect(EffectData effectData, IEntity source)
    {
        if (effectData == null) return false;
        if (Immunities.Contains(effectData.Type)) return false;
        
        // Check for existing effect
        var existing = ActiveEffects.FirstOrDefault(e => e.Data == effectData);
        if (existing != null)
        {
            // Refresh or stack
            if (effectData.Stackable)
            {
                existing.Stacks = Mathf.Min(existing.Stacks + 1, effectData.MaxStacks);
            }
            existing.RemainingDuration = effectData.Duration;
            return true;
        }
        
        // Add new effect
        ActiveEffects.Add(new ActiveEffect
        {
            Data = effectData,
            Source = source,
            RemainingDuration = effectData.Duration,
            Stacks = 1
        });
        
        return true;
    }
    
    /// <summary>
    /// Remove specific effect.
    /// </summary>
    public void RemoveEffect(EffectData effectData)
    {
        ActiveEffects.RemoveAll(e => e.Data == effectData);
    }
    
    /// <summary>
    /// Clear all effects.
    /// </summary>
    public void ClearAllEffects()
    {
        ActiveEffects.Clear();
    }
    
    /// <summary>
    /// Check if has effect type.
    /// </summary>
    public bool HasEffect(EffectType type)
    {
        return ActiveEffects.Any(e => e.Data.Type == type);
    }
    
    #endregion
}

public class ActiveEffect
{
    public EffectData Data { get; set; }
    public IEntity Source { get; set; }
    public float RemainingDuration { get; set; }
    public int Stacks { get; set; }
    public float TickAccumulator { get; set; }
}

public enum EffectType : byte
{
    Buff = 0,
    Debuff = 1,
    DoT = 2,      // Damage over time
    HoT = 3,      // Heal over time
    Stun = 4,
    Slow = 5,
    Haste = 6,
    Shield = 7,
    Poison = 8,
    Burn = 9,
    Freeze = 10,
    Silence = 11
}
```

---

## Component Scene Templates

### Hero Entity Template

```
HeroEntity.tscn
├── HeroEntity (Node2D with HeroEntity.cs)
│   ├── Physics (PhysicsComponent)
│   │   └── CollisionShape2D
│   ├── Health (HealthComponent)
│   ├── Mana (ManaComponent)
│   ├── Combat (CombatComponent)
│   ├── Abilities (AbilityComponent)
│   ├── Inventory (InventoryComponent)
│   ├── Effects (EffectReceiverComponent)
│   ├── Sprite (SpriteComponent)
│   ├── Hurtbox (HurtboxComponent)
│   │   └── CollisionShape2D
│   └── Interaction (InteractionComponent)
│       └── CollisionShape2D
```

### Enemy Entity Template

```
EnemyEntity.tscn
├── EnemyEntity (Node2D with EnemyEntity.cs)
│   ├── Physics (PhysicsComponent)
│   │   └── CollisionShape2D
│   ├── Health (HealthComponent)
│   ├── Combat (CombatComponent)
│   ├── AI (AIComponent)
│   ├── Effects (EffectReceiverComponent)
│   ├── Sprite (SpriteComponent)
│   ├── Hurtbox (HurtboxComponent)
│   │   └── CollisionShape2D
│   └── Hitbox (HitboxComponent)
│       └── CollisionShape2D
```

---

## Related Documents

- [00-game-design-document.md](00-game-design-document.md) - Game design overview
- [01-technical-architecture.md](01-technical-architecture.md) - System architecture
- [02-systems-design.md](02-systems-design.md) - System specifications
- [04-data-structures.md](04-data-structures.md) - Data resource definitions
- [05-implementation-roadmap.md](05-implementation-roadmap.md) - Development phases
