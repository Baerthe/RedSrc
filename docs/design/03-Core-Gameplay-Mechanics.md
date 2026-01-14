# Core Gameplay Mechanics

## Overview

This document details the moment-to-moment gameplay mechanics that define the player experience in the RTS game.

## Player Input and Control

### Camera Control

#### Movement
- **WASD Keys**: Pan camera
- **Arrow Keys**: Alternative panning
- **Mouse Edge Scrolling**: Move cursor to screen edge to scroll
- **Middle Mouse Drag**: Pan by dragging
- **Scroll Wheel**: Zoom in/out

#### Constraints
- Minimum zoom: Show 20x20 tile area
- Maximum zoom: Show 60x60 tile area  
- Camera bounded to map edges
- Smooth interpolation for movements

### Unit Selection

#### Single Selection
- **Left Click**: Select single unit
- **Click on Selected**: Cycle through stacked units
- **Click Empty Space**: Deselect all

#### Box Selection
- **Click + Drag**: Create selection rectangle
- **Select Multiple**: All units within box selected
- **Additive Selection**: Hold Shift to add to selection
- **Subtractive Selection**: Hold Ctrl to remove from selection

#### Advanced Selection
- **Double Click Unit**: Select all visible units of same type
- **Ctrl + 1-9**: Assign units to control group
- **1-9**: Select control group
- **Shift + 1-9**: Add to control group
- **Select All Units**: Ctrl+A (all player units on screen)

#### Selection Limits
- Maximum 12 units per selection (classic RTS style)
- Display unit count when exceeding visible icons
- Smart selection priorities (combat units over workers)

### Unit Commands

#### Movement Commands
- **Right Click Ground**: Move to location
- **M + Click**: Force move command (ignore enemies)
- **Shift + Command**: Queue command
- **Shift + Right Click Path**: Create waypoints

#### Attack Commands
- **A + Click**: Attack-move to location
- **Right Click Enemy**: Direct attack target
- **Ctrl + Click**: Attack ground (for siege units)
- **P + Click**: Patrol between points

#### Special Commands
- **S**: Stop current action
- **H**: Hold position (don't auto-retaliate)
- **G**: Guard target unit/building

### Building Commands

#### Placement
- **Click Building Button**: Enter placement mode
- **Move Mouse**: Preview building location
- **Green Overlay**: Valid placement
- **Red Overlay**: Invalid placement
- **Left Click**: Confirm placement
- **Right Click/Esc**: Cancel placement

#### Construction
- **Worker Auto-Build**: Workers automatically construct
- **Multiple Workers**: Speed up construction
- **Cancel Construction**: Refund partial resources

## Resource Management

### Resource Types

#### Gold
- **Sources**: Gold mines, enemy buildings
- **Uses**: Unit production, building construction, upgrades
- **Gathering Rate**: 10 gold per trip
- **Deposit**: Town Hall, Keep, Castle

#### Wood  
- **Sources**: Trees, lumber mills
- **Uses**: Building construction, some units, upgrades
- **Gathering Rate**: 5 wood per trip
- **Deposit**: Town Hall, Keep, Castle

#### Supply
- **Function**: Population cap for units
- **Maximum**: 200 supply
- **Provided By**: Supply buildings (farms, barracks, etc.)
- **Unit Costs**: Workers: 1, Basic units: 1-2, Advanced units: 3-5

### Resource Gathering

#### Worker Mechanics
1. **Assignment**: Right-click resource
2. **Pathfinding**: Navigate to resource
3. **Gathering**: Animation plays, resource collected
4. **Return**: Auto-path to nearest depot
5. **Repeat**: Automatically return to resource

#### Optimization
- Workers automatically find nearest depot
- Smart resource targeting (closest available)
- Gathering interrupted by attacks (can be set to ignore)
- Automatic re-assignment if resource depleted

### Economy Expansion

#### Phases
1. **Early Game**: 5-8 workers on resources
2. **Mid Game**: 12-16 workers per base
3. **Late Game**: Multiple bases, 30+ total workers

#### Base Management
- Optimal: 2 bases by minute 10
- Maximum: 4 simultaneous bases (engine limit)
- Resource denial: Destroy enemy expansions

## Unit Mechanics

### Unit Properties

#### Core Stats
- **Health**: Hit points before destruction
- **Armor**: Reduces incoming damage
- **Damage**: Attack strength
- **Attack Speed**: Time between attacks
- **Attack Range**: Maximum attack distance
- **Movement Speed**: Tiles per second
- **Vision Range**: Tiles visible in fog of war
- **Build Time**: Production duration
- **Cost**: Resource requirements

#### Derived Stats
- **DPS**: Damage Per Second (damage / attack speed)
- **Effective HP**: Health × (1 + armor modifier)
- **Cost Efficiency**: Combat value per resource spent

### Unit States

#### Idle State
- Default state when no commands
- Auto-acquire targets if set to aggressive
- Display idle animations
- Respond to nearby combat

#### Moving State
- Following pathfinding route
- Collision avoidance active
- Auto-attack if aggressive stance
- Formation maintenance in groups

#### Attacking State
- Facing target
- Attack animation playing
- Damage dealt on animation frame
- Auto-pursue fleeing targets (if aggressive)

#### Gathering State (Workers)
- Moving to resource
- Gathering animation
- Returning to depot
- Repeat cycle

#### Building State (Workers)
- Moving to construction site
- Building animation
- Progress bar displayed
- Multiple workers accelerate

### Unit Stances

#### Aggressive (Default)
- Auto-attack enemies in range
- Chase enemies briefly
- Return to position if far

#### Defensive
- Auto-attack enemies in range
- Do not pursue
- Stay near original position

#### Passive
- No auto-attack
- Only attack when commanded
- Flee if possible

#### Hold Position
- No movement
- Attack in range only
- Maintain exact position

### Formation and Movement

#### Group Movement
- Units maintain relative positions
- Faster units wait for slower
- Formation types:
  - Line: Spread horizontally
  - Box: Compact square
  - Column: Spread vertically
  - Scatter: Spread out (anti-splash)

#### Collision Behavior
- Units push aside stationary units
- Pathfinding recalculates on block
- Stuck detection and resolution
- Unit radius-based spacing

## Combat Mechanics

### Engagement Rules

#### Target Acquisition
1. **Commanded Target**: Priority #1
2. **Closest Attacker**: Priority #2
3. **Closest Enemy**: Priority #3
4. **Lowest Health**: Tiebreaker

#### Attack Timing
- **Wind-up**: 0.1-0.3s before damage
- **Damage Point**: Exact frame damage applies
- **Backswing**: 0.1-0.2s animation cooldown
- **Turn Rate**: 0.5-1.0s to face new direction

#### Attack Types

**Melee**
- Range: 1 tile (adjacent)
- Must chase target
- No miss chance
- Immediate damage

**Ranged**
- Range: 3-8 tiles
- Projectile travel time
- Can attack while kiting
- Miss if target dies mid-flight

**Siege**
- Range: 5-12 tiles
- Slow projectile
- Area of effect damage
- Minimum range requirement
- Bonus vs buildings

### Damage Calculation

#### Base Formula
```
Final Damage = Base Damage × Type Modifier × (1 - Armor Reduction)
Armor Reduction = Armor / (Armor + 100)
```

#### Example
- Base Damage: 10
- Type Modifier: 1.5 (Pierce vs Light)
- Armor: 5
- Final: 10 × 1.5 × (1 - 5/105) = 14.29 damage

#### Armor Types
- **Unarmored**: Basic units, workers
- **Light**: Ranged units, scouts
- **Medium**: Infantry, cavalry
- **Heavy**: Knights, elite units
- **Fortified**: Buildings, defensive structures

### Unit Abilities

#### Active Abilities
- **Cooldown**: 10-60 seconds
- **Resource Cost**: Some abilities cost mana/energy
- **Cast Time**: 0-1 second
- **Effects**: Buff, debuff, damage, summon

#### Passive Abilities
- **Always Active**: No player input required
- **Conditional**: Triggered by specific events
- **Aura Effects**: Buff nearby friendly units

#### Example Abilities

**Heal** (Active)
- Cost: 50 mana
- Cooldown: 30 seconds
- Effect: Restore 100 HP to target
- Range: 5 tiles

**Charge** (Active)
- Cost: None
- Cooldown: 45 seconds
- Effect: Rush forward, stun enemies
- Range: 8 tiles

**Regeneration** (Passive)
- Effect: Restore 2 HP per second
- Condition: Out of combat for 5 seconds

**Leadership** (Passive Aura)
- Effect: +10% attack speed to nearby allies
- Range: 4 tiles

## Building Mechanics

### Construction

#### Placement Requirements
- **Space**: Building size must fit (2x2, 3x3, 4x4 tiles)
- **Terrain**: Buildable ground only (no water, cliffs)
- **Clearance**: No overlap with units or resources
- **Proximity**: Some buildings require nearby structures

#### Construction Process
1. **Blueprint Phase**: 0% health, can't be used
2. **Building Phase**: Health increases over time
3. **Completion**: Full health, becomes functional
4. **Vulnerable**: Can be attacked during construction

#### Worker Efficiency
- 1 Worker: 100% build speed (base time)
- 2 Workers: 175% build speed
- 3 Workers: 225% build speed
- 4+ Workers: 250% build speed (cap)

### Building Types

#### Economic Buildings

**Town Hall**
- Size: 4x4
- HP: 1500
- Function: Main resource depot, worker production
- Upgrades: Keep (tier 2), Castle (tier 3)

**Farm**
- Size: 2x2
- HP: 400
- Function: +8 supply
- Cost: 60 gold, 20 wood

**Lumber Mill**
- Size: 3x3
- HP: 600
- Function: Wood gathering upgrades
- Cost: 120 gold, 80 wood

#### Military Buildings

**Barracks**
- Size: 3x3
- HP: 800
- Function: Infantry unit production
- Cost: 150 gold, 50 wood

**Archery Range**
- Size: 3x3
- HP: 700
- Function: Ranged unit production
- Cost: 140 gold, 60 wood

**Siege Workshop**
- Size: 3x3
- HP: 750
- Function: Siege unit production
- Cost: 200 gold, 100 wood

#### Defensive Buildings

**Guard Tower**
- Size: 2x2
- HP: 500
- Function: Attacks enemies, provides vision
- Cost: 100 gold, 50 wood
- Attack: 15 pierce damage, 8 range

**Wall**
- Size: 1x1
- HP: 300
- Function: Blocks movement
- Cost: 5 gold, 5 wood per segment

### Production Queue

#### Queue Mechanics
- Maximum 5 units queued per building
- Resources spent immediately on queue
- Cancel for 75% refund
- View progress on selected building
- Rally points set destination for produced units

#### Rally Points
- Click building, right-click location
- Units auto-move on completion
- Can rally to resources (workers auto-gather)
- Can rally to enemy (units auto-attack)

## Technology and Upgrades

### Tech Tree Structure

#### Tier System
- **Tier 1**: Basic units and buildings
- **Tier 2**: Advanced units, requires Keep upgrade
- **Tier 3**: Elite units, requires Castle upgrade

#### Upgrade Categories

**Economic Upgrades**
- Improved gathering speed (+10% per level, 3 levels)
- Increased carry capacity (+5 per level, 2 levels)
- Faster building construction (+20% per level, 2 levels)

**Military Upgrades**
- Weapon upgrades: +1 damage per level, 3 levels
- Armor upgrades: +1 armor per level, 3 levels
- Unit-specific upgrades (range, speed, abilities)

**Research Costs**
- Level 1: 100 gold, 50 wood, 60 seconds
- Level 2: 200 gold, 100 wood, 90 seconds
- Level 3: 300 gold, 150 wood, 120 seconds

### Faction-Specific Technologies
- Each faction has 3-5 unique technologies
- Provide distinct strategic advantages
- Example: Human "Divine Shield" (temporary invulnerability)

## Victory and Defeat Conditions

### Standard Victory
- **Elimination**: Destroy all enemy buildings and units
- **Resignation**: Opponent concedes

### Scenario Victory (Campaign)
- **Objective-Based**: Complete specific goals
- **Survival**: Survive for time duration
- **Escort**: Protect unit to destination
- **Destruction**: Destroy specific targets

### Defeat Conditions
- All buildings destroyed
- Special units killed (campaign)
- Time limit exceeded (some scenarios)
- Voluntary resignation

## Fog of War

### Visibility States

#### Unexplored (Black)
- Never seen before
- No information available
- Completely hidden

#### Explored (Dark Gray)
- Previously visible
- Shows terrain and buildings (last seen state)
- No unit information
- Buildings may be destroyed

#### Visible (Full Color)
- Currently in vision range
- Real-time information
- Units and buildings shown
- Health bars visible

### Vision Mechanics
- Units: 6-9 tile vision radius
- Buildings: 4-6 tile vision radius
- Towers: 10 tile vision radius
- High ground: +2 bonus vision
- Vision blocked by cliffs (not trees)

## Game Pacing

### Early Game (0-5 minutes)
- Build workers (aim for 12-14)
- Scout enemy base location
- Establish resource gathering
- First military building
- Minor harassment possible

### Mid Game (5-15 minutes)
- Army production begins
- First major battles
- Tech tree progression
- Expansion to second base
- Map control contested

### Late Game (15+ minutes)
- Multiple bases active
- Full army compositions
- High-tier units deployed
- Decisive battles
- Victory conditions approaching

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
