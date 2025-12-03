# About This Project
The concept behind this project is to create a base level project for use in more than one top-down 2D Godot-made game. It leans more heavily on the "C# as the engine" philosophy, using Godot as the backend and supporting framework. The idea is to create a solid architecture that can be used as a foundation for multiple games, with the ability to easily add new features, assets, and mechanics.

The primary among these is a hybrid game that combines elements of extraction shooters and the survivors genre (not survival or survival horror). It is not intended to be a full survival game but rather a game where players must extract from dangerous situations while managing limited resources and making strategic decisions to ensure their survival, all while being besieged by hostile entities. Though this project is open source and (currently) uses open assets, the end goal is to create a commercial game. At some point during the development process, key parts of this project will be split off into a survivors Godot template (without extraction elements) that can be used by other developers to create their own survivors games.

Unlike a lot of traditional Godot projects, this project deploys a Data-Driven, Entity-Component-System inspired model; prefering composition over inheritance with a strict seperation of concerns. The idea behind this was to maximize the power of C# in terms of code re-use, modularity, and performance, while still working harmoniously with Godot's scene-based workflow. This architecture also allows for easy expansion of the game mechanics and systems, as new components can be created and attached to entities without modifying existing code; in theory.

# The Architecture
RedSrc is a 2D game. Scripts are organized into namespaces and (matching) folders based on their functionality and role within the game. The architecture is designed to facilitate easy addition of new features, assets, and mechanics. The layered structure promotes separation of concerns, making it easier to maintain and expand the codebase while also allowing designers, if applicable, to create new content without needing to modify code. Because of this, the architecture is more complex than a typical Godot project, it is basically an extra "engine layer" between the game and Godot; but it provides a solid foundation for building a modular and extensible game.

This document outlines a tiered architecture:
Interfaces are contained, when relevant, in the base folder of each namespace under /interface/.
Folders and classes are named non-plural for clarity, all folders are snake_case, all classes are PascalCase and named after their namespace and role as a suffix (e.g. MainCore, PhysiscsDataComponent, MobEntityData).

- ## Layer 0: Core Infrastructure
- Namespace: Core
    - The core infrastructure holding everything together. All of the Core scripts are singleton nodes attached to (or are) MainCore.
    - MainCore; The root node of the scene tree, handles initialization and global orchestration.
        - CameraCore; Manages the main camera and its behavior.
        - EventCore; A global event bus for decoupled communication. (We do not use Godot signals for this purpose; signals are slower and less flexible.)
            - IEvent; The base interface for all events.
            - /events/; A folder containing all event classes.
        - StateCore; Manages what gamestate we are in and how to change them. It also stores references to globally relevant game data like what the current loaded level is, unlocked content, etc.
            - StateEnum; An enumeration of all possible game states.
        - ContextCore; Manages the lifecycle of persistent `Services`. On startup, it registers all core services with the `Registry`. Allows access to services throughout the engine through methods that access the `Registry`. Also managers registration, deregistration, and access to `Systems` through the `Registry`.
            - Registry; A static singleton class that acts as a central registry for all global `Services` and `Systems`. Other parts of the engine can access singletons from this registry without needing direct references.

- ## Layer 1: Primary Services
- Namespace: Service
    - The essential services that provide core functionality to the game. These are plain C# classes (non-Nodes) that are instantiated by `ContextCore` and registered with the `Registry`, making them globally accessible and persistent.
    - AudioService: Manages all audio playback and settings.
    - EntityService: Manages the lifecycle of all entities in the game. It uses the `Registry` to get references to the currently active `Systems` to perform component registration.
    - InputService: Handles player input and maps it to game actions.
    - SaveService: Manages saving and loading game data.
    - SceneService: Manages scene transitions and loading.
    - SettingsService: Manages game settings and preferences.

- ## Layer 2: Data Abstractions
- Namespace: Data
    - Data structures and definitions used throughout the game. These are primarily plain C# classes inheriting from `Godot.Resource` used to define shared, stateless data assets. They can be created and managed in the editor as `.tres` files.
    - ICommon: Base interface
        - Implementations for specific data types shared between specific entities. These are "pure data" resources.
        /common/; A folder containing all common data classes.
            - InfoCommon: Basic information shared by all entities (e.g., name, description). This can be exported on a component to link to it.
            - AssetCommon: Asset references shared by all entities (e.g., sprites, sounds).
    - SaveData: Structures for saving and loading game state.
    - LevelData: Structures defining level layouts and properties.

- ## Layer 2 (as well): Behavior Components
- Namespace: Component
    - Modular components that can be attached to entities to define their behavior and characteristics. These are nodes that are attached to Entity nodes. Components are not just generic Nodes; they are specialized, "stylized" Godot nodes that are self-contained.
    - Components primarily act as **data containers**, defining the properties and state of an entity. They expose their data to the Godot Inspector using the `[Export]` attribute, allowing for easy configuration. To achieve maximum performance and scalability (e.g., for 5000+ entities), high-frequency logic (like movement) is handled by `Systems` that operate on this data, rather than by individual components in `_Process` or `_PhysicsProcess`. This follows a more data-oriented design pattern, improving cache efficiency and reducing function call overhead.
    - IComponent: Base interface for all components.
        - Implementations for specific component types (e.g., HealthComponent, PhysicsComponent).
            - HealthComponent: A data container for an entity's health. Exports properties like `MaxHealth`.
            - PhysicsComponent: A data container for an entity's movement properties. Exports properties like `Speed`. It does **not** contain movement logic itself.
                Components can have child nodes as required by Godot; e.g., PhysicsComponent is a CharacterBody2D that has a CollisionShape2D child node. This is why they are scenes added to nodes, not just scripts.
            - InventoryComponent: Manages an entity's inventory.
            - AIComponent: Can be used to store AI state and simple steering data.

    - Component Self-Validation:
        To ensure components are configured correctly in the editor, they can implement the `_GetConfigurationWarnings()` method. This allows a component to check its own state and report errors (e.g., a missing reference or an invalid value) directly in the editor, providing immediate feedback and enforcing structural rules without sacrificing flexibility.

    - System Registration:
        When an entity is instantiated, the `EntityService` is responsible for wiring its components to the correct systems. It queries the `Registry` to get the currently registered systems (e.g., `Registry.Get<PhysicsSystem>()`) and passes those references to the new components, which then register themselves. When a component exits the tree, it unregisters from its system. This ensures each system always has a direct, up-to-date list of the components it needs to process.

    - Inter-Component Communication:
        Components do not communicate with each other directly. They are directed by Systems, which modify their state. For example, an `AISystem` might set a `TargetDirection` variable on an entity, and the `PhysicsComponent` reads that variable every physics frame to adjust its movement. This keeps components decoupled and focused on executing their specific behaviors.

- ## Layer 3: Building Block Entities
- Namespace: Entity
    - The entities that populate the game world. Entities are defined as pre-configured Godot Scenes (`.tscn` files).
    These scenes act as templates or "recipes". They are instantiated at runtime by the `EntityService`, often from an object pool for performance. This approach leverages Godot's highly optimized scene instantiation system.
    - IEntity: Base interface for all entities.
    - GameEntity: The main entity node that repersents all objects that could appear in the game world. It can also serve as a simple data bus for its components.
    - MenuEntity: A specialized entity for menu items and UI elements.

    - Scene Inheritance for Structure and Variation:
        To enforce a common structure (e.g., all mobs must have health and physics), a "BaseMob.tscn" can be created. Specific mobs like a "Goblin" are then created as *inherited scenes* from this base. This guarantees the presence of required components while allowing for additional components and property overrides in the derived scene. This maintains the principle of composition while providing architectural strictness.

    - Godot Group:
        Entities, when created, are also added to relevant Godot groups based on their type. This allows for easy debugging and broad-level queries, but it is not the primary mechanism for system control.
        Thus for each entity type, there is a matching Godot group name. These are named non-plural for clarity as single names.
            - GameEntity -> "Game"
            - MenuEntity -> "Menu"

        Example: The "Goblin" Entity Scene Tree
            The scene tree for a "Goblin" entity would be a self-contained, pre-configured scene (`Goblin.tscn`) that inherits from a base mob scene.
            Goblin (`Goblin.tscn`, root is a Node2D with GameEntity.cs, inherited from `BaseMob.tscn`)
                - The components below are part of the scene itself. Their properties are set directly in the Inspector.
                Physics (PhysicsComponent.tscn, root is a CharacterBody2D with PhysicsComponent.cs)
                    - Speed = 50
                    CollisionShape2D (A child of the physics body, as required by Godot)
                Health (HealthComponent.tscn, root is a Node with HealthComponent.cs)
                    - MaxHealth = 100
                Render (RenderComponent.tscn, root is a Sprite2D with RenderComponent.cs)
                Hitbox (HitboxComponent.tscn, root is an Area2D with HitboxComponent.cs)
                    CollisionShape2D (A child of the area, as required by Godot)
                AI (AIComponent.tscn, root is a Node2D with AIComponent.cs)

- ## Layer 4: Orchestration Managers
- Namespace: Manager
    - Managers are the two main root-node Scenes that control the game; the GameManager and MenuManager. These are handled by StateCore to switch between them based on the current game state. When a manager scene is loaded, it is responsible for finding all of its child `Systems` and registering them with the global `Registry`. When the scene is unloaded, it unregisters its systems.
    - GameManager: The main scene for gameplay. On `_Ready`, it registers all relevant game systems (e.g., `PhysicsSystem`, `AISystem`) with the `Registry`.
    - MenuManager: The main scene for menus and UI. On `_Ready`, it registers all relevant menu systems (e.g., `UISystem`) with the `Registry`.

- ## Layer 5: Controlling Logic Systems
- Namespace: System
    - Systems are nodes that control and manage entities within the game. They are designed to handle the core logic, acting as the "brains" of the operation. They operate by iterating over groups of components, reading their state, and writing back changes.
    - Systems choose *what* an entity should do, and the components then define *how* it gets done.
    This logic can be executed on a throttled timer for low-frequency updates (e.g., AI decision-making at 10Hz) or every frame in `_Process` or `_PhysicsProcess` for high-frequency updates (e.g., movement). This centralized approach is critical for performance, as it allows for processing thousands of entities in tight, cache-friendly loops.
    - ISystem: Base interface for all systems. It contains methods for initialization, updating, and handling events; all of which are called by their parent Manager.
        - Implementations for specific system types.
            - PhysicsSystem: Executes in `_PhysicsProcess`. It iterates through all entities with a `PhysicsComponent`, calculates velocity based on their data (e.g., `Speed`, `Direction`), and calls `move_and_slide`.
            - AISystem: The "brain" for NPCs. On a timer, it scans the environment, chooses targets, and sets the state on an entity's `AIComponent` (e.g., `State = "Attacking"`, `Target = Player`).
            - CombatSystem: Manages combat interactions. On a timer, it might process damage-over-time effects or check for cooldown expirations.
            - CraftSystem: Manages crafting mechanics and recipes.
            - ClockSystem: Manages in-game time and sends out time-related events for other systems to use.
            - InventorySystem: Manages inventory interactions and item usage.
            - InteractionSystem: Handles interactions between entities and the game world.
            - MapSystem: Manages the active game map, potentially spawning entities based on player location.
            - UISystem: Manages UI elements and interactions.
            - XPSystem: Manages experience points and leveling mechanics.