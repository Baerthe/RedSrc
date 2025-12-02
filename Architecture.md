# About This Project
The concept behind this project is to create a base level project for use in more than one top-down 2D Godot-made game.

The primary among these is a hybrid game that combines elements of extraction shooters and the survivors genre (not survival or survival horror). It is not intended to be a full survival game but rather a game where players must extract from dangerous situations while managing limited resources and making strategic decisions to ensure their survival, all while being besieged by hostile entities. Though this project is open source and (currently) uses open assets, the end goal is to create a commercial game. At some point during the development process, key parts of this project will be split off into a survivors Godot template (without extraction elements) that can be used by other developers to create their own survivors games.

Unlike a lot of traditional Godot projects, this project deploys a Data-Driven Entity-Component-System model; prefering composition over inheritance with a strict seperation of concerns. The idea behind this was to maximize the power of C# in terms of code re-use, modularity, and performance. This architecture also allows for easy expansion of the game mechanics and systems, as new components can be created and attached to entities without modifying existing code; in theory.

# The Architecture
RedSrc is a 2D game. Scripts are organized into namespaces and (matching) folders based on their functionality and role within the game. The architecture is designed to facilitate easy addition of new features, assets, and mechanics. The layered structure promotes separation of concerns, making it easier to maintain and expand the codebase while also allowing designers, if applicable, to create new content without needing to modify code. Because of this, the architecture is more complex than a typical Godot project, it is basically an extra "engine layer" between the game and Godot; but it provides a solid foundation for building a modular and extensible game.

This document outlines a tiered architecture:
Interfaces are contained, when relevant, in the base folder of each namespace under /interface/.
Folders and classes are named non-plural for clarity, all folders are snake_case, all classes are PascalCase.

- Level 0:
Namespace: Core
    The core infrastructure holding everything together. All of the Core scripts are singleton nodes attached to (or are) MainCore.
    - MainCore; The root node of the scene tree, handles initialization and global orchestration.
        - CameraCore; Manages the main camera and its behavior.
        - EventCore; A global event bus for decoupled communication. (We do not use Godot signals for this purpose; signals are slower and less flexible.)
            - IEvent; The base interface for all events.
            - /events/; A folder containing all event classes.
        - StateCore; Manages what gamestate we are in and how to change them. It also stores references to globally relevant game data like what the current loaded level is, unlocked content, etc.
            - StateEnum; An enumeration of all possible game states.
        - ServiceCore; Manages all services in the game
            - ServiceContainer; A container for all services, preventing the need for direct references between services.

- Level 1:
Namespace: Service
    The essential services that provide core functionality to the game. These are singletones contained within the ServiceContainer attached to ServiceCore, they are not nodes. These are Dependency Injected into other nodes that need them, specifically Systems that control Entities.
    - AudioService: Manages all audio playback and settings.
    - EntityService: Manages the lifecycle of all entities in the game. Factory methods for creating entities.
    - InputService: Handles player input and maps it to game actions.
    - SaveService: Manages saving and loading game data.
    - SceneService: Manages scene transitions and loading.
    - SettingsService: Manages game settings and preferences.
    - TemplateService: Manages entity templates and their data.

- Level 2:
Namespace: Data
    Data structures and definitions used throughout the game. These are *all* plain C# classes used to define resources, they are not nodes. The idea is that the data resources will be highly reusable and modular. "Recipe Cards"
    - ICommon: Base interface
        - Implementations for specific data types shared between specific entities. These are "pure data" componenets
        /common/; A folder containing all common data classes.
            - InfoCommon: Basic information shared by all entities (e.g., name, description).
            - AssetCommon: Asset references shared by all entities (e.g., sprites, sounds).
    - IComponentData: Data interface for various components.
        - Implementations for specific component types (e.g., HealthComponent, MovementComponent). These are data injected into relevant component nodes to define their behavior.
        /component/; A folder containing all component data classes.
    - IEntityData: Data interface for entities. Holds an array of IComponentData to define what components an entity has and contains some common data (All have InfoCommon and AssetCommon). The interface basic metadata like an icon and unique ID.
        - Implementations for specific entity types (e.g., MobData, ItemData). These are data injected into entity nodes to define their overall structure and behavior.
        /entity/; A folder containing all entity data classes.
    - SaveData: Structures for saving and loading game state.
    - LevelData: Structures defining level layouts and properties.

- Level 3:
Namespace: Component
    Modular components that can be attached to entities to define their behavior and characteristics. These are nodes that are attached to Entity nodes. They receive data from IEntityData implementations to define what the entity has; they're injected with IComponentData implementations.
    These are nodes inside of a scene that is instanced into the Entity node at runtime based on the IEntityData the Entity was created with.
    Entities queue and call ``Inject(IComponentData data)`` on each of their components to set themsleves up based on the data provided.
    - IComponent: Base interface for all components.
        - Implementations for specific component types (e.g., HealthComponent, MovementComponent).
            - HealthComponent: Manages health and damage for an entity.
            - MovementComponent: Handles movement logic for an entity.
            - InventoryComponent: Manages an entity's inventory.
            - AIComponent: Controls AI behavior for NPCs.

- Level 4:
Namespace: Entity
    The entities that populate the game world. These are nodes that represent game objects, they are composed of multiple components to define their behavior and characteristics, look and feel. These are generic templates that can be instantiated with specific data at runtime.
    They are controlled by Systems that act on their components; so they do not have much logic themselves.
    Calls ``Inject(IEntityData data)`` to set themselves up based on the data provided. They are built by EntityService factory methods.
    - IEntity: Base interface for all entities.
    - GameEntity: The main entity node that repersents all objects that could appear in the game world.
    - MenuEntity: A specialized entity for menu items and UI elements.

- Level 5:
Namespace: Manager
    Managers are the two main root-node Scenes that control the game; the GameManager and MenuManager. These are handled by StateCore to switch between them based on the current game state.
    - GameManager: The main scene for gameplay. Contains the game world, player entity; it is the root node that controls relevant Game Systems.
    - MenuManager: The main scene for menus and UI. Contains menu entities; it is the root node that controls relevant Menu Systems.

- Level 6:
Namespace: System
    Systems are nodes that control and manage entities within the game. They operate on entities and their components to implement game mechanics and logic. These are children of either GameManager or MenuManager depending on their purpose.
    To contain all the logic for a specific domain of behavior. Systems are the "verbs" that act upon the "nouns" (entities/components).
    - ISystem: Base interface for all systems. It contains methods for initialization, updating, and handling events; all of which are called by their parent Manager.
        - Implementations for specific system types.
            - AISystem: Controls AI behavior for NPCs.
            - CombatSystem: Manages combat interactions and damage calculations.
            - CraftSystem: Manages crafting mechanics and recipes.
            - ClockSystem: Manages in-game time and sends out time-related events.
            - InventorySystem: Manages inventory interactions and item usage.
            - InteractionSystem: Handles interactions between entities and the game world.
            - MapSystem: Manages the active game map.
            - UISystem: Manages UI elements and interactions.
            - XPSystem: Manages experience points and leveling mechanics.