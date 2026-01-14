# RTS Game Design Documentation

This directory contains comprehensive design documentation for an 8-bit real-time tactical strategy (RTS) game built in Godot 4.5 (.NET), inspired by classics like Command & Conquer and Warcraft 2/3.

## Document Index

### [01 - RTS Design Overview](01-RTS-Design-Overview.md)
**Purpose**: High-level vision and project scope  
**Contents**:
- Game concept and design pillars
- Target audience and unique selling points
- Game modes and scope boundaries
- Success criteria and MVP definition

**Read this first** to understand the overall vision and goals.

---

### [02 - Architecture and Systems Design](02-Architecture-and-Systems.md)
**Purpose**: Technical architecture and system relationships  
**Contents**:
- System architecture hierarchy
- Core system components (Unit, Building, Resource, Combat, AI)
- Data layer architecture
- Performance considerations
- Integration with existing RedSrc codebase

**Essential for**: Developers implementing core systems.

---

### [03 - Core Gameplay Mechanics](03-Core-Gameplay-Mechanics.md)
**Purpose**: Detailed gameplay specifications  
**Contents**:
- Player input and camera controls
- Unit selection and commands
- Resource management mechanics
- Combat mechanics and damage calculation
- Building and production systems
- Technology upgrades and fog of war

**Essential for**: Designers and developers implementing gameplay features.

---

### [04 - Units and Buildings Systems](04-Units-and-Buildings.md)
**Purpose**: Complete unit and building specifications  
**Contents**:
- Two faction designs (Humans and Orcs)
- Detailed unit stats and abilities
- Building types and functions
- Balance considerations
- Cost efficiency calculations
- Counter systems and interactions

**Essential for**: Balance design and content creation.

---

### [05 - AI and Pathfinding Systems](05-AI-and-Pathfinding.md)
**Purpose**: AI behavior and navigation implementation  
**Contents**:
- Pathfinding algorithms (A*, Flow Fields)
- Navigation system integration
- AI controller architecture
- AI difficulty levels
- AI personalities (Aggressive, Defensive, Economic, Balanced)
- Performance optimization strategies

**Essential for**: AI and pathfinding implementation.

---

### [06 - UI/UX Design](06-UI-UX-Design.md)
**Purpose**: User interface and experience specifications  
**Contents**:
- Screen layout and HUD design
- Menu systems and navigation flows
- Control schemes (mouse, keyboard, gamepad)
- Visual feedback and notifications
- Accessibility features
- 8-bit aesthetic guidelines

**Essential for**: UI developers and UX designers.

---

### [07 - Technical Implementation Plan](07-Technical-Implementation-Plan.md)
**Purpose**: Detailed development roadmap and tasks  
**Contents**:
- Phase-by-phase implementation plan (Weeks 1-12)
- Task breakdowns with checklists
- Code examples and structure
- Integration points with existing codebase
- Testing strategies
- Risk mitigation

**Essential for**: Project planning and sprint organization.

---

### [08 - Asset and Data Structure](08-Asset-and-Data-Structure.md)
**Purpose**: Asset pipeline and data organization  
**Contents**:
- Visual asset requirements and organization
- Audio asset specifications
- Data file structure (.tres resources)
- Scene template organization
- Asset creation guidelines (8-bit style)
- Import settings and optimization

**Essential for**: Artists, audio designers, and content creators.

---

### [09 - Development Roadmap](09-Development-Roadmap.md)
**Purpose**: Complete project timeline and milestones  
**Contents**:
- 7 development phases (24+ weeks)
- Milestone definitions and success criteria
- Resource requirements
- Risk management
- Post-release plans
- Review and adaptation procedures

**Essential for**: Project managers and stakeholders.

---

## Quick Start Guide

### For Project Managers
1. Read: [01-RTS-Design-Overview](01-RTS-Design-Overview.md)
2. Read: [09-Development-Roadmap](09-Development-Roadmap.md)
3. Review: All documents for team assignments

### For Developers
1. Read: [01-RTS-Design-Overview](01-RTS-Design-Overview.md)
2. Read: [02-Architecture-and-Systems](02-Architecture-and-Systems.md)
3. Read: [07-Technical-Implementation-Plan](07-Technical-Implementation-Plan.md)
4. Reference: Other docs as needed during implementation

### For Designers
1. Read: [01-RTS-Design-Overview](01-RTS-Design-Overview.md)
2. Read: [03-Core-Gameplay-Mechanics](03-Core-Gameplay-Mechanics.md)
3. Read: [04-Units-and-Buildings](04-Units-and-Buildings.md)
4. Reference: [06-UI-UX-Design](06-UI-UX-Design.md) for interface work

### For Artists/Audio Designers
1. Read: [01-RTS-Design-Overview](01-RTS-Design-Overview.md)
2. Read: [08-Asset-and-Data-Structure](08-Asset-and-Data-Structure.md)
3. Reference: [06-UI-UX-Design](06-UI-UX-Design.md) for UI assets

---

## Document Status

| Document | Version | Status | Last Updated |
|----------|---------|--------|--------------|
| 01 - Design Overview | 1.0 | Draft for Review | 2026-01-14 |
| 02 - Architecture | 1.0 | Draft for Review | 2026-01-14 |
| 03 - Gameplay Mechanics | 1.0 | Draft for Review | 2026-01-14 |
| 04 - Units & Buildings | 1.0 | Draft for Review | 2026-01-14 |
| 05 - AI & Pathfinding | 1.0 | Draft for Review | 2026-01-14 |
| 06 - UI/UX Design | 1.0 | Draft for Review | 2026-01-14 |
| 07 - Implementation Plan | 1.0 | Draft for Review | 2026-01-14 |
| 08 - Asset Structure | 1.0 | Draft for Review | 2026-01-14 |
| 09 - Development Roadmap | 1.0 | Draft for Review | 2026-01-14 |

---

## Key Concepts at a Glance

### Technical Foundation
- **Engine**: Godot 4.5 with .NET/C#
- **Target Resolution**: 854x480 (16:9)
- **Visual Style**: 8-bit pixel art (1-bit Kenney pack)
- **Architecture**: Component-based, event-driven
- **Existing Base**: RedSrc codebase patterns (Core, Data, Manager, Event)

### Game Features (MVP)
- **Factions**: 2 (Humans, Orcs)
- **Units**: 5-7 per faction
- **Buildings**: 7-10 per faction
- **Resources**: Gold, Wood, Supply
- **Game Modes**: Skirmish vs AI, Campaign (3-5 missions)
- **AI**: 3 difficulty levels

### Performance Targets
- **FPS**: 60 on mid-range hardware
- **Max Units**: 200 simultaneously
- **Load Times**: < 5 seconds
- **Memory**: < 500MB

### Development Timeline
- **Phase 1-4** (Weeks 1-12): MVP completion
- **Phase 5-6** (Weeks 13-20): Advanced features
- **Phase 7** (Weeks 21-24): Polish and release
- **Total**: ~24 weeks for complete release

---

## Design Principles

1. **Surgical Changes**: Build on existing RedSrc architecture
2. **Data-Driven**: All units, buildings, and mechanics defined in .tres resources
3. **Event-Driven**: Use existing EventCore for system communication
4. **Performance First**: Optimize for 200+ units at 60 FPS
5. **Accessibility**: Colorblind modes, clear UI, keyboard shortcuts
6. **Modular**: Easy to extend with new units, factions, and features

---

## Review Process

### Document Review Cycle
1. **Draft** (Current): Initial documentation complete
2. **Review**: Team reviews and provides feedback
3. **Revision**: Updates based on feedback
4. **Approved**: Ready for implementation
5. **Living**: Updated as project evolves

### Requesting Changes
- Create an issue or PR with suggested changes
- Reference specific document and section
- Provide rationale for change
- Tag relevant team members

### Approval Authority
- Design decisions: Lead Designer
- Technical decisions: Lead Developer
- Scope changes: Project Manager
- Budget/Timeline: Stakeholders

---

## Related Resources

### External References
- [Godot 4.5 Documentation](https://docs.godotengine.org/en/stable/)
- [C# in Godot](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/index.html)
- [Kenney Assets](https://kenney.nl/assets) (8-bit pack)
- [Classic RTS Design Articles](https://www.gamedeveloper.com/)

### Project Files
- Main README: `/README.md`
- Project Config: `/project.godot`
- Source Code: `/src/`
- Assets: `/asset/`
- Scenes: `/scene/`

---

## Glossary

**MVP**: Minimum Viable Product - core features for playable release  
**RTS**: Real-Time Strategy game genre  
**APM**: Actions Per Minute (control metric)  
**DPS**: Damage Per Second  
**FPS**: Frames Per Second (performance metric)  
**Tech Tree**: Upgrade and research progression system  
**Fog of War**: Hidden/explored/visible area system  
**Lockstep**: Networking synchronization model  

---

## Version History

### v1.0 (2026-01-14)
- Initial documentation suite created
- 9 comprehensive design documents
- Based on Command & Conquer / Warcraft 2/3 concepts
- Designed for Godot 4.5 (.NET) with RedSrc foundation

---

## Contact and Contributions

For questions, suggestions, or contributions to this design documentation:

1. Create an issue in the repository
2. Tag with `documentation` label
3. Reference specific document(s)
4. Provide clear description of question/change

---

**Last Updated**: 2026-01-14  
**Documentation Version**: 1.0  
**Project Phase**: Planning / Design
