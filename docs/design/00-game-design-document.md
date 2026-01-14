# Game Design Document (GDD)
## RedGate: Tactical Dungeon RPG

> _This is an evolving document and may change over time. It acts as the guiding blueprint for the project._

---

## Executive Summary

RedGate is being reimagined as a **single-player, 8-bit style real-time tactical strategy RPG** built using Godot Engine 4.5 with C# (.NET). The game draws inspiration from Warcraft 3's hero system and Diablo-style dungeon crawling, featuring procedurally generated dungeons and a persistent town hub. The project maintains its open-source nature under the AGPL-3.0-only license.

This document adapts the existing RedSrc codebase architecture to support the new tactical RPG vision while preserving the modular, data-driven ECS-inspired design philosophy.

---

## Game Concept

### High-Level Description

Players control a single hero character who ventures into a large, procedurally generated dungeon to battle enemies, collect loot, and complete objectives. Between dungeon runs, players return to a persistent town where they can:
- Manage inventory and equipment
- Upgrade abilities and stats
- Interact with NPCs
- Craft and purchase items
- Accept quests and advance the narrative

The game combines **real-time tactical combat** with **strategic resource management** and **character progression**.

### Core Genre Elements

| Element | Description |
|---------|-------------|
| **Hero System** | Single controllable hero with distinct class abilities, inspired by Warcraft 3 |
| **Real-Time Combat** | Tactical, ability-based combat with cooldowns and resource management |
| **Dungeon Exploration** | Large procedurally generated dungeon with multiple biomes and difficulty tiers |
| **Town Hub** | Persistent safe zone for between-run activities and progression |
| **Loot System** | Diablo-style item drops with rarity tiers and stat variations |
| **Character Progression** | Experience-based leveling with skill trees and attribute allocation |

---

## Design Pillars

These pillars guide all design decisions and remain consistent with the original RedSrc architecture:

1. **Modularity**: Self-contained components, systems, and services that can be easily added, removed, or modified.

2. **Data-Driven Design**: Game logic driven by data resources (`.tres` files), enabling configuration without code changes.

3. **Performance**: Centralized system logic for handling large numbers of entities efficiently.

4. **Separation of Concerns**: Clear layer responsibilities for maintainability and extensibility.

5. **Designer-Friendly**: Accessible content creation through Godot's scene system and exported resources.

6. **Dual Game Modes**: Distinct Dungeon (action) and Town (management) modes with separate managers.

7. **Fantasy OS Metaphor**: UI themed around a retro fantasy operating system aesthetic.

8. **Limited Graphics**: 1-bit/2-bit pixel art style with CRT filter aesthetic.

---

## Game Loops

### Primary Loop: Dungeon Exploration

```
┌─────────────────────────────────────────────────────────────┐
│                    DUNGEON EXPLORATION                       │
├─────────────────────────────────────────────────────────────┤
│  1. Enter Dungeon (from Town)                                │
│     └─> Select difficulty/floor                              │
│                                                              │
│  2. Explore & Combat                                         │
│     ├─> Navigate procedural rooms/corridors                  │
│     ├─> Engage enemies in real-time tactical combat          │
│     ├─> Use abilities (cooldown-based)                       │
│     ├─> Manage health and mana resources                     │
│     └─> Discover secrets and treasure                        │
│                                                              │
│  3. Collect Loot                                             │
│     ├─> Equipment drops from enemies/chests                  │
│     ├─> Gold and crafting materials                          │
│     └─> Quest items and keys                                 │
│                                                              │
│  4. Progress or Return                                       │
│     ├─> Find stairs to descend deeper                        │
│     ├─> Use portal/waypoint to return to town                │
│     └─> Death returns to town (with penalties)               │
└─────────────────────────────────────────────────────────────┘
```

### Secondary Loop: Town Management

```
┌─────────────────────────────────────────────────────────────┐
│                      TOWN MANAGEMENT                         │
├─────────────────────────────────────────────────────────────┤
│  1. Return to Town                                           │
│     └─> Safe haven, no combat                                │
│                                                              │
│  2. Character Management                                     │
│     ├─> Level up and allocate stat points                    │
│     ├─> Unlock and upgrade abilities                         │
│     └─> Equip and compare items                              │
│                                                              │
│  3. Town Services                                            │
│     ├─> Vendor: Buy/sell items                               │
│     ├─> Blacksmith: Craft and upgrade equipment              │
│     ├─> Alchemist: Create potions and consumables            │
│     ├─> Quest Board: Accept and track quests                 │
│     └─> Stash: Long-term item storage                        │
│                                                              │
│  4. Narrative & NPCs                                         │
│     ├─> Dialogue with town residents                         │
│     ├─> Unlock story progression                             │
│     └─> Discover lore and world-building                     │
│                                                              │
│  5. Prepare for Next Run                                     │
│     └─> Optimize loadout and return to dungeon               │
└─────────────────────────────────────────────────────────────┘
```

---

## Hero System

### Hero Classes

Each hero class has distinct playstyles, abilities, and stat distributions:

| Class | Primary Role | Primary Stat | Description |
|-------|--------------|--------------|-------------|
| **Warrior** | Tank/Melee DPS | Strength | Heavy armor, melee weapons, defensive abilities |
| **Mage** | Ranged DPS | Intelligence | Elemental spells, crowd control, glass cannon |
| **Rogue** | Burst DPS | Agility | Stealth, critical strikes, mobility |
| **Cleric** | Support/Hybrid | Wisdom | Healing, buffs, holy damage |

### Ability System

- **4 Active Abilities**: Class-specific skills with cooldowns
- **1 Ultimate Ability**: Powerful ability with long cooldown
- **Passive Abilities**: Always-active bonuses based on class and level
- **Basic Attack**: Primary weapon attack (no cooldown)

### Stat System

| Stat | Effect |
|------|--------|
| **Strength** | Melee damage, health, armor |
| **Agility** | Attack speed, dodge chance, critical chance |
| **Intelligence** | Magic damage, mana pool, ability power |
| **Wisdom** | Mana regen, cooldown reduction, healing power |
| **Vitality** | Maximum health, health regeneration |

---

## Dungeon Design

### Structure

The dungeon is a single, large procedurally generated environment divided into:

- **Floors**: Vertical progression (deeper = harder)
- **Rooms**: Individual combat/exploration areas
- **Corridors**: Connecting passages between rooms
- **Biomes**: Thematic areas with unique enemies and aesthetics

### Procedural Generation

```
Dungeon Floor Structure:
├── Entry Room (safe, waypoint)
├── Standard Rooms (combat, loot)
├── Challenge Rooms (elite encounters)
├── Treasure Rooms (high loot)
├── Boss Room (floor guardian)
└── Exit/Stairs (to next floor)
```

### Enemy Encounters

- **Standard Mobs**: Common enemies, moderate threat
- **Elite Mobs**: Stronger variants with special abilities
- **Mini-Bosses**: Powerful enemies guarding key areas
- **Floor Bosses**: Major encounters at each floor's end

---

## Town Design

### Town Locations

| Location | Function |
|----------|----------|
| **Town Square** | Central hub, waypoint access |
| **Merchant's Row** | Vendors and shops |
| **Blacksmith** | Crafting and upgrades |
| **Apothecary** | Potions and consumables |
| **Guild Hall** | Quest board and guild services |
| **Inn** | Save game, rest, NPC stories |
| **Library** | Lore, tutorials, bestiary |
| **Player Stash** | Secure item storage |

### NPC Interactions

- **Dialogue Trees**: Branching conversations
- **Reputation System**: Relationship tracking with factions
- **Quest Givers**: Mission providers with unique storylines
- **Merchants**: Buy/sell with dynamic pricing

---

## Item System

### Item Categories

- **Weapons**: Swords, staffs, daggers, maces, bows
- **Armor**: Head, chest, gloves, boots, accessories
- **Consumables**: Potions, scrolls, food
- **Materials**: Crafting components
- **Quest Items**: Key story/progression items

### Rarity Tiers

| Tier | Color | Drop Rate | Stat Bonus |
|------|-------|-----------|------------|
| Common | White | 60% | Base stats |
| Uncommon | Green | 25% | +1 modifier |
| Rare | Blue | 10% | +2 modifiers |
| Epic | Purple | 4% | +3 modifiers |
| Legendary | Orange | 1% | Unique effects |

---

## Combat System

### Real-Time Mechanics

- **Movement**: WASD or click-to-move
- **Basic Attack**: Primary weapon attack (hold/click)
- **Abilities**: Hotkey-activated skills (1-4 + R for ultimate)
- **Dodge/Roll**: Invincibility frames for skill-based evasion
- **Targeting**: Auto-target nearest or manual selection

### Resource Management

- **Health**: Damage buffer, death on depletion
- **Mana**: Ability fuel, regenerates over time
- **Cooldowns**: Per-ability timers

### Damage Calculation

```
Final Damage = (Base Damage + Stat Bonus) × (1 + Critical Multiplier) × (1 - Armor Reduction)
```

---

## Progression Systems

### Character Leveling

- Experience gained from kills, quests, and exploration
- Level cap: 50 (soft cap, prestige system at end-game)
- Stat points per level: 3 (player-allocated)
- Ability points per level: 1 (for skill tree)

### Equipment Progression

- Item level scales with dungeon floor
- Crafting augments existing equipment
- Set bonuses for matching equipment

### Meta Progression

- Unlockable hero classes
- Permanent town upgrades
- Achievement system with rewards

---

## Technical Considerations

### Architecture Alignment

This design aligns with the existing RedSrc layered architecture:

| Layer | Role in Tactical RPG |
|-------|---------------------|
| **Core Infrastructure** | MainCore, EventCore, ServiceCore, StateCore |
| **Primary Services** | Save, Audio, Input, Scene management |
| **Data Abstractions** | Hero, Item, Mob, Dungeon, Quest data resources |
| **Behavior Components** | Health, Physics, Inventory, Combat, AI components |
| **Building Block Entities** | Hero, Enemy, NPC, Item, Projectile entities |
| **Orchestration Managers** | DungeonManager, TownManager |
| **Controlling Logic Systems** | Combat, AI, Loot, Dialogue, Craft systems |
| **Assisting Utilities** | Map generation, pathfinding, damage calculation |

### Performance Targets

- **Entity Count**: Support for 200+ simultaneous entities
- **Frame Rate**: Stable 60 FPS
- **Load Times**: <3 seconds for dungeon floor transitions

---

## Inspirations

| Game | Borrowed Elements |
|------|-------------------|
| **Warcraft 3** | Hero system, abilities, level progression |
| **Diablo** | Loot system, dungeon crawling, town hub |
| **Loop Hero** | Art style, meta-progression |
| **Vampire Survivors** | Real-time combat feel (from original design) |
| **Dwarf Fortress** | Data-driven complexity |
| **Elder Scrolls** | NPC interactions, skill system |

---

## Out of Scope (Initial Release)

The following features are intentionally excluded from the initial scope to focus on core systems:

- Multiplayer/Co-op
- Extraction mechanics (from original design)
- Slot-machine loot containers
- Idle mechanics
- Gardening systems
- Full narrative/story content
- Specific themes and lore

These may be considered for future expansions after core systems are stable.

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2026-01-14 | Design Team | Initial draft adapting from RedSrc architecture |

---

## Related Documents

- [01-technical-architecture.md](01-technical-architecture.md) - System architecture details
- [02-systems-design.md](02-systems-design.md) - Game systems specifications
- [03-component-design.md](03-component-design.md) - Component specifications
- [04-data-structures.md](04-data-structures.md) - Data resource definitions
- [05-implementation-roadmap.md](05-implementation-roadmap.md) - Development phases
