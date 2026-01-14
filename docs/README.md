# RedGate Design Documentation

This directory contains the comprehensive design documentation for RedGate, a tactical dungeon RPG being developed using the RedSrc codebase.

## Document Overview

| Document | Description |
|----------|-------------|
| [00-game-design-document.md](design/00-game-design-document.md) | High-level game vision, design pillars, and core loops |
| [01-technical-architecture.md](design/01-technical-architecture.md) | Layered architecture, code structure, and system organization |
| [02-systems-design.md](design/02-systems-design.md) | Detailed specifications for all game systems |
| [03-component-design.md](design/03-component-design.md) | Modular component specifications and interfaces |
| [04-data-structures.md](design/04-data-structures.md) | Data resource definitions and structures |
| [05-implementation-roadmap.md](design/05-implementation-roadmap.md) | Development phases, tasks, and milestones |

## Quick Start

1. **New to the project?** Start with the [Game Design Document](design/00-game-design-document.md) for an overview
2. **Implementing a feature?** Reference the [Systems Design](design/02-systems-design.md) for specifications
3. **Creating content?** See [Data Structures](design/04-data-structures.md) for resource definitions
4. **Planning work?** Check the [Implementation Roadmap](design/05-implementation-roadmap.md)

## Game Concept Summary

RedGate is a **single-player, 8-bit style real-time tactical strategy RPG** featuring:

- **Hero System**: Class-based characters with unique abilities (inspired by Warcraft 3)
- **Dungeon Crawling**: Procedurally generated multi-floor dungeon exploration
- **Real-Time Combat**: Ability-based tactical combat with cooldowns
- **Town Hub**: Persistent safe zone with NPCs, shops, quests, and crafting
- **Progression**: Experience, leveling, equipment, and skill trees
- **Data-Driven**: Modular architecture enabling easy content creation

## Architecture Overview

The project follows an ECS-inspired layered architecture:

```
Layer 1A: Core Infrastructure (MainCore, EventCore, StateCore)
Layer 1B: Primary Services (Audio, Save, Input, Scene)
Layer 2A: Data Abstractions (Resources, .tres files)
Layer 2B: Behavior Components (Health, Physics, Combat, AI)
Layer 3:  Building Block Entities (Hero, Enemy, Item, NPC)
Layer 4:  Orchestration Managers (DungeonManager, TownManager)
Layer 5:  Controlling Logic Systems (Combat, AI, Loot, Quest)
Layer 6:  Assisting Utilities (Generators, Calculators)
```

## Technology Stack

- **Engine**: Godot Engine 4.5+
- **Language**: C# (.NET)
- **License**: AGPL-3.0-only

## Development Status

This documentation establishes the foundation for adapting RedSrc from a "Survivor Extraction" game to a tactical dungeon RPG. The codebase will be incrementally refactored following the [Implementation Roadmap](design/05-implementation-roadmap.md).

## Contributing

When contributing to this project:

1. Review relevant design documents before implementation
2. Follow the established architecture patterns
3. Create data resources (`.tres` files) for configurable content
4. Use the EventCore for cross-system communication
5. Register services and systems with ServiceCore

## Document Maintenance

These documents are living specifications. When making significant changes:

1. Update the relevant design document
2. Note the change in the document's history section
3. Ensure cross-references remain accurate
