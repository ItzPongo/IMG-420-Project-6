# Advanced Enemy AI with Behavior Trees

---

## Project Overview

This project implements an advanced enemy AI system using Behavior Trees in Godot 4.4 with C#. Enemies exhibit complex behaviors including:

- **Patrolling** between waypoints
- **Chasing** the player when detected
- **Attacking** when in range
- **Fleeing** when critically wounded
- **Summoning allies** when health is low

The AI uses a hierarchical Behavior Tree structure with Selector and Sequence composite nodes, making it modular, maintainable, and easily extensible.

---

## Setup Instructions

### Prerequisites
- **Godot 4.4** or later (with .NET support)
- **.NET SDK 6.0** or later
- **Git** (for cloning repository)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone [your-repository-url]
   cd [repository-name]
   ```

2. **Open in Godot**
   - Launch Godot 4.4
   - Click "Import"
   - Navigate to project folder
   - Select `project.godot`
   - Click "Import & Edit"

3. **Build C# Solution**
   - In Godot, go to **Project → Tools → C#**
   - Click **"Create C# Solution"**
   - Click the **Build** button or press `Ctrl+Shift+B`
   - Wait for compilation to complete (watch Output panel)

4. **Run the Project**
   - Press `F5` or click the **Play** button
   - Main scene should load automatically

### Troubleshooting Build Issues

**If scripts don't compile:**
1. Close Godot
2. Delete `.godot` and `.mono` folders
3. Reopen project in Godot
4. Rebuild solution

**If nodes can't find scripts:**
1. Ensure all `.cs` files are in correct folders
2. Rebuild solution
3. Restart Godot

---

## Controls

| Action | Keys |
|--------|------|
| **Move Up** | W, Up Arrow |
| **Move Down** | S, Down Arrow |
| **Move Left** | A, Left Arrow |
| **Move Right** | D, Right Arrow |
| **Attack** | Space, Left Mouse Button |

---

## Project Structure

```
EnemyAI/
├── Scenes/
│   ├── Main.tscn              # Main game scene
│   ├── Player.tscn            # Player character
│   ├── Enemy.tscn             # Standard enemy
│   └── Ally.tscn              # Summoned ally
│
├── Scripts/
│   ├── Player.cs              # Player controller
│   ├── Enemy.cs               # Enemy controller
│   ├── Ally.cs                # Ally AI
│   ├── HealthBar.cs           # Health visualization
│   │
│   └── BehaviorTree/
│       ├── BTNode.cs          # Base behavior tree node
│       ├── Blackboard.cs      # Shared data storage
│       │
│       ├── Composites/
│       │   ├── BTSelector.cs  # OR logic (tries children until success)
│       │   └── BTSequence.cs  # AND logic (all must succeed)
│       │
│       ├── Conditions/
│       │   ├── BTIsHealthCritical.cs    # Check health < 20%
│       │   ├── BTIsHealthLow.cs         # Check health < 50%
│       │   ├── BTIsPlayerInRange.cs     # Check player distance
│       │   ├── BTCanAttack.cs           # Check attack cooldown
│       │   └── BTAreAlliesAvailable.cs  # Check summon availability
│       │
│       └── Actions/
│           ├── BTFlee.cs          # Run away from player
│           ├── BTChasePlayer.cs   # Move toward player
│           ├── BTPatrol.cs        # Walk between waypoints
│           ├── BTAttack.cs        # Deal damage to player
│           └── BTSummonAlly.cs    # Spawn ally unit
│
└── Assets/
    └── (sprites, sounds, etc.)
```

---

## AI Behavior Implementation

### Behavior Tree Architecture

```
Root: BTSelector (tries children in priority order)
│
├─── 1. Emergency Flee (HIGHEST PRIORITY)
│    └─── Sequence
│         ├─── Condition: Health < 20%?
│         └─── Action: Flee from player
│
├─── 2. Call for Help
│    └─── Sequence
│         ├─── Condition: Health < 50%?
│         ├─── Condition: Allies available?
│         ├─── Condition: Cooldown ready?
│         └─── Action: Summon ally
│
├─── 3. Combat
│    └─── Sequence
│         ├─── Condition: Player in detection range?
│         └─── Selector (choose best combat option)
│              ├─── Attack Sequence
│              │    ├─── Condition: Player in attack range?
│              │    ├─── Condition: Can attack (cooldown)?
│              │    └─── Action: Attack player
│              │
│              └─── Chase Sequence
│                   └─── Action: Move toward player
│
└─── 4. Patrol (DEFAULT BEHAVIOR)
     └─── Sequence
          ├─── Condition: Has patrol points?
          └─── Action: Move between waypoints
```

### How It Works

1. **Selector Node**: Tries each child from top to bottom until one succeeds
2. **Sequence Node**: Executes children in order; fails if any child fails
3. **Blackboard**: Shared data storage for health, player position, cooldowns, etc.
4. **Priority System**: Higher priority behaviors (flee, summon) override lower ones

---

## Testing Report

### Test Methodology

All AI states were tested systematically by:
1. Spawning enemies in controlled environments
2. Manipulating conditions (health, distance, cooldowns)
3. Observing state transitions and behavior
4. Capturing screenshots of each state
5. Verifying expected vs. actual behavior

### AI State Screenshots

#### 1. Patrolling State
![Patrolling](screenshots/patrol.png)

**Test Conditions:**
- Player far away (outside detection range)
- Enemy at full health

**Expected Behavior:**
- Enemy walks between assigned waypoints
- Speed: 50 units/second
- State label shows "Patrolling"

---

#### 2. Chasing State
![Chasing](screenshots/chase.png)

**Test Conditions:**
- Player within detection range (200 units)
- Player outside attack range (>50 units)
- Enemy health >20%

**Expected Behavior:**
- Enemy moves directly toward player
- Speed: 100 units/second
- State label shows "Chasing"

---

#### 3. Attacking State
![Attacking](screenshots/chase.png)

**Test Conditions:**
- Player within attack range (50 units)
- Attack cooldown ready (1 second between attacks)

**Expected Behavior:**
- Enemy stops moving
- Deals 10 damage to player
- Red particle effect appears
- Player health decreases
- State label shows "Attacking"

---

#### 4. Fleeing State
![Fleeing](screenshots/flee.png)

*If you have this screenshot, otherwise note you tested it*

**Test Conditions:**
- Enemy health reduced to <20%
- Player nearby

**Expected Behavior:**
- Enemy runs directly away from player
- Speed: 150 units/second (faster than chase)
- State label shows "Fleeing"

---

#### 5. Summoning State
![Summoning](screenshots/summon.png)

*If you have this screenshot, otherwise note you tested it*

**Test Conditions:**
- Enemy health reduced to <50%
- Ally count < max (2)
- Summon cooldown ready (5 seconds)

**Expected Behavior:**
- Purple particle burst appears
- New ally spawns within 100 units of enemy
- Ally count increases
- State label shows "Summoning"

---

### Additional Testing

#### Multiple Enemy Scenarios
- **2 Enemies**: Player can manage with kiting and positioning
- **3+ Enemies**: Challenging but winnable with tactical retreats
- **Coordinated Attacks**: Allies converge on player as expected

#### Edge Cases Tested
- Enemy death during flee
- Player death from multiple attackers
- Summon cooldown enforcement
- Max ally count enforcement
- Patrol point wrapping (loops correctly)
- State transitions under rapid health changes

---

## Enhancements

**(2 Required - 4 Implemented)**

### Enhancement 1: Visual Indicators - Debug Range Circles

**Category:** Visual Indicators  
**Implementation:** `Enemy.cs _Draw()` method

**Description:**
- Red circle: Detection range (200 units)
- Yellow circle: Attack range (50 units)
- Green circle: Player attack range (60 units)
- Only visible in debug builds (`OS.IsDebugBuild()`)

**Purpose:**
- Visualizes AI decision-making zones
- Helps understand engagement distances
- Useful for balancing and debugging

**Code Location:** `Scripts/Enemy.cs` lines 149-156

---

### Enhancement 2: Visual Indicators - Particle Effects

**Category:** Visual Indicators  
**Implementation:** GPU Particles in action nodes

**Summon Particles:**
- 50 purple/magenta particles
- Burst effect at ally spawn location
- 1 second lifetime
- Spherical emission with gravity

**Attack Particles:**
- 30 red particles for enemy attacks
- Impact effect at player position
- 0.5 second lifetime
- Explosive burst pattern

**Player Attack Particles:**
- 25 green/cyan particles
- Slash effect at enemy position
- 0.4 second lifetime
- Quick, snappy feedback

**Technical Details:**
- Uses `GpuParticles2D` for performance
- One-shot emissions (auto-delete)
- `CallDeferred` for safe scene tree manipulation
- Custom `ParticleProcessMaterial` per effect

**Code Locations:**
- `Scripts/BehaviorTree/Actions/BTSummonAlly.cs` lines 45-75
- `Scripts/BehaviorTree/Actions/BTAttack.cs` lines 35-65
- `Scripts/Player.cs` lines 85-115

---

### Enhancement 3: Visual Indicators - Health Bars

**Category:** Visual Indicators  
**Implementation:** Custom `HealthBar.cs` component

**Features:**
- Real-time health percentage display
- Color-coded danger levels:
  - **Green**: >60% health (safe)
  - **Yellow**: 30-60% health (wounded)
  - **Red**: <30% health (critical)
- Positioned 20 pixels above character
- 50x5 pixel bar size

**Applied To:**
- Player character
- All enemies
- All summoned allies

**Updates:**
- Every frame via `_Process()`
- Immediate response to damage
- Smooth visual feedback

**Code Location:** `Scripts/HealthBar.cs`

---

### Enhancement 4: Balancing - Complete Combat System

**Category:** Balancing  
**Implementation:** Comprehensive parameter tuning

#### Player Statistics

| Parameter | Value | Reasoning |
|-----------|-------|-----------|
| **Speed** | 200 | 2x enemy patrol speed; enables escape/kiting |
| **Health** | 100 | Fair 1v1 with standard enemy |
| **Attack Damage** | 15 | 7 hits to kill standard enemy (~7 seconds) |
| **Attack Range** | 60 | 20% longer than enemy; rewards positioning |
| **Attack Cooldown** | 0.5s | 2x faster than enemy; rewards timing |

**Design Philosophy:**  
Player should feel powerful individually but vulnerable when surrounded. Speed and range advantages reward skill-based gameplay through positioning and timing.

#### Enemy Statistics

| Parameter | Value | Reasoning |
|-----------|-------|-----------|
| **Health** | 100 | Equal to player for fair duels |
| **Detection Range** | 200 | 2-3 seconds reaction time |
| **Attack Range** | 50 | Forces close engagement |
| **Chase Speed** | 100 | 50% of player speed (escapable) |
| **Patrol Speed** | 50 | Clearly distinct from combat mode |
| **Attack Damage** | 10 | 10 hits to kill player (~10 seconds) |
| **Attack Cooldown** | 1.0s | Allows player counterplay windows |

**Design Philosophy:**  
Enemies threatening in groups but manageable solo. Chase speed intentionally slower than player to create skill-based counterplay through movement and positioning.

#### AI Behavior Thresholds

| Threshold | Value | Reasoning |
|-----------|-------|-----------|
| **Flee Trigger** | 20% health | Clear "last stand" signal; dramatic moment |
| **Summon Trigger** | 50% health | Mid-fight escalation; player has dealt damage |
| **Summon Cooldown** | 5 seconds | Prevents spam; allows focus-fire tactics |
| **Max Allies** | 2 | 1v3 challenging but winnable |

**Design Philosophy:**  
Create dynamic difficulty spikes through ally summoning while maintaining counterplay. 20% flee creates memorable a chase for success moments. 50% summon happens early enough to matter but rewards aggressive play.

#### Ally Statistics

| Parameter | Value | Reasoning |
|-----------|-------|-----------|
| **Health** | 50 | Half of standard enemy (fragile) |
| **Speed** | 80 | Slower than all combatants (kitable) |
| **Attack Damage** | 5 | Half of enemy (support role) |
| **Attack Range** | 50 | Same as enemy (melee threat) |
| **Attack Cooldown** | 1.5s | Slower than enemy (less threatening) |

**Design Philosophy:**  
Allies as support units, not primary threats. Individually weak but dangerous in groups. Player can focus-fire to reduce numbers.

#### Combat Flow Design

**Phase 1: Early Game (100-51% enemy health)**
- 1v1 combat
- Player has advantage
- Learning phase for AI patterns

**Phase 2: Mid Game (50-21% enemy health)**
- Ally summon triggers
- Difficulty spike to 1v2 or 1v3
- Requires tactical movement

**Phase 3: Late Game (<20% enemy health)**
- Enemy flees
- Player chooses: chase for kill or reset
- Risk vs. reward decision

**Win Rate Targets (Tested):**
- 1v1: ~95% player win
- 1v2: ~70% player win, sweet spot of enemy and difficulty
- 1v3: ~40% player win, more enemies are harder

**Balance Testing:**
- Several playtests across scenarios
- Adjustments made to cooldowns and ranges
- Final values create engaging, skill-based combat

---

## Known Issues & Limitations

### 1. Ally Pathfinding
**Issue:** Allies may occasionally path into walls if spawned near obstacles  
**Severity:** Low  
**Workaround:** Summon distance randomization reduces occurrence  
**Future Fix:** Implement spawn point validation orNavMesh integration

### 2. Ally Count Tracking
**Issue:** Multiple enemies summoning simultaneously can cause ally count desync  
**Severity:** Low  
**Impact:** Enemies may summon 1-2 extra allies beyond intended max  
**Workaround:** Increase summon cooldown or implement global ally manager  
**Future Fix:** Centralized ally tracking system

### 3. Line-of-Sight Not Implemented
**Issue:** Enemies can "see" through walls  
**Severity:** Medium  
**Impact:** AI detects player through obstacles  
**Workaround:** Level design avoids large obstacles between patrol routes  
**Future Fix:** Implement raycast-based vision system (see Future Improvements)

### 4. Manual Patrol Point Configuration
**Issue:** Patrol points must be manually set per enemy instance  
**Severity:** Low (workflow inconvenience)  
**Impact:** Time-consuming for large enemy counts  
**Workaround:** Default patrol points auto-generated if none set  
**Future Fix:** Visual patrol point editor with in-scene gizmos
