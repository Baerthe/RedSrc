# RedSrc/RedGate Design Documentation

> **Version**: 0.1.0  
> **Last Updated**: 2026-01-14  
> **Status**: Draft

## Overview

This documentation establishes the design blueprint for RedSrc, an open-source codebase for a **Survivor Extraction** game built with Godot Engine 4.5.1 and C# (.NET). The commercial implementation is called **RedGate**.

### What is RedGate?

RedGate is a unique hybrid game combining elements of:
- **Extraction Shooters**: Strategic loot gathering with permanent loss risk
- **Survivors Genre**: Wave-based combat with auto-attacking abilities (à la Vampire Survivors)
- **RPG Systems**: Character progression, crafting, and narrative elements

Players control Dwarven Prospectors who venture to the surface to gather resources and "Voices" (souls) for their underground civilization, while being hunted by the hostile Alfarish Union (The Flock).

### Core Design Philosophy

| Pillar | Description |
|--------|-------------|
| **Modularity** | Self-contained components, systems, and services for easy extension |
| **Data-Driven** | Game logic driven by Resource assets for designer accessibility |
| **Performance** | Centralized systems handling thousands of entities efficiently |
| **Separation of Concerns** | Clear boundaries between layers and responsibilities |
| **Designer-Friendly** | Content creation without code modification |
| **Fantasy OS Metaphor** | UI themed as a retro operating system interface |

## Document Structure

| Document | Description |
|----------|-------------|
| [Architecture](architecture.md) | Technical architecture, layered system design, and code conventions |
| [Core Systems](core-systems.md) | Infrastructure, services, managers, and system implementations |
| [Gameplay](gameplay.md) | Gameplay mechanics, loops, and player-facing features |
| [Data Structures](data-structures.md) | Data resources, components, and entity definitions |
| [UI & Presentation](ui-and-presentation.md) | Visual design, UI/UX, and the Fantasy OS theme |
| [Roadmap](roadmap.md) | Development phases, milestones, and implementation order |

## Game Modes

### Extraction Mode (GameManager)
The high-intensity action segment where players:
- Explore procedurally generated surface areas (Claims)
- Combat waves of increasingly aggressive enemies
- Gather loot through proximity-based auto-collection
- Race to Lodestones (extraction points) before being overwhelmed
- Risk losing all gathered resources upon death

### Downtime Mode (MenuManager)
The strategic management segment where players:
- Manage extracted resources in Dagsgard (the Prospectors' Warren)
- Craft and upgrade equipment at workbenches
- Interact with NPCs and progress narrative
- Plan loadouts for the next extraction
- Engage with idle/passive progression systems

## Technical Stack

- **Engine**: Godot 4.5.1 (.NET)
- **Language**: C# (.NET 8.0)
- **Architecture**: Data-driven ECS-inspired with strict separation of concerns
- **License**: AGPL-3.0-only (open source)

## Visual Style

- **Palette**: 2-bit limited color palette (4 colors)
- **Filter**: CRT shader overlay for retro aesthetic
- **Assets**: Based on 1-bit pack assets with 2-bit detail additions
- **Inspiration**: Loop Hero, Kingsway, classic Dwarf Fortress

## Quick Reference

### Namespace Structure
```
Core/           - Infrastructure (MainCore, EventCore, ServiceCore, etc.)
Core.Service/   - Primary services (AudioService, SaveService, etc.)
Data/           - Data resources and definitions
Component/      - Entity behavior components
Entity/         - Pre-configured entity scenes
Manager/        - Root scene managers (GameManager, MenuManager)
System/         - Logic controllers for entity groups
Utility/        - Helper classes and algorithms
```

### Folder Conventions
- Folders: `snake_case`
- Classes: `PascalCase`
- Names end with namespace role suffix (e.g., `MainCore`, `HeroData`, `PhysicsComponent`)

## Getting Started

1. Review the [Architecture](architecture.md) document for technical foundation
2. Study [Core Systems](core-systems.md) to understand infrastructure
3. Read [Gameplay](gameplay.md) for mechanics implementation details
4. Reference [Data Structures](data-structures.md) when creating content
5. Follow the [Roadmap](roadmap.md) for development priorities

---

*This documentation is evolving. Updates will reflect design decisions and implementation changes throughout development.*
