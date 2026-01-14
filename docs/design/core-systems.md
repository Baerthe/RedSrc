# Core Systems Design

> **Version**: 0.1.0  
> **Last Updated**: 2026-01-14

## Overview

This document details the implementation design for RedSrc's core infrastructure, services, managers, and game systems. Each system is described with its responsibilities, interfaces, and integration points.

## Core Infrastructure (Layer 1A)

### MainCore

The root singleton node orchestrating the entire game.

```csharp
public sealed partial class MainCore : Node2D
{
    // Singleton access
    public static MainCore Instance { get; private set; }
    
    // Core nodes (exported for editor assignment)
    public CameraCore CameraCore { get; private set; }
    public ContextCore ContextCore { get; private set; }
    public EventCore EventCore { get; private set; }
    public ServiceCore ServiceCore { get; private set; }
    public StateCore StateCore { get; private set; }
    
    // Managers
    public GameManager GameManager { get; private set; }
    public MenuManager MenuManager { get; private set; }
    public UiManager UiManager { get; private set; }
    
    // Data indices
    public EntityIndex EntityTemplates { get; private set; }
    public HeroIndex Heroes { get; private set; }
    public ItemIndex Items { get; private set; }
    public LevelIndex Levels { get; private set; }
    public WeaponIndex Weapons { get; private set; }
}
```

**Responsibilities:**
- Validate all critical nodes on startup
- Initialize core systems in correct order
- Provide singleton access to global systems
- Handle fatal initialization errors

### EventCore

Global event bus with timing system.

```csharp
public sealed partial class EventCore : Node2D
{
    public Heart Heart { get; private set; }
    
    // Event methods
    void Subscribe<T>(Action handler);
    void Subscribe<T>(Action<IEvent> handler);
    void Unsubscribe<T>(Action handler);
    void Unsubscribe<T>(Action<IEvent> handler);
    void Publish<T>();
    void Publish<T>(IEvent eventData);
    void UnsubscribeAll();
}
```

#### Heart Timing System

```csharp
public partial class Heart : Node
{
    // Timer management
    void BuildTimer<T>(string name, float interval, bool oneShot, bool autoStart);
    void StartTimer(string name);
    void StopTimer(string name);
    void PauseAllTimers();
    void ResumeAllTimers();
    
    // Time tracking
    float GameTime { get; }        // Total extraction time
    int GameMinute { get; }        // Current in-game minute
    int DifficultyLevel { get; }   // Escalation level
}
```

#### Event Types

**Timing Events:**
```csharp
PulseTimeout       // 20 Hz - High frequency game updates
SlowPulseTimeout   // 5 Hz - Lower frequency updates
MobSpawnTimeout    // Mob wave triggers
ChestSpawnTimeout  // Loot container spawning
GameTimeout        // Per-minute game progression
StartingTimeout    // Initial countdown (one-shot)
```

**Game Events:**
```csharp
// State events
StateEvent(State newState)
InitEvent
DebugModeEvent

// Data events
IndexEvent(indices...)
LoadingProgress(int percent)

// Gameplay events
DamageEvent(IEntity target, uint amount, IEntity source)
DeathEvent(IEntity entity)
LootCollectedEvent(ItemData item, int quantity)
ExtractionEvent(bool success)
LevelUpEvent(int newLevel)

// XP events
XPDropEvent(Vector2 position, int amount)
XPCollectedEvent(int amount)
```

### ServiceCore

Service registry and resolution.

```csharp
public sealed partial class ServiceCore : Node2D
{
    internal static Registry ServiceRegistry { get; private set; }
    
    // Service accessors
    static IAudioService AudioService();
    static IHeroService HeroService();
    static IPrefService PrefService();
    static ILevelService LevelService();
    static ISaveService SaveService();
    static IInputService InputService();
    static ISettingsService SettingsService();
    static IStateService StateService();
}
```

#### Registry Implementation

```csharp
internal class Registry
{
    private Dictionary<Type, object> _services;
    
    void Register<TInterface, TImplementation>() where TImplementation : TInterface, new();
    void Register<TInterface>(TInterface instance);
    T Resolve<T>();
    bool TryResolve<T>(out T service);
    void Unregister<T>();
    void Clear();
}
```

### StateCore

Game state management.

```csharp
public sealed partial class StateCore : Node2D
{
    public GameState CurrentState { get; private set; }
    
    void ChangeState(GameState newState);
    void PushState(GameState state);
    void PopState();
}

public enum GameState
{
    Boot,           // Initial loading
    MainMenu,       // Title screen
    Loading,        // Scene transition
    Playing,        // Active extraction
    Paused,         // Game paused
    Downtime,       // Between extractions
    GameOver,       // Extraction failed
    Extracted       // Successful extraction
}
```

### CameraCore

Camera management and effects.

```csharp
public sealed partial class CameraCore : Node2D
{
    public Camera2D MainCamera { get; private set; }
    
    void FollowTarget(Node2D target);
    void SetBounds(Rect2 bounds);
    void Shake(float intensity, float duration);
    void Flash(Color color, float duration);
    void SetZoom(float zoom);
    void LerpToPosition(Vector2 position, float duration);
}
```

### ContextCore

Global context and game data.

```csharp
public sealed partial class ContextCore : Node2D
{
    // Current session data
    public HeroData CurrentHero { get; set; }
    public LevelData CurrentLevel { get; set; }
    public SaveData CurrentSave { get; set; }
    
    // Runtime state
    public int ExtractionCount { get; set; }
    public float TotalPlayTime { get; set; }
    public Dictionary<string, int> Resources { get; }
    
    // Progression
    public List<string> UnlockedItems { get; }
    public List<string> CompletedQuests { get; }
}
```

## Primary Services (Layer 1B)

### AudioService

```csharp
public interface IAudioService
{
    // Music
    void PlayMusic(AudioStream music, float fadeTime = 0);
    void StopMusic(float fadeTime = 0);
    void SetMusicVolume(float volume);
    
    // SFX
    void PlaySFX(AudioStream sfx, Vector2? position = null);
    void PlaySFXOneShot(AudioStream sfx);
    void SetSFXVolume(float volume);
    
    // Ambient
    void PlayAmbient(AudioStream ambient);
    void StopAmbient();
    
    // Settings
    void SetMasterVolume(float volume);
    void MuteAll();
    void UnmuteAll();
}
```

### SaveService

```csharp
public interface ISaveService
{
    // Save operations
    void SaveGame(string slotName);
    void QuickSave();
    void AutoSave();
    
    // Load operations
    SaveData LoadGame(string slotName);
    SaveData LoadQuickSave();
    SaveData[] GetAllSaves();
    
    // Management
    void DeleteSave(string slotName);
    bool SaveExists(string slotName);
    
    // Settings (separate from game saves)
    void SaveSettings(SettingsData settings);
    SettingsData LoadSettings();
}
```

### InputService

```csharp
public interface IInputService
{
    // Input state
    Vector2 GetMovementVector();
    bool IsActionPressed(string action);
    bool IsActionJustPressed(string action);
    bool IsActionJustReleased(string action);
    
    // Input mode
    InputMode CurrentInputMode { get; }
    void SetInputEnabled(bool enabled);
    
    // Rebinding
    void RemapAction(string action, InputEvent newInput);
    void ResetToDefaults();
}

public enum InputMode
{
    Keyboard,
    Gamepad,
    Touch
}
```

### SceneService

```csharp
public interface ISceneService
{
    // Scene transitions
    void LoadScene(string scenePath, TransitionType transition = TransitionType.Fade);
    void LoadSceneAsync(string scenePath, Action<float> onProgress, Action onComplete);
    void ReloadCurrentScene();
    
    // Scene management
    string CurrentScenePath { get; }
    void AddToScene(Node node, Node parent = null);
    void RemoveFromScene(Node node);
}

public enum TransitionType
{
    None,
    Fade,
    Wipe,
    Dissolve
}
```

### HeroService

```csharp
public interface IHeroService
{
    // Current hero
    HeroData CurrentHero { get; }
    void LoadHero(HeroData hero);
    void UnloadHero();
    
    // Hero state
    uint CurrentHealth { get; set; }
    int CurrentLevel { get; set; }
    int CurrentXP { get; set; }
    
    // Loadout
    void EquipItem(ItemData item, EquipSlot slot);
    void UnequipItem(EquipSlot slot);
    ItemData GetEquippedItem(EquipSlot slot);
}
```

### LevelService

```csharp
public interface ILevelService
{
    // Current level
    LevelData CurrentLevel { get; }
    void LoadLevel(LevelData level);
    void UnloadLevel();
    
    // Level state
    float ElapsedTime { get; }
    int CurrentWave { get; }
    int DifficultyScale { get; }
    
    // Extraction
    Vector2[] GetExtractionPoints();
    bool IsExtractionAvailable { get; }
}
```

## Managers (Layer 4)

### GameManager

Controls the extraction/gameplay state.

```csharp
public partial class GameManager : Node2D
{
    public static GameManager Instance { get; private set; }
    
    // Systems (initialized on _Ready)
    public PhysicsSystem PhysicsSystem { get; private set; }
    public CombatSystem CombatSystem { get; private set; }
    public AISystem AISystem { get; private set; }
    public SpawnSystem SpawnSystem { get; private set; }
    public LootSystem LootSystem { get; private set; }
    public XPSystem XPSystem { get; private set; }
    public FactorySystem FactorySystem { get; private set; }
    public MapSystem MapSystem { get; private set; }
    
    // Utility managers
    public ClockUtility ClockUtility { get; private set; }
    public PlayerUtility PlayerUtility { get; private set; }
    
    // State
    public bool IsLevelLoaded { get; private set; }
    public LevelEntity CurrentLevel { get; private set; }
    
    // Methods
    void Initialize();
    void LoadLevel();
    void UnloadLevel();
    void TogglePause();
    void TriggerExtraction(bool success);
}
```

**Initialization Sequence:**
1. Resolve services from ServiceCore
2. Subscribe to relevant events
3. Wait for level load command
4. Instantiate level entity from template
5. Initialize all game systems
6. Start Heart timers
7. Publish InitEvent

### MenuManager

Controls the downtime/menu state.

```csharp
public partial class MenuManager : Node2D
{
    public static MenuManager Instance { get; private set; }
    
    // Systems
    public UISystem UISystem { get; private set; }
    public CraftSystem CraftSystem { get; private set; }
    public InventorySystem InventorySystem { get; private set; }
    public DialogueSystem DialogueSystem { get; private set; }
    public QuestSystem QuestSystem { get; private set; }
    public IdleSystem IdleSystem { get; private set; }
    
    // Fantasy OS
    public DesktopManager Desktop { get; private set; }
    public WindowManager Windows { get; private set; }
    
    // Methods
    void Initialize();
    void OpenWindow(WindowType type);
    void CloseWindow(string windowId);
    void ShowNotification(string message);
}
```

### UiManager

Handles persistent UI across all states.

```csharp
public partial class UiManager : Node2D
{
    // HUD elements
    public HealthBar HealthBar { get; private set; }
    public XPBar XPBar { get; private set; }
    public MiniMap MiniMap { get; private set; }
    public Timer Timer { get; private set; }
    
    // Overlays
    public PauseMenu PauseMenu { get; private set; }
    public SettingsMenu SettingsMenu { get; private set; }
    public LoadingScreen LoadingScreen { get; private set; }
    
    // Methods
    void ShowHUD();
    void HideHUD();
    void ShowOverlay(OverlayType type);
    void HideOverlay();
    void UpdateHealth(uint current, uint max);
    void UpdateXP(int current, int required);
    void UpdateTimer(float time);
}
```

## Game Systems (Layer 5)

### PhysicsSystem

Handles all entity movement.

```csharp
public partial class PhysicsSystem : Node2D
{
    private List<PhysicsComponent> _components;
    
    public override void _PhysicsProcess(double delta)
    {
        foreach (var component in _components)
        {
            if (!component.IsActive) continue;
            
            var velocity = component.Direction * component.Speed;
            component.Body.Velocity = velocity;
            component.Body.MoveAndSlide();
        }
    }
    
    void RegisterComponent(PhysicsComponent component);
    void UnregisterComponent(PhysicsComponent component);
}
```

### AISystem

Controls enemy behavior.

```csharp
public partial class AISystem : Node2D
{
    private List<AIComponent> _components;
    private HeroEntity _player;
    private float _updateAccumulator;
    private const float UPDATE_INTERVAL = 0.1f; // 10 Hz
    
    public override void _Process(double delta)
    {
        _updateAccumulator += (float)delta;
        if (_updateAccumulator < UPDATE_INTERVAL) return;
        _updateAccumulator = 0;
        
        foreach (var ai in _components)
        {
            UpdateAI(ai);
        }
    }
    
    private void UpdateAI(AIComponent ai)
    {
        // State machine logic
        switch (ai.CurrentState)
        {
            case AIState.Idle:
                CheckForTargets(ai);
                break;
            case AIState.Chasing:
                ChaseTarget(ai);
                break;
            case AIState.Attacking:
                Attack(ai);
                break;
            case AIState.Fleeing:
                Flee(ai);
                break;
        }
    }
}

public enum AIState
{
    Idle,
    Wandering,
    Chasing,
    Attacking,
    Fleeing,
    Dead
}
```

### CombatSystem

Manages combat interactions.

```csharp
public partial class CombatSystem : Node2D
{
    void ProcessAttack(IEntity attacker, IEntity target, uint damage);
    void ApplyDamage(IEntity target, uint damage);
    void ApplyHealing(IEntity target, uint amount);
    void ApplyEffect(IEntity target, EffectData effect);
    void RemoveEffect(IEntity target, string effectId);
    
    // Chant (auto-attack) management
    void ActivateChant(ChantData chant);
    void DeactivateChant(string chantId);
    void UpdateChants(float delta);
}
```

### SpawnSystem

Controls entity spawning with escalation.

```csharp
public partial class SpawnSystem : Node2D
{
    private MobTable _currentTable;
    private int _waveNumber;
    private float _difficultyMultiplier;
    
    void SpawnWave();
    void SpawnMob(MobData data, Vector2 position);
    void SpawnBoss(MobData data, Vector2 position);
    void SetSpawnTable(MobTable table);
    void IncreaseDifficulty();
    
    // Spawning configuration
    int MaxActiveEnemies { get; set; }
    float SpawnRadius { get; set; }
    float SpawnBuffer { get; set; } // Min distance from player
}
```

### LootSystem

Manages loot drops and collection.

```csharp
public partial class LootSystem : Node2D
{
    // Auto-collection
    float CollectionRadius { get; set; }
    float CollectionSpeed { get; set; }
    
    void SpawnLoot(Vector2 position, LootTable table);
    void SpawnItem(Vector2 position, ItemData item);
    void ProcessCollection(HeroEntity player);
    
    // Slot machine mechanic
    void StartSlotRoll(ChestEntity chest);
    void OnSlotComplete(ItemData[] results);
}
```

### XPSystem

Experience and leveling.

```csharp
public partial class XPSystem : Node2D
{
    private List<XPEntity> _activeXP;
    
    void SpawnXP(Vector2 position, int amount);
    void CollectXP(HeroEntity player, XPEntity xp);
    void ProcessXPMagnet(HeroEntity player);
    
    int GetXPForLevel(int level);
    void CheckLevelUp();
    
    // Voices (soul collection for extraction)
    int TotalVoices { get; }
    void CollectVoice(int value);
}
```

### FactorySystem

Entity instantiation with object pooling.

```csharp
public partial class FactorySystem : Node2D
{
    private Dictionary<string, Queue<IEntity>> _pools;
    
    T CreateEntity<T>(PackedScene template, IData data) where T : IEntity;
    void ReturnToPool(IEntity entity);
    void PrewarmPool(PackedScene template, int count);
    void ClearPool(string poolName);
    void ClearAllPools();
}
```

### MapSystem

Map generation and boundaries.

```csharp
public partial class MapSystem : Node2D
{
    public Rect2 MapBounds { get; private set; }
    public Vector2 PlayerSpawn { get; private set; }
    public Vector2[] ExtractionPoints { get; private set; }
    
    void GenerateMap(LevelData levelData, int seed);
    void LoadMapTemplate(PackedScene template);
    bool IsPositionInBounds(Vector2 position);
    bool HasPlayerCrossedBorder(HeroEntity player);
    Vector2 GetRandomSpawnPosition();
    Vector2 GetNearestExtractionPoint(Vector2 from);
}
```

### CraftSystem

Crafting and workbench management.

```csharp
public partial class CraftSystem : Node2D
{
    bool CanCraft(CraftData recipe);
    void Craft(CraftData recipe);
    List<CraftData> GetAvailableRecipes();
    List<CraftData> GetRecipesForWorkbench(WorkbenchType type);
    
    // Workbench management
    void UnlockWorkbench(WorkbenchType type);
    bool IsWorkbenchUnlocked(WorkbenchType type);
}

public enum WorkbenchType
{
    Basic,
    Forge,
    Alchemy,
    Enchanting,
    Engineering
}
```

### DialogueSystem

NPC dialogue and narrative.

```csharp
public partial class DialogueSystem : Node2D
{
    void StartDialogue(DialogueData dialogue);
    void AdvanceDialogue();
    void SelectOption(int optionIndex);
    void EndDialogue();
    
    DialogueState CurrentState { get; }
    string CurrentSpeaker { get; }
    string CurrentText { get; }
    string[] CurrentOptions { get; }
}
```

### QuestSystem

Quest tracking and objectives.

```csharp
public partial class QuestSystem : Node2D
{
    void AcceptQuest(QuestData quest);
    void UpdateObjective(string questId, string objectiveId, int progress);
    void CompleteQuest(string questId);
    void AbandonQuest(string questId);
    
    List<QuestData> ActiveQuests { get; }
    List<QuestData> CompletedQuests { get; }
    List<QuestData> AvailableQuests { get; }
}
```

### IdleSystem

Background/idle progression.

```csharp
public partial class IdleSystem : Node2D
{
    void StartIdleProcess(IdleProcessType type);
    void StopIdleProcess(string processId);
    void CollectIdleRewards();
    
    float GetIdleProgress(string processId);
    Dictionary<string, int> PendingRewards { get; }
    TimeSpan OfflineTime { get; }
}

public enum IdleProcessType
{
    ResourceGathering,
    Gardening,
    Training,
    Research
}
```

## System Initialization Order

### GameManager Boot Sequence
```
1. ServiceCore.BuildServiceContainer()
2. EventCore.BuildTimers()
3. StateCore.ChangeState(Loading)
4. GameManager.Initialize()
   a. Resolve services
   b. Subscribe to events
5. GameManager.LoadLevel()
   a. Load level template
   b. Initialize systems:
      - MapSystem
      - PhysicsSystem
      - AISystem
      - CombatSystem
      - SpawnSystem
      - LootSystem
      - XPSystem
      - FactorySystem
   c. Spawn player
6. Heart.StartTimers()
7. StateCore.ChangeState(Playing)
```

### MenuManager Boot Sequence
```
1. MenuManager.Initialize()
   a. Resolve services
   b. Subscribe to events
2. Initialize systems:
   - UISystem
   - CraftSystem
   - InventorySystem
   - DialogueSystem
   - QuestSystem
   - IdleSystem
3. Initialize Fantasy OS
   - DesktopManager
   - WindowManager
4. CollectIdleRewards() // If returning from extraction
5. StateCore.ChangeState(Downtime)
```

---

*See [Gameplay](gameplay.md) for how these systems create the player-facing game experience.*
