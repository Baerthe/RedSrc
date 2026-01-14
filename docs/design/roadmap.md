# Development Roadmap

> **Version**: 0.1.0  
> **Last Updated**: 2026-01-14

## Overview

This roadmap outlines the development phases for RedSrc/RedGate, focusing on building core systems first before adding content-specific features. The goal is to establish a robust, generic foundation that can support future theming and story elements.

## Development Philosophy

### Build Order Priority
1. **Core Infrastructure** - Systems that everything depends on
2. **Gameplay Foundation** - Basic game loop functionality
3. **Content Framework** - Data structures and tools for content creation
4. **Polish & Features** - Enhanced features and game-specific content

### Milestone Definitions

| Status | Meaning |
|--------|---------|
| ⬜ Not Started | Work not yet begun |
| 🟨 In Progress | Currently being developed |
| 🟩 Complete | Implemented and tested |
| 🟦 Blocked | Waiting on dependencies |

## Phase 1: Core Foundation (Weeks 1-4)

Establish the fundamental architecture and infrastructure.

### 1.1 Infrastructure Setup 🟨

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| MainCore singleton | 🟩 | - | Root orchestrator |
| EventCore pub/sub | 🟩 | MainCore | Event bus system |
| ServiceCore registry | 🟩 | MainCore | Service location |
| StateCore state machine | 🟨 | EventCore | Game state management |
| ContextCore globals | ⬜ | ServiceCore | Global context data |
| CameraCore | 🟩 | MainCore | Camera management |

### 1.2 Service Layer 🟨

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| AudioService | 🟩 | ServiceCore | Audio playback |
| HeroService | 🟩 | ServiceCore | Hero data management |
| LevelService | 🟩 | ServiceCore | Level data management |
| PrefService | 🟩 | ServiceCore | User preferences |
| SaveService | ⬜ | ServiceCore | Save/load persistence |
| InputService | ⬜ | ServiceCore | Input mapping |
| SceneService | ⬜ | ServiceCore | Scene transitions |

### 1.3 Data Foundation 🟨

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| InfoData common | 🟩 | - | Basic info resource |
| StatsData common | 🟩 | - | Stats resource |
| AssetData common | 🟩 | - | Asset references |
| Metadata common | 🟩 | - | Tracking data |
| HeroData | 🟩 | Commons | Hero definition |
| MobData | 🟩 | Commons | Enemy definition |
| LevelData | 🟩 | Commons | Level definition |
| ItemData | 🟩 | Commons | Item definition |
| WeaponData | 🟩 | Commons | Weapon definition |

### 1.4 Index System 🟨

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| EntityIndex | 🟩 | Data resources | Entity templates |
| HeroIndex | 🟩 | HeroData | Hero lookup |
| LevelIndex | 🟩 | LevelData | Level lookup |
| ItemIndex | 🟩 | ItemData | Item lookup |
| WeaponIndex | 🟩 | WeaponData | Weapon lookup |
| ChantIndex | ⬜ | ChantData | Chant lookup |

**Phase 1 Deliverable**: Working infrastructure with service registration, event system, and data loading.

---

## Phase 2: Gameplay Core (Weeks 5-8)

Implement the basic extraction gameplay loop.

### 2.1 Entity System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| IEntity interface | 🟩 | - | Entity contract |
| IComponent interface | ⬜ | - | Component contract |
| ISystem interface | ⬜ | - | System contract |
| GameEntity base | 🟩 | IEntity | Game entity |
| Component registration | ⬜ | IComponent | System wiring |

### 2.2 Component Development ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| HealthComponent | ⬜ | IComponent | Health data container |
| PhysicsComponent | ⬜ | IComponent | Movement data |
| AIComponent | ⬜ | IComponent | AI state data |
| RenderComponent | ⬜ | IComponent | Visual representation |
| HitboxComponent | ⬜ | IComponent | Collision detection |
| CombatComponent | ⬜ | IComponent | Combat stats |

### 2.3 Game Systems ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| PhysicsSystem | ⬜ | PhysicsComponent | Movement processing |
| AISystem | ⬜ | AIComponent | Enemy behavior |
| CombatSystem | ⬜ | CombatComponent | Damage calculations |
| FactorySystem | ⬜ | All components | Entity instantiation |
| Object pooling | ⬜ | FactorySystem | Performance optimization |

### 2.4 Player Implementation ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| HeroEntity scene | 🟩 | Components | Player template |
| Player movement | ⬜ | PhysicsSystem | WASD movement |
| Player input handling | ⬜ | InputService | Input mapping |
| Camera follow | 🟩 | CameraCore | Player tracking |
| Player health | ⬜ | HealthComponent | Damage/healing |

### 2.5 Enemy Implementation ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| MobEntity scene | 🟩 | Components | Enemy template |
| Basic AI (chase) | ⬜ | AISystem | Simple pursuit |
| Enemy spawning | ⬜ | FactorySystem | Wave spawning |
| Enemy death | ⬜ | FactorySystem | Death handling |
| Difficulty scaling | ⬜ | SpawnSystem | Time-based escalation |

**Phase 2 Deliverable**: Player can move, enemies spawn and chase, basic combat works.

---

## Phase 3: Combat & Loot (Weeks 9-12)

Implement the survivors-style combat and loot collection.

### 3.1 Chant System (Auto-Attacks) ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| ChantData resource | ⬜ | Data foundation | Chant definition |
| Chant slot system | ⬜ | ChantData | 4 active slots |
| Projectile chants | ⬜ | CombatSystem | Fire projectiles |
| Nova chants | ⬜ | CombatSystem | 360° attacks |
| Orbit chants | ⬜ | CombatSystem | Circling attacks |
| Area chants | ⬜ | CombatSystem | Ground effects |
| Chant cooldowns | ⬜ | Heart timers | Timing system |

### 3.2 Combat Expansion ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Damage numbers | ⬜ | CombatSystem | Floating text |
| Critical hits | ⬜ | StatsData | Crit chance/damage |
| Status effects | ⬜ | EffectData | Buffs/debuffs |
| EffectData resource | ⬜ | Data foundation | Effect definition |
| Effect application | ⬜ | CombatSystem | Apply effects |
| DOT/HOT processing | ⬜ | Heart timers | Periodic damage/heal |

### 3.3 Loot System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| LootTable resource | ⬜ | ItemData | Drop tables |
| Auto-collection | ⬜ | PhysicsSystem | Proximity pickup |
| XP orbs | ⬜ | XPSystem | Experience drops |
| XP magnet | ⬜ | XPSystem | Attraction effect |
| Level up system | ⬜ | XPSystem | Level progression |

### 3.4 Chest System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| ChestEntity scene | 🟩 | Components | Chest template |
| Chest spawning | ⬜ | SpawnSystem | Chest placement |
| Slot machine UI | ⬜ | UISystem | Loot reveal |
| Loot drop mechanics | ⬜ | LootSystem | Item generation |
| Chest interaction | ⬜ | InteractionComponent | Opening chests |

### 3.5 Voice System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Voice collection | ⬜ | XPSystem variant | Soul gathering |
| Voice counter UI | ⬜ | UiManager | HUD display |
| Voice banking | ⬜ | LodestoneEntity | Safe deposit |
| Voice loss on death | ⬜ | GameManager | Death penalty |

**Phase 3 Deliverable**: Full combat loop with auto-attacks, loot drops, XP, and Voices.

---

## Phase 4: Map & Extraction (Weeks 13-16)

Implement procedural maps and extraction mechanics.

### 4.1 Map System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| MapSystem | ⬜ | LevelData | Map management |
| Map boundaries | ⬜ | MapSystem | Level borders |
| Spawn zones | ⬜ | MapSystem | Enemy spawn areas |
| Tilemap loading | ⬜ | LevelData | Map visuals |
| Procedural generation | ⬜ | MapSystem | Random maps |

### 4.2 Extraction Mechanics ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| LodestoneEntity | ⬜ | GameEntity | Extraction point |
| Lodestone placement | ⬜ | MapSystem | Map positions |
| Extraction timer | ⬜ | Heart | Charging portal |
| Extraction UI | ⬜ | UiManager | Progress indicator |
| Successful extraction | ⬜ | GameManager | Win condition |

### 4.3 Wave System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| SpawnSystem | ⬜ | FactorySystem | Wave spawning |
| MobTable resource | 🟩 | MobData | Spawn tables |
| Escalation curves | ⬜ | SpawnSystem | Difficulty scaling |
| Wave announcements | ⬜ | UiManager | UI feedback |
| Boss waves | ⬜ | SpawnSystem | Special encounters |

### 4.4 Game Flow ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Level loading | 🟩 | LevelService | Load level data |
| Game start sequence | ⬜ | StateCore | Countdown/intro |
| Death handling | ⬜ | GameManager | Game over state |
| Extraction success | ⬜ | GameManager | Victory handling |
| Return to Dagsgard | ⬜ | StateCore | Mode transition |

**Phase 4 Deliverable**: Complete extraction loop from deployment to extraction/death.

---

## Phase 5: Downtime Mode (Weeks 17-20)

Implement the strategic downtime between extractions.

### 5.1 Fantasy OS Framework ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Desktop scene | ⬜ | MenuManager | OS desktop |
| Window system | ⬜ | UISystem | Window management |
| Taskbar | ⬜ | Desktop | Start bar |
| Icon system | ⬜ | Desktop | App icons |
| Window theming | ⬜ | UI resources | Visual style |

### 5.2 Core Applications ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Inventory window | ⬜ | InventorySystem | Item management |
| Character window | ⬜ | HeroService | Stats/equipment |
| Claims map window | ⬜ | LevelIndex | Level selection |
| Settings window | ⬜ | PrefService | Options |

### 5.3 Crafting System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| CraftData resource | ⬜ | Data foundation | Recipe definition |
| CraftSystem | ⬜ | MenuManager | Crafting logic |
| Forge window | ⬜ | CraftSystem | Crafting UI |
| Recipe unlocking | ⬜ | Progression | New recipes |
| Workbench types | ⬜ | CraftData | Specialized crafting |

### 5.4 Inventory System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| InventorySystem | ⬜ | MenuManager | Item management |
| Item storage | ⬜ | SaveData | Persistent items |
| Item stacking | ⬜ | ItemData | Stack management |
| Equipment slots | ⬜ | HeroService | Gear equipping |
| Item sorting | ⬜ | Inventory UI | Organization |

### 5.5 Persistence ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| SaveData structure | ⬜ | Data foundation | Save format |
| Save game | ⬜ | SaveService | Write to disk |
| Load game | ⬜ | SaveService | Read from disk |
| Auto-save | ⬜ | SaveService | Automatic saves |
| Save slots | ⬜ | SaveService | Multiple saves |

**Phase 5 Deliverable**: Functional downtime mode with crafting and inventory management.

---

## Phase 6: Progression & Content (Weeks 21-24)

Add meta-progression and content systems.

### 6.1 Quest System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| QuestData resource | ⬜ | Data foundation | Quest definition |
| QuestSystem | ⬜ | MenuManager | Quest tracking |
| Quest log window | ⬜ | QuestSystem | Quest UI |
| Quest objectives | ⬜ | QuestData | Completion tracking |
| Quest rewards | ⬜ | InventorySystem | Reward distribution |

### 6.2 NPC & Dialogue ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| DialogueData resource | ⬜ | Data foundation | Dialogue trees |
| DialogueSystem | ⬜ | MenuManager | Conversation logic |
| Dialogue window | ⬜ | DialogueSystem | Dialogue UI |
| NPC definitions | ⬜ | DialogueData | Character data |
| Dialogue choices | ⬜ | DialogueSystem | Branching paths |

### 6.3 Meta-Progression ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Voice donation | ⬜ | VoiceSystem | Permanent upgrades |
| Blessing tiers | ⬜ | Progression data | Unlock tiers |
| Permanent bonuses | ⬜ | StatsData | Stat increases |
| Unlock tracking | ⬜ | SaveData | Persist unlocks |

### 6.4 Skill System ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Skill definitions | ⬜ | Data foundation | Skill data |
| Skill experience | ⬜ | Gameplay tracking | Use-based XP |
| Skill levels | ⬜ | Progression | Level up skills |
| Skill bonuses | ⬜ | StatsData | Level benefits |

### 6.5 Idle Systems ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| IdleSystem | ⬜ | MenuManager | Background tasks |
| Garden plots | ⬜ | IdleSystem | Growing resources |
| Training | ⬜ | IdleSystem | Passive skills |
| Offline progress | ⬜ | SaveService | Time-based gains |

**Phase 6 Deliverable**: Complete meta-progression loop with quests and idle mechanics.

---

## Phase 7: Polish & Optimization (Weeks 25-28)

Performance, visual polish, and quality of life.

### 7.1 Visual Polish ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| CRT shader | ⬜ | Shaders | Visual effect |
| Screen shake | ⬜ | CameraCore | Combat feedback |
| Screen flash | ⬜ | CameraCore | Damage/level up |
| Particle effects | ⬜ | Visual assets | Combat VFX |
| Animation polish | ⬜ | Sprite assets | Smooth animations |

### 7.2 Audio Implementation ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Music tracks | ⬜ | AudioService | Background music |
| Combat SFX | ⬜ | AudioService | Attack sounds |
| UI sounds | ⬜ | AudioService | Interface feedback |
| Ambient audio | ⬜ | AudioService | Environmental |
| Audio settings | ⬜ | PrefService | Volume controls |

### 7.3 Performance ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Object pooling | ⬜ | FactorySystem | Reduce allocations |
| Spatial partitioning | ⬜ | Systems | Collision optimization |
| System throttling | ⬜ | Systems | Update frequency |
| Memory profiling | ⬜ | Testing | Identify leaks |
| Frame budget | ⬜ | Profiling | 60 FPS target |

### 7.4 Quality of Life ⬜

| Task | Status | Dependencies | Notes |
|------|--------|--------------|-------|
| Tutorials | ⬜ | UISystem | New player guidance |
| Tooltips | ⬜ | UISystem | Hover information |
| Notifications | ⬜ | UISystem | Event alerts |
| Keybinding | ⬜ | InputService | Custom controls |
| Accessibility | ⬜ | Settings | Options for all |

**Phase 7 Deliverable**: Polished, performant game ready for content expansion.

---

## Future Phases

### Phase 8: Content Expansion ⬜
- Additional enemy types
- More chants and weapons
- New level types and tiers
- Story quests

### Phase 9: Survivors Template Split ⬜
- Extract generic systems
- Remove extraction-specific code
- Create standalone template
- Documentation

### Phase 10: Commercial (RedGate) ⬜
- Unique art assets
- Full narrative
- Marketing materials
- Distribution

---

## Version Milestones

| Version | Phase | Features |
|---------|-------|----------|
| 0.1.0 | 1-2 | Core infrastructure, basic movement |
| 0.2.0 | 2-3 | Combat, auto-attacks, loot |
| 0.3.0 | 3-4 | Full extraction loop |
| 0.4.0 | 5 | Downtime mode foundation |
| 0.5.0 | 6 | Meta-progression, quests |
| 0.6.0 | 7 | Polish, optimization |
| 0.7.0 | 8 | Content expansion |
| 1.0.0 | 9-10 | Template release / Commercial |

---

## Risk Assessment

### High Risk
- **Performance with 5000+ entities**: Requires careful system design
- **Procedural generation complexity**: May need simplification
- **Save/load compatibility**: Version migrations

### Medium Risk
- **Fantasy OS complexity**: UI development time
- **Balance tuning**: Extensive playtesting needed
- **Scope creep**: Feature discipline required

### Low Risk
- **Core architecture**: Well-designed patterns
- **Godot 4.5 stability**: Mature engine
- **C# performance**: Strong language choice

---

*This roadmap will be updated as development progresses and priorities shift.*
