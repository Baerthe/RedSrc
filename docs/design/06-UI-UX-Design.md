# UI/UX Design Document

## Overview

This document outlines the user interface and user experience design for the RTS game, including HUD elements, menus, controls, and information presentation.

## Visual Design Philosophy

### 8-Bit Aesthetic
- **Color Palette**: Limited color palette (2-4 colors per element)
- **Pixel Art**: 16x16 or 32x32 pixel sprites for UI elements
- **Typography**: Pixel font (existing font package from Kenney.nl)
- **Borders**: Sharp, pixelated borders and frames
- **Animations**: Simple, frame-based animations

### UI Principles
1. **Clarity**: Information should be instantly readable
2. **Consistency**: Similar elements look and behave similarly
3. **Feedback**: Every action provides immediate visual/audio feedback
4. **Accessibility**: Colorblind-friendly, high contrast
5. **Performance**: Minimal impact on game performance

## Screen Layout

### In-Game HUD

```
┌─────────────────────────────────────────────────────────────┐
│ [Resources] [Time] [Supply]              [Menu] [Settings]  │ Top Bar
├─────────────────────────────────────────────────────────────┤
│                                                               │
│                                                               │
│                      [Game Viewport]                          │
│                                                               │
│                                                               │
│                                                     ┌────────┐│
│                                                     │Minimap ││ Right Panel
│                                                     └────────┘│
├──────────────────────────┬──────────────────────────────────┤
│  [Unit/Building Panel]   │   [Command/Build Panel]          │ Bottom Panel
└──────────────────────────┴──────────────────────────────────┘
```

### Layout Dimensions (854x480 resolution)
- **Top Bar**: 854x40px
- **Game Viewport**: 624x360px
- **Right Panel**: 230x360px
- **Bottom Panel**: 854x80px

## HUD Components

### Top Bar

#### Resource Display
```
┌──────────────────────────────┐
│ ⚜ 1,250  🪵 650  👥 45/100   │
│ Gold    Wood    Supply       │
└──────────────────────────────┘
```

**Elements:**
- **Gold Icon + Number**: Current gold amount (updates in real-time)
- **Wood Icon + Number**: Current wood amount
- **Supply**: Current/Max (changes color: Green < 80%, Yellow 80-95%, Red > 95%)
- **Color Coding**: Red when resource low (<100), yellow when medium (<300)

#### Game Timer
```
┌──────────┐
│ ⏱ 12:34  │
└──────────┘
```
- Shows elapsed game time (MM:SS format)
- Used for build order timing and replays

#### Menu Buttons
```
┌────────────────────────┐
│ [Menu] [Objectives]    │
└────────────────────────┘
```
- **Menu**: Pause game, access settings, save/load, quit
- **Objectives**: Show current mission objectives (campaign mode)

### Right Panel

#### Minimap
```
┌──────────────────┐
│ [==============] │ 200x200px
│ [==============] │
│ [==============] │
│ [==============] │
└──────────────────┘
```

**Features:**
- Represents entire map at scale
- **Colors:**
  - Green: Friendly units/buildings
  - Red: Enemy units/buildings
  - Yellow: Neutral/resources
  - Gray: Terrain/fog of war
  - Black: Unexplored
- **Interactions:**
  - Left Click: Move camera to location
  - Right Click: Issue move command
  - Drag: Draw box, units move there
- **Alerts**: Ping animation when units under attack

#### Alert Feed
```
┌──────────────────┐
│ ⚠ Base Attack!   │
│ ✓ Unit Complete  │
│ ! Low Resources  │
└──────────────────┘
```
- Shows last 3 alerts
- Click alert to jump to location
- Auto-dismiss after 10 seconds

### Bottom Panel

#### Unit/Building Info Panel (Left Side)
```
┌─────────────────────────────────┐
│  [Unit Portrait]  Footman       │
│                   HP: 45/60     │
│                   Armor: 2      │
│                   Damage: 12    │
│                                 │
│  [Status Icons]  [Abilities]    │
└─────────────────────────────────┘
```

**Single Selection:**
- Portrait (32x32px sprite)
- Unit name
- Health bar (colored: Green > 66%, Yellow 33-66%, Red < 33%)
- Key stats (Armor, Damage, Range if ranged)
- Status effects (buffs/debuffs as icons)
- Abilities (with cooldown overlays)

**Multiple Selection:**
```
┌─────────────────────────────────┐
│ [icon] [icon] [icon]  Footman x3│
│ [icon] [icon]         Archer x2 │
│                       5 Selected │
└─────────────────────────────────┘
```
- Up to 12 unit icons
- Group by unit type
- Shows count per type
- Click icon to select that unit type

**Building Selection:**
```
┌─────────────────────────────────┐
│  [Portrait]    Barracks         │
│                HP: 800/800      │
│                                 │
│  [Rally Point] [Production]     │
│  Set: ➤        Queue: 2/5       │
│                                 │
│  [===Progress Bar===] 45%       │
└─────────────────────────────────┘
```
- Building portrait and name
- Health bar
- Rally point indicator
- Production queue (shows units being built)
- Progress bar for current production
- Cancel button (refund 75%)

#### Command/Build Panel (Right Side)

**Unit Commands:**
```
┌──────────────────────────────────┐
│ [Move] [Stop] [Hold] [Patrol]    │
│ [Atk]  [Spec1] [Spec2] [...]     │
└──────────────────────────────────┘
```

**Grid Layout:** 4x2 = 8 buttons
- **Standard Commands**: Move, Attack, Stop, Hold Position, Patrol
- **Unit Abilities**: Displayed when available
- **Hotkeys**: Shown on button (A for Attack, S for Stop, etc.)

**Build Menu (Workers Selected):**
```
┌──────────────────────────────────┐
│ Basic     Advanced    Military   │ [Tabs]
├──────────────────────────────────┤
│ [Farm]   [House]   [Lumber Mill] │
│  60g      100g       120g        │
│  20w      50w        80w         │
│                                  │
│ [Barracks] [Tower]  [...]        │
│  150g      100g                  │
│  50w       50w                   │
└──────────────────────────────────┘
```

**Tabs:**
- Basic: Economic buildings (Farms, Houses)
- Advanced: Tech buildings (Blacksmith, Church)
- Military: Barracks, Ranges, Workshops

**Building Production (Barracks Selected):**
```
┌──────────────────────────────────┐
│ [Footman] [Archer]  [Knight]     │
│   100g     120g      250g        │
│   0w       20w       50w         │
│   20s      25s       45s         │
│                                  │
│ [Upgrade1] [Upgrade2]            │
└──────────────────────────────────┘
```

### Selection Indicators

#### Unit Selection
```
┌──────┐
│ Unit │  ← Green circle outline
└──────┘
   ↓
[Health Bar] 
```
- Green circle at unit feet (selected)
- Health bar above unit (always visible when selected)
- Control group number badge (if assigned)

#### Building Selection
```
┌─────────┐
│Building │  ← Green box outline
└─────────┘
   ↓
[Health Bar]
```
- Green box outline around building
- Health bar above building
- Construction progress bar (if building)

#### Selection Box (Drag Selection)
```
┌ - - - - - - - ┐
│               │  ← Green dashed box
│   [Units]     │
└ - - - - - - - ┘
```

### Control Groups Display

```
[1] [2] [3] [4] [5] [6] [7] [8] [9] [0]
 ✓   ✓           ✓                    
```
- Located above minimap or bottom-left corner
- Shows which groups are assigned (✓ or number)
- Click to select, double-click to jump to
- Glows when group takes damage

## Menu Systems

### Main Menu

```
╔════════════════════════════════╗
║                                ║
║        [GAME LOGO]             ║
║                                ║
║       [Single Player]          ║
║       [Multiplayer]            ║
║       [Settings]               ║
║       [Credits]                ║
║       [Quit]                   ║
║                                ║
║   Version 0.1.0                ║
╚════════════════════════════════╝
```

### Pause Menu (In-Game)

```
╔════════════════════════════════╗
║          PAUSED                ║
║                                ║
║       [Resume]                 ║
║       [Settings]               ║
║       [Save Game]              ║
║       [Load Game]              ║
║       [Objectives]             ║
║       [Main Menu]              ║
║       [Quit]                   ║
╚════════════════════════════════╝
```

### Settings Menu

```
╔════════════════════════════════╗
║          SETTINGS              ║
║                                ║
║  [Graphics] [Audio] [Controls] ║ [Tabs]
║                                ║
║  Resolution: [854x480] [▼]     ║
║  Fullscreen: [X] Yes [ ] No    ║
║  Vsync:      [ ] Yes [X] No    ║
║  FPS Limit:  [60]              ║
║                                ║
║       [Apply]  [Cancel]        ║
╚════════════════════════════════╝
```

**Tabs:**
- **Graphics**: Resolution, fullscreen, effects
- **Audio**: Master, music, SFX volume sliders
- **Controls**: Keybind customization, mouse sensitivity

### Mission Select (Campaign)

```
╔════════════════════════════════╗
║      CAMPAIGN MISSIONS         ║
║                                ║
║  [Mission 1: Training] ✓       ║
║  [Mission 2: First Battle] ✓   ║
║  [Mission 3: Expansion] ✓      ║
║  [Mission 4: Defense] [LOCKED] ║
║  [Mission 5: Final Push] [LOCK]║
║                                ║
║         [Back]                 ║
╚════════════════════════════════╝
```

### Skirmish Setup

```
╔════════════════════════════════╗
║        SKIRMISH SETUP          ║
║                                ║
║  Map: [Test Map] [▼]           ║
║                                ║
║  Player 1 (You): [Human] [▼]   ║
║  Player 2 (AI):  [Orc]   [▼]   ║
║  Difficulty:     [Medium] [▼]  ║
║                                ║
║  Starting Res:   [Standard] [▼]║
║                                ║
║     [Start Game]  [Back]       ║
╚════════════════════════════════╝
```

## In-Game Notifications

### Alert Types

#### Combat Alert
```
⚔ Your base is under attack!
```
- Red flashing border
- Sound: Alert horn
- Minimap ping at location
- Auto-camera jump option

#### Completion Alert
```
✓ Footman training complete
```
- Green checkmark
- Sound: "Unit ready"
- Bottom-right notification

#### Warning Alert
```
⚠ Not enough resources!
```
- Yellow warning icon
- Sound: Error beep
- Temporary message

#### Research Alert
```
🔬 Weapon Upgrade I complete
```
- Blue icon
- Sound: Success chime
- Global buff applied

### Status Messages

**On-Screen Messages:**
```
┌──────────────────────┐
│  Victory!            │
│  Enemy defeated      │
└──────────────────────┘
```
- Centered, large text
- Background overlay (semi-transparent)
- Auto-dismiss or button to continue

### Tooltips

**Hover Tooltips:**
```
┌─────────────────────────┐
│ Footman                 │
│ Cost: 100 gold          │
│ Time: 20 seconds        │
│ HP: 60  Armor: 2        │
│ Damage: 12 (Normal)     │
│                         │
│ Basic infantry unit     │
│ Hotkey: F               │
└─────────────────────────┘
```

- Appear after 0.5s hover
- Show comprehensive info
- Consistent format across UI
- Include hotkey information

## Visual Feedback

### Unit States

#### Movement
- Dotted line shows path
- Destination marked with green flag icon
- Formation indicators for groups

#### Attack
- Red line from unit to target (brief flash)
- Attack animation plays
- Target flashes on hit

#### Under Attack
- Unit flashes red briefly
- Health bar displays prominently
- Alert on minimap

#### Low Health
- Health bar turns red
- Unit portrait border flashes red
- Unit blinks/flashes

### Building States

#### Construction
```
[=====-----] 50%
```
- Progress bar above building
- Semi-transparent sprite
- Scaffold/construction visual effect

#### Damaged
- Smoke particle effects (< 50% health)
- Fire effects (< 25% health)
- Visual cracks/damage on sprite

#### Producing
- Progress bar for current unit
- Queue indicators (1, 2, 3...) above building
- Light glow or animation when active

### Resource Gathering

#### Worker Carrying
- Visual indicator (tiny gold/wood icon) above worker
- Different animation when carrying
- Line showing return path

#### Resource Depleted
- Gold mine: Grayed out, "Depleted" label
- Tree: Stump remains, eventually regrows

## Accessibility Features

### Colorblind Modes
- **Protanopia**: Red-green colorblind support
- **Deuteranopia**: Another red-green variant
- **Tritanopia**: Blue-yellow colorblind support
- **Options**: Alternative color schemes, pattern overlays

### UI Scaling
- 100% (default for 854x480)
- 125% (for higher resolutions)
- 150% (for accessibility)

### Text Options
- Font size adjustment (Small, Medium, Large)
- High contrast text backgrounds
- Text-to-speech for messages (future)

### Audio Cues
- Distinct sounds for each alert type
- Positional audio for attacks (stereo)
- Audio captions option

## Control Schemes

### Mouse Controls

#### Left Click
- Select unit/building
- UI button activation
- Confirm placement

#### Right Click
- Context-sensitive command (move/attack/gather)
- Cancel building placement

#### Left Click + Drag
- Box selection
- Minimap camera drag

#### Scroll Wheel
- Zoom in/out

#### Middle Mouse + Drag
- Pan camera

### Keyboard Shortcuts

#### Camera Control
- **WASD** / **Arrow Keys**: Pan camera
- **Home**: Jump to main base
- **Spacebar**: Jump to last alert location
- **Backspace**: Jump to selected units

#### Selection
- **Ctrl + A**: Select all army units on screen
- **Ctrl + Click**: Remove from selection
- **Shift + Click**: Add to selection
- **Tab**: Cycle through selected units
- **Double-click unit**: Select all visible units of that type

#### Commands
- **A**: Attack-move
- **S**: Stop
- **H**: Hold position
- **M**: Move (ignore enemies)
- **P**: Patrol
- **G**: Guard target
- **B**: Build menu (workers)

#### Control Groups
- **Ctrl + 1-0**: Assign to group
- **1-0**: Select group
- **Shift + 1-0**: Add to group
- **Ctrl + Shift + 1-0**: Append current to group

#### Building Hotkeys
- **B**: Build menu
- **F**: Farm
- **H**: House/Supply
- **R**: Barracks
- **A**: Archery Range
- **S**: Stable
- **W**: Workshop

#### Unit Production Hotkeys
- **Q, W, E, R, A, S, D, F**: Quick build (matches grid)

#### Game Control
- **Esc**: Pause menu / Cancel action
- **F1**: Help
- **F2-F9**: Quick save slots (future)
- **F11**: Fullscreen toggle
- **F12**: Screenshot

### Gamepad Support (Future)

#### Layout (Xbox Style)
- **Left Stick**: Move cursor
- **Right Stick**: Pan camera
- **A**: Select/Confirm
- **B**: Cancel/Back
- **X**: Context command
- **Y**: Open build menu
- **LB/RB**: Cycle through units
- **LT/RT**: Cycle through buildings
- **D-Pad**: Quick commands (up: attack, down: stop, etc.)
- **Start**: Pause menu
- **Select**: Toggle command panel/build menu

## Performance Considerations

### UI Optimization
- Minimize draw calls (batch UI elements)
- Use texture atlases for UI sprites
- Cache tooltip content
- Lazy update for off-screen UI elements
- Throttle minimap updates (15 FPS sufficient)

### Responsive Design
- UI scales with resolution
- Maintain 16:9 aspect ratio recommendations
- Support for 4:3 and 16:10 (adjust viewport)

---

**Document Version**: 1.0  
**Last Updated**: 2026-01-14  
**Status**: Draft for Review
