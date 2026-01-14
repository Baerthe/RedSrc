# Unit and Building Systems Design

## Overview

This document provides comprehensive specifications for all units and buildings in the RTS game, including stats, abilities, and faction variations.

## Faction Design

### Faction Philosophy

#### Humans
- **Theme**: Versatile and balanced
- **Strength**: Strong defense, healing magic
- **Weakness**: Expensive units, slower production
- **Playstyle**: Defensive expansion, quality over quantity

#### Orcs
- **Theme**: Aggressive and brutal
- **Strength**: High damage, fast production
- **Weakness**: Lower armor, resource intensive
- **Playstyle**: Early aggression, overwhelming force

### Faction Starting Conditions

#### Starting Resources
- Gold: 500
- Wood: 150
- Supply: 5/10

#### Starting Units
- 5 Workers
- 1 Town Hall
- Vision range revealed: 25 tiles radius

## Human Faction Units

### Worker Units

#### Peasant
```yaml
Type: Worker
Cost: 75 gold
Build Time: 15 seconds
Supply: 1

Stats:
  Health: 40
  Armor: 0 (Unarmored)
  Movement Speed: 3.0 tiles/sec
  Vision: 6 tiles
  
Abilities:
  - Gather Resources (Gold: 10/trip, Wood: 5/trip)
  - Construct Buildings
  - Repair Buildings (5 HP/sec, costs 1 gold per 5 HP)
```

### Basic Military Units

#### Footman
```yaml
Type: Melee Infantry
Cost: 100 gold
Build Time: 20 seconds
Supply: 1
Requirements: Barracks

Stats:
  Health: 60
  Armor: 2 (Medium)
  Damage: 12 (Normal)
  Attack Speed: 1.5 sec
  Attack Range: 1 tile (melee)
  Movement Speed: 2.8 tiles/sec
  Vision: 7 tiles
  
Abilities:
  - Defend (Active): +4 armor for 10 sec, cannot move (Cooldown: 30s)
```

#### Archer
```yaml
Type: Ranged Infantry
Cost: 120 gold, 20 wood
Build Time: 25 seconds
Supply: 1
Requirements: Archery Range

Stats:
  Health: 40
  Armor: 0 (Light)
  Damage: 8 (Pierce)
  Attack Speed: 1.8 sec
  Attack Range: 6 tiles
  Movement Speed: 3.2 tiles/sec
  Vision: 8 tiles
  
Upgrades Available:
  - Long Bows: +2 range (150 gold, 50 wood)
  - Fire Arrows: +3 damage vs buildings (200 gold, 100 wood)
```

#### Priest
```yaml
Type: Support Caster
Cost: 150 gold
Build Time: 30 seconds
Supply: 2
Requirements: Church

Stats:
  Health: 50
  Armor: 0 (Unarmored)
  Mana: 100
  Mana Regen: 1/sec
  Movement Speed: 2.5 tiles/sec
  Vision: 7 tiles
  
Abilities:
  - Heal (Active): Restore 50 HP, Range: 5 tiles, Cost: 30 mana (Cooldown: 10s)
  - Holy Shield (Active): Target ally immune for 3 sec, Cost: 80 mana (Cooldown: 60s)
```

### Advanced Military Units

#### Knight
```yaml
Type: Heavy Cavalry
Cost: 250 gold, 50 wood
Build Time: 45 seconds
Supply: 3
Requirements: Stable, Keep (Tier 2)

Stats:
  Health: 150
  Armor: 4 (Heavy)
  Damage: 25 (Normal)
  Attack Speed: 2.0 sec
  Attack Range: 1 tile (melee)
  Movement Speed: 4.5 tiles/sec
  Vision: 7 tiles
  
Abilities:
  - Charge (Active): Rush 8 tiles, stun hit enemies for 1s (Cooldown: 45s)
  - Trample (Passive): Deals 5 damage to enemies passed through
```

#### Ballista
```yaml
Type: Siege Unit
Cost: 300 gold, 150 wood
Build Time: 60 seconds
Supply: 4
Requirements: Siege Workshop, Keep (Tier 2)

Stats:
  Health: 100
  Armor: 0 (Light)
  Damage: 50 (Siege)
  Attack Speed: 3.5 sec
  Attack Range: 8-12 tiles (minimum range)
  Movement Speed: 2.0 tiles/sec
  Vision: 10 tiles
  
Properties:
  - Area Damage: 2 tile radius splash
  - Bonus vs Buildings: +100 damage
  - Setup Time: 2 sec before attacking
  - Pack Time: 2 sec before moving
```

#### Paladin
```yaml
Type: Hero Unit (Elite)
Cost: 400 gold, 100 wood
Build Time: 75 seconds
Supply: 5
Requirements: Castle (Tier 3), Church
Limit: 1 per player

Stats:
  Health: 300
  Armor: 6 (Heavy)
  Damage: 30 (Normal)
  Attack Speed: 1.8 sec
  Attack Range: 1 tile (melee)
  Movement Speed: 3.5 tiles/sec
  Vision: 9 tiles
  Mana: 150
  Mana Regen: 1.5/sec
  
Abilities:
  - Divine Shield (Active): Invulnerable 5 sec, Cost: 100 mana (Cooldown: 90s)
  - Resurrection (Active): Revive dead unit, Cost: 120 mana (Cooldown: 120s)
  - Holy Aura (Passive): Nearby allies +2 HP/sec regeneration (4 tile radius)
```

## Orc Faction Units

### Worker Units

#### Peon
```yaml
Type: Worker
Cost: 75 gold
Build Time: 15 seconds
Supply: 1

Stats:
  Health: 50
  Armor: 0 (Unarmored)
  Damage: 6 (Normal) - can fight if needed
  Attack Speed: 2.0 sec
  Movement Speed: 2.8 tiles/sec
  Vision: 6 tiles
  
Abilities:
  - Gather Resources (Gold: 10/trip, Wood: 5/trip)
  - Construct Buildings
  - Basic Attack (can defend self)
```

### Basic Military Units

#### Grunt
```yaml
Type: Melee Infantry
Cost: 90 gold
Build Time: 18 seconds
Supply: 1
Requirements: Barracks

Stats:
  Health: 70
  Armor: 1 (Medium)
  Damage: 15 (Normal)
  Attack Speed: 1.6 sec
  Attack Range: 1 tile (melee)
  Movement Speed: 3.0 tiles/sec
  Vision: 7 tiles
  
Abilities:
  - Battle Cry (Active): +20% attack speed for 8 sec (Cooldown: 40s)
```

#### Spearman
```yaml
Type: Ranged Infantry
Cost: 110 gold, 30 wood
Build Time: 22 seconds
Supply: 1
Requirements: War Mill

Stats:
  Health: 45
  Armor: 0 (Light)
  Damage: 10 (Pierce)
  Attack Speed: 1.6 sec
  Attack Range: 5 tiles
  Movement Speed: 3.0 tiles/sec
  Vision: 7 tiles
  
Properties:
  - Bonus vs Cavalry: +8 damage
  - Throwing Axes: Can attack while moving
```

#### Shaman
```yaml
Type: Support Caster
Cost: 140 gold
Build Time: 28 seconds
Supply: 2
Requirements: Spirit Lodge

Stats:
  Health: 45
  Armor: 0 (Unarmored)
  Mana: 120
  Mana Regen: 1.2/sec
  Movement Speed: 2.6 tiles/sec
  Vision: 7 tiles
  
Abilities:
  - Bloodlust (Active): +40% damage to ally for 15 sec, Cost: 40 mana (Cooldown: 20s)
  - Purge (Active): Remove buffs/debuffs, Cost: 50 mana (Cooldown: 15s)
```

### Advanced Military Units

#### Raider
```yaml
Type: Fast Cavalry
Cost: 220 gold, 40 wood
Build Time: 40 seconds
Supply: 3
Requirements: Stable, Stronghold (Tier 2)

Stats:
  Health: 120
  Armor: 2 (Medium)
  Damage: 20 (Normal)
  Attack Speed: 1.5 sec
  Attack Range: 1 tile (melee)
  Movement Speed: 5.0 tiles/sec (fastest unit)
  Vision: 8 tiles
  
Abilities:
  - Pillage (Passive): Gain 20 gold when destroying buildings
  - Net (Active): Immobilize target 3 sec, Range: 4 tiles (Cooldown: 30s)
```

#### Catapult
```yaml
Type: Siege Unit
Cost: 280 gold, 120 wood
Build Time: 55 seconds
Supply: 4
Requirements: Siege Workshop, Stronghold (Tier 2)

Stats:
  Health: 120
  Armor: 1 (Light)
  Damage: 45 (Siege)
  Attack Speed: 3.0 sec
  Attack Range: 7-10 tiles (minimum range)
  Movement Speed: 2.2 tiles/sec
  Vision: 9 tiles
  
Properties:
  - Area Damage: 2.5 tile radius splash
  - Bonus vs Buildings: +80 damage
  - Attack Ground: Can target specific location
```

#### Blademaster
```yaml
Type: Hero Unit (Elite)
Cost: 380 gold, 80 wood
Build Time: 70 seconds
Supply: 5
Requirements: Fortress (Tier 3), Spirit Lodge
Limit: 1 per player

Stats:
  Health: 280
  Armor: 4 (Medium)
  Damage: 35 (Normal)
  Attack Speed: 1.4 sec (very fast)
  Attack Range: 1 tile (melee)
  Movement Speed: 4.0 tiles/sec
  Vision: 9 tiles
  Mana: 180
  Mana Regen: 2/sec
  
Abilities:
  - Critical Strike (Passive): 20% chance for 3x damage
  - Wind Walk (Active): Invisible, +20% speed, 10 sec, Cost: 60 mana (Cooldown: 50s)
  - Bladestorm (Active): Spin attack, damage all nearby, Cost: 100 mana (Cooldown: 80s)
```

## Human Buildings

### Core Buildings

#### Town Hall → Keep → Castle
```yaml
Town Hall (Tier 1):
  Size: 4x4
  Cost: 385 gold, 200 wood
  Build Time: 120 seconds
  Health: 1500
  Armor: 5 (Fortified)
  Functions:
    - Resource depot
    - Produces: Peasants
    - Supply Provided: 10
  
Keep (Tier 2):
  Upgrade Cost: 1500 gold, 500 wood
  Upgrade Time: 180 seconds
  Health: 2500
  Functions: Same as Town Hall + unlocks Tier 2 units
  
Castle (Tier 3):
  Upgrade Cost: 2500 gold, 1000 wood
  Upgrade Time: 240 seconds
  Health: 4000
  Functions: Same as Keep + unlocks Tier 3 units
```

### Economic Buildings

#### Farm
```yaml
Size: 2x2
Cost: 60 gold, 20 wood
Build Time: 20 seconds
Health: 400
Armor: 0 (Fortified)
Functions:
  - Supply Provided: +8
```

#### Lumber Mill
```yaml
Size: 3x3
Cost: 120 gold, 80 wood
Build Time: 40 seconds
Health: 600
Armor: 2 (Fortified)
Functions:
  - Wood gathering upgrades
  - Required for some buildings
```

#### Blacksmith
```yaml
Size: 3x3
Cost: 180 gold, 60 wood
Build Time: 50 seconds
Health: 700
Armor: 3 (Fortified)
Functions:
  - Weapon upgrades (+1/+2/+3 damage)
  - Armor upgrades (+1/+2/+3 armor)
```

### Military Production Buildings

#### Barracks
```yaml
Size: 3x3
Cost: 150 gold, 50 wood
Build Time: 60 seconds
Health: 800
Armor: 3 (Fortified)
Functions:
  - Produces: Footman
  - Supply Provided: +4
```

#### Archery Range
```yaml
Size: 3x3
Cost: 140 gold, 60 wood
Build Time: 55 seconds
Health: 700
Armor: 2 (Fortified)
Functions:
  - Produces: Archer
  - Unit upgrades available
```

#### Stable
```yaml
Size: 3x3
Cost: 240 gold, 100 wood
Build Time: 70 seconds
Health: 750
Armor: 3 (Fortified)
Requirements: Keep (Tier 2)
Functions:
  - Produces: Knight
```

#### Siege Workshop
```yaml
Size: 3x3
Cost: 200 gold, 100 wood
Build Time: 65 seconds
Health: 750
Armor: 2 (Fortified)
Requirements: Keep (Tier 2), Lumber Mill
Functions:
  - Produces: Ballista
```

#### Church
```yaml
Size: 3x3
Cost: 180 gold, 50 wood
Build Time: 60 seconds
Health: 700
Armor: 2 (Fortified)
Functions:
  - Produces: Priest
  - Magical upgrades
```

### Defensive Buildings

#### Guard Tower → Cannon Tower
```yaml
Guard Tower:
  Size: 2x2
  Cost: 100 gold, 50 wood
  Build Time: 40 seconds
  Health: 500
  Armor: 5 (Fortified)
  Attack Damage: 15 (Pierce)
  Attack Speed: 1.5 sec
  Attack Range: 8 tiles
  Vision: 10 tiles

Cannon Tower (Upgrade):
  Cost: 200 gold, 100 wood
  Upgrade Time: 60 seconds
  Health: 700
  Attack Damage: 40 (Siege)
  Attack Speed: 2.5 sec
  Attack Range: 10 tiles
  Splash Damage: 1.5 tile radius
```

#### Wall
```yaml
Size: 1x1
Cost: 5 gold, 5 wood per segment
Build Time: 5 seconds per segment
Health: 300
Armor: 8 (Fortified)
Functions:
  - Blocks ground unit movement
  - Can be destroyed
  - Gates can be built (200 gold, opens for allies)
```

## Orc Buildings

### Core Buildings

#### Great Hall → Stronghold → Fortress
```yaml
Great Hall (Tier 1):
  Size: 4x4
  Cost: 385 gold, 200 wood
  Build Time: 120 seconds
  Health: 1600
  Armor: 5 (Fortified)
  Functions:
    - Resource depot
    - Produces: Peons
    - Supply Provided: 10
  
Stronghold (Tier 2):
  Upgrade Cost: 1400 gold, 450 wood
  Upgrade Time: 170 seconds
  Health: 2600
  Functions: Same as Great Hall + unlocks Tier 2 units
  
Fortress (Tier 3):
  Upgrade Cost: 2400 gold, 900 wood
  Upgrade Time: 230 seconds
  Health: 4200
  Functions: Same as Stronghold + unlocks Tier 3 units
```

### Economic Buildings

#### Pig Farm
```yaml
Size: 2x2
Cost: 60 gold, 20 wood
Build Time: 20 seconds
Health: 450
Armor: 0 (Fortified)
Functions:
  - Supply Provided: +8
```

#### War Mill
```yaml
Size: 3x3
Cost: 120 gold, 80 wood
Build Time: 40 seconds
Health: 650
Armor: 2 (Fortified)
Functions:
  - Wood gathering upgrades
  - Weapon/Armor upgrades
  - Required for ranged units
```

### Military Production Buildings

#### Barracks
```yaml
Size: 3x3
Cost: 140 gold, 40 wood
Build Time: 55 seconds
Health: 850
Armor: 3 (Fortified)
Functions:
  - Produces: Grunt, Spearman
  - Supply Provided: +4
```

#### Stable
```yaml
Size: 3x3
Cost: 230 gold, 90 wood
Build Time: 65 seconds
Health: 800
Armor: 3 (Fortified)
Requirements: Stronghold (Tier 2)
Functions:
  - Produces: Raider
```

#### Siege Workshop
```yaml
Size: 3x3
Cost: 190 gold, 90 wood
Build Time: 60 seconds
Health: 750
Armor: 2 (Fortified)
Requirements: Stronghold (Tier 2), War Mill
Functions:
  - Produces: Catapult
```

#### Spirit Lodge
```yaml
Size: 3x3
Cost: 170 gold, 50 wood
Build Time: 55 seconds
Health: 650
Armor: 2 (Fortified)
Functions:
  - Produces: Shaman
  - Magical upgrades
```

### Defensive Buildings

#### Watch Tower → Bombard Tower
```yaml
Watch Tower:
  Size: 2x2
  Cost: 90 gold, 40 wood
  Build Time: 35 seconds
  Health: 550
  Armor: 5 (Fortified)
  Attack Damage: 13 (Pierce)
  Attack Speed: 1.3 sec
  Attack Range: 7 tiles
  Vision: 10 tiles

Bombard Tower (Upgrade):
  Cost: 180 gold, 90 wood
  Upgrade Time: 55 seconds
  Health: 750
  Attack Damage: 35 (Siege)
  Attack Speed: 2.3 sec
  Attack Range: 9 tiles
  Splash Damage: 1.5 tile radius
```

#### Spiked Barricade
```yaml
Size: 1x1
Cost: 5 gold, 5 wood per segment
Build Time: 4 seconds per segment
Health: 280
Armor: 7 (Fortified)
Damage: 5 (Pierce) - damages melee attackers
Functions:
  - Blocks ground unit movement
  - Thornmail effect on attack
```

## Resource Deposits

### Gold Mine
```yaml
Type: Resource Node
Capacity: 10,000 gold
Gather Rate: 10 gold per trip
Workers Supported: Up to 5 efficiently
Visual: Gold ore deposit sprite
Depletes: Yes, becomes depleted mine (cannot gather)
```

### Tree / Forest
```yaml
Type: Resource Node
Capacity: Infinite (respawns slowly)
Gather Rate: 5 wood per trip
Workers Supported: Up to 3 per tree cluster
Visual: 8-bit tree sprites
Depletes: Individual trees removed, respawn after 180 seconds
```

## Unit Interactions Matrix

### Effectiveness Table
```
Attacker →     Peasant  Footman  Archer  Knight  Siege  Priest
Defender ↓     /Peon                                     /Shaman
─────────────────────────────────────────────────────────────────
Peasant/Peon    Equal   Weak     Weak    Weak    Weak   Equal
Footman/Grunt   Strong  Equal    Weak    Weak    Strong Equal
Archer/Spear    Strong  Strong   Equal   Weak    Strong Weak
Knight/Raider   Strong  Strong   Strong  Equal   Strong Strong
Siege           Strong  Weak     Weak    Weak    Equal  Strong
Buildings       Weak    Weak     Weak    Weak    STRONG Weak
```

### Counter System
- **Infantry counters**: Siege, Casters
- **Ranged counters**: Infantry, Casters
- **Cavalry counters**: Ranged, Siege, Casters
- **Siege counters**: Buildings, stationary units
- **Casters counter**: Massed units (with AoE)

## Balance Considerations

### Cost Efficiency
- Basic units: 100-120 total resources
- Advanced units: 200-300 total resources
- Elite units: 400-500 total resources
- Siege units: 400-450 total resources

### Power Scaling
- Tier 1 units: 100% baseline
- Tier 2 units: 200-250% effective power
- Tier 3 units: 400-500% effective power

### Production Time Balance
- Worker: 15s (constant production needed)
- Basic unit: 20-25s (rapid army building)
- Advanced unit: 40-60s (tactical choices)
- Elite unit: 70-75s (significant investment)

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
