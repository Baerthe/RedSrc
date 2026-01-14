# Implementation Roadmap
## RedGate: Tactical Dungeon RPG

> This document outlines the phased development approach for building the tactical RPG.

---

## Development Philosophy

### Principles

1. **Incremental Delivery**: Each phase delivers playable functionality
2. **Core First**: Build essential systems before features
3. **Validate Early**: Test each system before moving forward
4. **Maintain Flexibility**: Design for future expansion
5. **Data-Driven**: Prefer data configuration over code changes

### Phase Structure

Each development phase follows this cycle:

```
┌─────────────────────────────────────────────────────────────┐
│                    PHASE CYCLE                               │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  1. DESIGN         - Document requirements and interfaces    │
│  2. IMPLEMENT      - Build core functionality                │
│  3. INTEGRATE      - Connect with existing systems           │
│  4. TEST           - Validate behavior and performance       │
│  5. POLISH         - Refine and document                     │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Phase Overview

```
┌─────────────────────────────────────────────────────────────┐
│                 DEVELOPMENT TIMELINE                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Phase 0: Foundation Refactor          [Current → Week 2]    │
│  ────────────────────────────                                │
│  Adapt existing architecture for tactical RPG                │
│                                                              │
│  Phase 1: Core Systems                 [Week 3 → Week 6]     │
│  ─────────────────────                                       │
│  Hero control, combat, basic AI                              │
│                                                              │
│  Phase 2: Dungeon Framework            [Week 7 → Week 10]    │
│  ──────────────────────────                                  │
│  Procedural generation, rooms, navigation                    │
│                                                              │
│  Phase 3: Progression Systems          [Week 11 → Week 14]   │
│  ────────────────────────────                                │
│  Leveling, abilities, loot                                   │
│                                                              │
│  Phase 4: Town Hub                     [Week 15 → Week 18]   │
│  ──────────────                                              │
│  NPCs, shops, quests                                         │
│                                                              │
│  Phase 5: Polish & Content             [Week 19 → Week 22]   │
│  ─────────────────────────                                   │
│  UI, balance, content creation                               │
│                                                              │
│  Phase 6: Testing & Release            [Week 23 → Week 26]   │
│  ──────────────────────────                                  │
│  QA, optimization, launch preparation                        │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Phase 0: Foundation Refactor

**Duration**: 2 weeks  
**Goal**: Adapt existing RedSrc architecture for tactical RPG mechanics

### Objectives

- [ ] Refactor core architecture for new game type
- [ ] Update naming and structure for tactical RPG
- [ ] Establish new component-based entity system
- [ ] Create foundational data structures

### Tasks

#### 0.1 Core Infrastructure Update

| Task | Description | Priority |
|------|-------------|----------|
| Refactor MainCore | Update for DungeonManager/TownManager dual-mode | High |
| Add StateCore | Implement game state management | High |
| Update EventCore | Add tactical RPG event types | High |
| Update ServiceCore | Register new services | High |

```csharp
// StateCore implementation
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

#### 0.2 Manager Restructure

| Task | Description | Priority |
|------|-------------|----------|
| Create DungeonManager | Replaces GameManager for dungeon gameplay | High |
| Create TownManager | New manager for town hub | Medium |
| Refactor UiManager | Update for new UI requirements | Medium |

#### 0.3 Component System Setup

| Task | Description | Priority |
|------|-------------|----------|
| Create IComponent interface | Base interface with Initialize/Reset | High |
| Create HealthComponent | Replace existing health system | High |
| Create PhysicsComponent | Standardized movement component | High |
| Create CombatComponent | Combat stats container | High |

#### 0.4 Data Structure Migration

| Task | Description | Priority |
|------|-------------|----------|
| Update HeroData | Adapt for class-based heroes | High |
| Create EnemyData | Replace MobData | High |
| Create ItemData | New item system foundation | High |
| Create AbilityData | Ability definitions | Medium |

### Deliverables

- [ ] Updated project structure
- [ ] New state management system
- [ ] Base component interfaces
- [ ] Initial data resource classes
- [ ] Working MainCore with state transitions

### Validation

- [ ] Application launches without errors
- [ ] State transitions work (Menu → Loading → Game)
- [ ] Components can be attached to test entities

---

## Phase 1: Core Systems

**Duration**: 4 weeks  
**Goal**: Implement playable hero with basic combat

### Objectives

- [ ] Playable hero character with movement and abilities
- [ ] Basic enemy AI and spawning
- [ ] Functional combat system
- [ ] Simple test environment

### Tasks

#### 1.1 Hero System (Week 3)

| Task | Description | Priority |
|------|-------------|----------|
| Create HeroEntity | Entity with all hero components | High |
| Implement input handling | WASD movement, ability hotkeys | High |
| Create SpriteComponent | Animation handling | High |
| Create AbilityComponent | Ability slot management | High |

**Hero Entity Structure**:
```
HeroEntity.tscn
├── HeroEntity (Node2D)
│   ├── Physics (PhysicsComponent/CharacterBody2D)
│   ├── Health (HealthComponent)
│   ├── Mana (ManaComponent)
│   ├── Combat (CombatComponent)
│   ├── Abilities (AbilityComponent)
│   ├── Sprite (SpriteComponent/AnimatedSprite2D)
│   └── Hurtbox (Area2D)
```

#### 1.2 Physics System (Week 3)

| Task | Description | Priority |
|------|-------------|----------|
| Create PhysicsSystem | Centralized movement processing | High |
| Implement movement | Velocity, acceleration, friction | High |
| Add collision handling | move_and_slide integration | High |

#### 1.3 Combat System (Week 4)

| Task | Description | Priority |
|------|-------------|----------|
| Create CombatSystem | Damage processing and resolution | High |
| Implement damage calculation | Formulas for physical/magical | High |
| Add critical hit system | Crit chance and multiplier | Medium |
| Implement death handling | Death events and cleanup | High |

**Damage Formula**:
```
BaseDamage = WeaponDamage + (Strength × 0.5)
CritCheck = Random() < CritChance
FinalDamage = BaseDamage × (IsCrit ? CritMultiplier : 1) × (1 - ArmorReduction)
```

#### 1.4 Ability System (Week 4-5)

| Task | Description | Priority |
|------|-------------|----------|
| Create AbilitySystem | Ability execution and cooldowns | High |
| Implement instant abilities | Direct damage/effects | High |
| Implement projectile abilities | Projectile spawning | Medium |
| Implement AoE abilities | Area damage | Medium |

#### 1.5 AI System (Week 5-6)

| Task | Description | Priority |
|------|-------------|----------|
| Create AIComponent | AI state and behavior data | High |
| Create AISystem | Decision making and behavior | High |
| Implement Chase behavior | Follow and approach target | High |
| Implement Attack behavior | Attack when in range | High |

**AI State Machine**:
```
Idle → (detect player) → Chase → (in range) → Attack
  ↑                        ↓                     ↓
  └────────────────────────┴─────────────────────┘
         (target lost / dead)
```

#### 1.6 Spawn System (Week 6)

| Task | Description | Priority |
|------|-------------|----------|
| Create SpawnSystem | Entity creation and pooling | High |
| Implement object pooling | Reuse inactive entities | Medium |
| Create EnemyEntity | Enemy entity template | High |

### Deliverables

- [ ] Playable hero with 4-directional movement
- [ ] Working health/mana systems
- [ ] Basic melee attack
- [ ] 2-3 test abilities
- [ ] 2-3 enemy types with simple AI
- [ ] Test arena scene

### Validation

- [ ] Hero moves smoothly with WASD
- [ ] Abilities trigger on hotkey press
- [ ] Enemies chase and attack player
- [ ] Combat damage applies correctly
- [ ] Death triggers for both hero and enemies
- [ ] 60 FPS with 50+ enemies

---

## Phase 2: Dungeon Framework

**Duration**: 4 weeks  
**Goal**: Procedurally generated dungeon with exploration

### Objectives

- [ ] Working dungeon generation algorithm
- [ ] Room-based structure with corridors
- [ ] Functional navigation between rooms
- [ ] Chest and loot spawning

### Tasks

#### 2.1 Dungeon Generation (Week 7-8)

| Task | Description | Priority |
|------|-------------|----------|
| Create DungeonGenerator utility | Room placement algorithm | High |
| Implement room templates | Predefined room shapes | High |
| Create corridor generation | Connect rooms | High |
| Add door placement | Entry/exit points | High |

**Generation Algorithm**:
```
1. Create starting room at origin
2. For each room count:
   a. Pick random existing room
   b. Pick random direction with open door
   c. Generate new room of valid type
   d. Create corridor connecting them
3. Place boss room at furthest point from start
4. Place exit stairs in boss room
```

#### 2.2 Tilemap Integration (Week 8)

| Task | Description | Priority |
|------|-------------|----------|
| Create dungeon tileset | Walls, floors, doors | High |
| Implement tile painting | Convert rooms to tiles | High |
| Add collision generation | Create navigation mesh | High |

#### 2.3 Room System (Week 9)

| Task | Description | Priority |
|------|-------------|----------|
| Create RoomData structures | Room templates and spawns | High |
| Implement room types | Combat, treasure, boss, safe | High |
| Create room population | Spawn enemies and items | High |
| Add room discovery | Fog of war / minimap | Medium |

#### 2.4 Loot System (Week 9-10)

| Task | Description | Priority |
|------|-------------|----------|
| Create LootSystem | Drop and collection handling | High |
| Implement loot tables | Weighted random drops | High |
| Create ItemEntity | Ground item representation | High |
| Add chest objects | Interactive loot containers | Medium |

#### 2.5 Floor Progression (Week 10)

| Task | Description | Priority |
|------|-------------|----------|
| Implement stairs | Floor transition triggers | High |
| Create floor scaling | Enemy level scaling | High |
| Add waypoint system | Return points | Medium |

### Deliverables

- [ ] Generated dungeon floors (5-12 rooms)
- [ ] Multiple room types
- [ ] Working corridors and doors
- [ ] Enemy spawns per room
- [ ] Chests with loot
- [ ] Floor transitions

### Validation

- [ ] Dungeons generate consistently
- [ ] All rooms are connected
- [ ] Navigation mesh works correctly
- [ ] Enemies spawn in appropriate rooms
- [ ] Loot drops and can be collected
- [ ] Stairs lead to new floor

---

## Phase 3: Progression Systems

**Duration**: 4 weeks  
**Goal**: Character progression and equipment systems

### Objectives

- [ ] Experience and leveling system
- [ ] Stat allocation on level up
- [ ] Equipment with stat modifiers
- [ ] Skill/ability progression

### Tasks

#### 3.1 Experience System (Week 11)

| Task | Description | Priority |
|------|-------------|----------|
| Implement XP gains | From kills, quests | High |
| Create level calculation | XP thresholds | High |
| Add level up events | Trigger on threshold | High |
| Create XP UI | Progress bar | Medium |

**XP Formula**:
```
XP Required = BaseXP × (Level ^ ExperienceMultiplier)
Example: 100 × (Level ^ 1.5)
```

#### 3.2 Stat Allocation (Week 11-12)

| Task | Description | Priority |
|------|-------------|----------|
| Create stat point system | Points per level | High |
| Implement stat effects | Derived stat calculation | High |
| Add allocation UI | Stat screen | Medium |

#### 3.3 Equipment System (Week 12-13)

| Task | Description | Priority |
|------|-------------|----------|
| Create InventoryComponent | Item storage | High |
| Implement equipment slots | Head, chest, weapon, etc. | High |
| Add stat modifier application | Equipment bonuses | High |
| Create inventory UI | Grid-based inventory | Medium |

#### 3.4 Item Generation (Week 13)

| Task | Description | Priority |
|------|-------------|----------|
| Create LootGenerator utility | Random item creation | High |
| Implement modifier rolling | Random stat bonuses | High |
| Add item level scaling | Stats scale with floor | High |

**Modifier Rolling**:
```
Modifiers by Rarity:
- Common: 0 random modifiers
- Uncommon: 1 random modifier
- Rare: 2 random modifiers
- Epic: 3 random modifiers
- Legendary: 3+ with unique effect
```

#### 3.5 Ability Progression (Week 14)

| Task | Description | Priority |
|------|-------------|----------|
| Create skill tree data | Ability unlock paths | High |
| Implement ability unlocking | Level requirements | High |
| Add ability upgrades | Enhanced effects | Medium |
| Create skill tree UI | Visual tree | Medium |

### Deliverables

- [ ] Working XP and leveling
- [ ] Stat allocation system
- [ ] 5 equipment slots functional
- [ ] Item generation with modifiers
- [ ] Basic skill tree
- [ ] Progression UI elements

### Validation

- [ ] Killing enemies grants XP
- [ ] Level ups occur at correct thresholds
- [ ] Stat allocation affects gameplay
- [ ] Equipment modifies stats correctly
- [ ] Item rarity affects power
- [ ] Abilities can be unlocked

---

## Phase 4: Town Hub

**Duration**: 4 weeks  
**Goal**: Safe zone with services and NPCs

### Objectives

- [ ] Town environment with locations
- [ ] Interactive NPCs
- [ ] Shop system
- [ ] Quest system
- [ ] Dialogue system

### Tasks

#### 4.1 Town Environment (Week 15)

| Task | Description | Priority |
|------|-------------|----------|
| Create TownManager | Town state management | High |
| Design town layout | Buildings and paths | High |
| Create town tileset | 8-bit aesthetic | High |
| Add building interiors | Interior scenes | Medium |

#### 4.2 NPC System (Week 15-16)

| Task | Description | Priority |
|------|-------------|----------|
| Create NPCEntity | NPC base class | High |
| Implement NPCData | NPC definitions | High |
| Add interaction trigger | Talk when near | High |
| Create NPC schedules | Optional: time-based | Low |

#### 4.3 Dialogue System (Week 16)

| Task | Description | Priority |
|------|-------------|----------|
| Create DialogueSystem | Conversation management | High |
| Implement dialogue trees | Branching conversations | High |
| Add dialogue choices | Player responses | High |
| Create dialogue UI | Text box, choices | High |

#### 4.4 Shop System (Week 17)

| Task | Description | Priority |
|------|-------------|----------|
| Create ShopSystem | Buy/sell handling | High |
| Implement vendor inventory | Shop stock | High |
| Add pricing calculation | Buy/sell prices | High |
| Create shop UI | Item grid, gold display | High |

#### 4.5 Quest System (Week 17-18)

| Task | Description | Priority |
|------|-------------|----------|
| Create QuestSystem | Quest tracking | High |
| Implement objectives | Kill, collect, explore | High |
| Add quest rewards | XP, gold, items | High |
| Create quest log UI | Active/completed lists | Medium |

#### 4.6 Crafting System (Week 18)

| Task | Description | Priority |
|------|-------------|----------|
| Create CraftSystem | Recipe processing | Medium |
| Implement crafting recipes | Item combinations | Medium |
| Add crafting station | Blacksmith NPC | Medium |
| Create crafting UI | Recipe list, materials | Medium |

### Deliverables

- [ ] Navigable town environment
- [ ] 5+ interactive NPCs
- [ ] Working shop (buy/sell)
- [ ] Quest board with 3+ quests
- [ ] Dialogue trees
- [ ] Basic crafting

### Validation

- [ ] Can enter/exit town from dungeon
- [ ] NPCs respond to interaction
- [ ] Dialogues display correctly
- [ ] Items can be bought/sold
- [ ] Quests track progress
- [ ] Crafting produces items

---

## Phase 5: Polish & Content

**Duration**: 4 weeks  
**Goal**: Refined experience with content variety

### Objectives

- [ ] Complete UI/UX
- [ ] Multiple hero classes
- [ ] Enemy variety
- [ ] Balance tuning
- [ ] Audio integration

### Tasks

#### 5.1 UI Polish (Week 19)

| Task | Description | Priority |
|------|-------------|----------|
| Implement fantasy OS theme | Retro UI aesthetic | High |
| Add HUD elements | Health, mana, abilities | High |
| Create menu screens | Main menu, pause, options | High |
| Add UI animations | Transitions, feedback | Medium |

#### 5.2 Hero Classes (Week 19-20)

| Task | Description | Priority |
|------|-------------|----------|
| Create Warrior class | Melee, tank abilities | High |
| Create Mage class | Ranged, AoE abilities | High |
| Create Rogue class | Stealth, burst abilities | High |
| Create Cleric class | Healing, support | Medium |

#### 5.3 Enemy Content (Week 20-21)

| Task | Description | Priority |
|------|-------------|----------|
| Create enemy variety | 10+ enemy types | High |
| Design boss encounters | 3+ unique bosses | High |
| Implement enemy abilities | Special attacks | Medium |
| Add elite modifiers | Enhanced enemies | Medium |

#### 5.4 Item Content (Week 21)

| Task | Description | Priority |
|------|-------------|----------|
| Create weapon variety | 15+ weapons | High |
| Create armor sets | 3+ full sets | High |
| Add consumables | Potions, scrolls | High |
| Design legendary items | Unique effects | Medium |

#### 5.5 Audio Integration (Week 22)

| Task | Description | Priority |
|------|-------------|----------|
| Add sound effects | Combat, UI, ambient | High |
| Implement music system | Dynamic music | Medium |
| Add ambient sounds | Environment audio | Medium |

#### 5.6 Balance Pass (Week 22)

| Task | Description | Priority |
|------|-------------|----------|
| Tune damage values | Combat balance | High |
| Adjust progression | XP and scaling curves | High |
| Balance economy | Gold gains and prices | High |
| Test difficulty curve | Floor-by-floor | High |

### Deliverables

- [ ] 4 playable hero classes
- [ ] 15+ enemy types
- [ ] 3+ boss encounters
- [ ] 30+ equipment items
- [ ] Complete UI theme
- [ ] Full audio integration

### Validation

- [ ] All classes feel distinct
- [ ] Enemy variety provides challenge
- [ ] Bosses require strategy
- [ ] Progression feels rewarding
- [ ] UI is intuitive
- [ ] Audio enhances experience

---

## Phase 6: Testing & Release

**Duration**: 4 weeks  
**Goal**: Stable, optimized release candidate

### Objectives

- [ ] Comprehensive testing
- [ ] Performance optimization
- [ ] Bug fixing
- [ ] Release preparation

### Tasks

#### 6.1 Quality Assurance (Week 23-24)

| Task | Description | Priority |
|------|-------------|----------|
| Full playthrough testing | Start to end testing | High |
| Edge case testing | Boundary conditions | High |
| Regression testing | Previous fixes | High |
| Performance profiling | Identify bottlenecks | High |

#### 6.2 Bug Fixing (Week 24-25)

| Task | Description | Priority |
|------|-------------|----------|
| Critical bug fixes | Game-breaking issues | High |
| Major bug fixes | Significant problems | High |
| Minor bug fixes | Polish issues | Medium |
| UI/UX fixes | Usability problems | Medium |

#### 6.3 Optimization (Week 25)

| Task | Description | Priority |
|------|-------------|----------|
| Memory optimization | Reduce allocations | High |
| Render optimization | Draw call reduction | High |
| Loading optimization | Faster transitions | Medium |
| Entity pooling tuning | Pool sizes | Medium |

#### 6.4 Release Preparation (Week 26)

| Task | Description | Priority |
|------|-------------|----------|
| Create build pipeline | Automated builds | High |
| Prepare release notes | Changelog | High |
| Create documentation | Player guide | Medium |
| Final QA pass | Release candidate test | High |

### Deliverables

- [ ] Stable release candidate
- [ ] Performance benchmarks met
- [ ] Documentation complete
- [ ] Build pipeline operational

### Validation

- [ ] No critical bugs
- [ ] 60 FPS on target hardware
- [ ] Complete gameplay loop functional
- [ ] Clean startup and shutdown

---

## Technical Debt & Maintenance

### Ongoing Tasks

These tasks should be addressed throughout development:

| Task | Frequency | Priority |
|------|-----------|----------|
| Code review | Weekly | High |
| Documentation update | Per feature | Medium |
| Unit test maintenance | Per change | Medium |
| Performance monitoring | Weekly | Medium |
| Dependency updates | Monthly | Low |

### Technical Debt Items

Track and address these as capacity allows:

- [ ] Legacy code cleanup from original RedSrc
- [ ] Test coverage improvement
- [ ] Error handling standardization
- [ ] Logging system enhancement
- [ ] Save/load system robustness

---

## Risk Assessment

### High Risk Items

| Risk | Mitigation |
|------|------------|
| Procedural generation produces unplayable layouts | Implement validation and regeneration |
| AI performance with many entities | Aggressive pooling and update throttling |
| Balance issues discovered late | Early playtesting and iterative tuning |

### Medium Risk Items

| Risk | Mitigation |
|------|------------|
| Scope creep | Strict phase deliverables |
| Integration issues between systems | Clear interfaces and documentation |
| Art asset availability | Plan for placeholder/procedural generation |

### Low Risk Items

| Risk | Mitigation |
|------|------------|
| Godot version compatibility | Lock version, track breaking changes |
| C# API changes | Use stable features only |

---

## Success Criteria

### Minimum Viable Product (Phase 3 Complete)

- [ ] One playable hero class
- [ ] 5+ floor dungeon
- [ ] Basic loot and progression
- [ ] Combat functional

### Feature Complete (Phase 5 Complete)

- [ ] 4 hero classes
- [ ] 10+ floor dungeon
- [ ] Full progression system
- [ ] Town hub with services
- [ ] Quest system

### Release Ready (Phase 6 Complete)

- [ ] Stable 60 FPS
- [ ] No critical bugs
- [ ] Complete gameplay loop
- [ ] Documentation complete

---

## Related Documents

- [00-game-design-document.md](00-game-design-document.md) - Game design overview
- [01-technical-architecture.md](01-technical-architecture.md) - System architecture
- [02-systems-design.md](02-systems-design.md) - System specifications
- [03-component-design.md](03-component-design.md) - Component specifications
- [04-data-structures.md](04-data-structures.md) - Data resource definitions
