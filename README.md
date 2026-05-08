<div id="top">

<div align="center">

# 🗡️ Action RPG AI & Level Design
*Game AI Assignment 3*

<img src="https://img.shields.io/github/last-commit/7450N/Game-AI-Assignment-3?style=flat&logo=git&logoColor=white&color=0080ff" alt="last-commit">
<img src="https://img.shields.io/badge/Language-C%23-0080ff?style=flat&logo=csharp&logoColor=white" alt="Language">
<img src="https://img.shields.io/badge/Engine-Unity_3D-FFFFFF.svg?style=flat&logo=Unity&logoColor=black" alt="Unity">

</div>
<br>

---

## ✨ Overview

This project focuses on advanced game AI, player character mechanics, and level design. The core objective was to build upon environmental assets and develop a robust, interactable world featuring intelligent NPC adversaries and a fully state-driven player controller.

The project showcases two distinct approaches to Artificial Intelligence—**Finite State Machines (FSM)** and **Behavior Trees (Unity Behavior Graph)**—allowing different enemy types to navigate the environment using Unity NavMesh and engage the player dynamically.

---

## 🎥 Gameplay Demo

Watch the AI navigation, player state transitions, and combat mechanics in action:

[![Game AI Assignment 3 Demo](https://img.youtube.com/vi/QM9u4jzjnIA/maxresdefault.jpg)](https://youtu.be/QM9u4jzjnIA)

---

## 🎮 Player Controls & Mechanics

The player character is entirely driven by a custom State Machine, ensuring fluid and responsive transitions between movement, combat, and stances. 

### Core Controls
* **Movement:** `W, A, S, D` to navigate the environment.
* **Jump:** `Spacebar` — Triggers the airborne physics and transitions the character into the `JumpingState`.
* **Crouch:** `C` or `Left Ctrl` — Lowers the player's hitbox and transitions into the `DuckingState`, allowing navigation through low-clearance areas.

### Combat Controls
* **Change Weapon / Magic:** `Mouse Wheel` — Swaps the active equipment between melee (Sword) and ranged magic (Fireball). This dynamically changes the player's attack state capabilities (`MeleeState` vs `MagicState`).
* **Attack:** `Left Mouse Button` — Executes an attack based on the currently drawn weapon. If the sword is drawn, it triggers the `SwingSwordState` utilizing melee hitboxes. If magic is equipped, it launches a projectile via the `Fireball` script.

---

## ⚔️ Interacting with the AI NPCs

The environment is populated by two distinct types of NPC adversaries, each governed by a completely different AI architecture. The player can interact with them through proximity, line-of-sight, and combat.

### 1. The Creature (Finite State Machine)
This NPC utilizes a traditional code-based Finite State Machine.
* **Interaction:** If the player stays out of sight, the Creature remains in its `WanderState` or `EatState`, peacefully patrolling its designated waypoints. 
* **Detection:** Once the player steps into its detection radius, the FSM instantly shifts to the `ChaseState`, using Unity NavMesh to pursue the player.
* **Combat:** Upon getting close enough, it enters the `AttackState`. The player must use their equipped weapons (Sword or Fireball) to damage its `CreatureHitBox` and deplete its health.

### 2. The Monster (Behavior Tree)
This NPC utilizes the modern **Unity Behavior Graph** for advanced, node-based decision making.
* **Interaction:** The Monster reads the environment dynamically. Instead of rigid states, it evaluates a "Blackboard" of variables (like player distance and health).
* **Combat:** When the player engages the Monster, the Behavior Tree evaluates combat conditions and executes custom action nodes (like `AttackAction.cs`). The player must actively block or dodge its attacks while countering with their own state-driven melee combos or magic.

---

## 🧠 System Architecture & State Explanations

All custom gameplay and AI logic was engineered from scratch and can be found in `Assets/RW/Scripts/`. 

### Player State Machine (`StateMachine.cs` & `State.cs`)
The player uses a hierarchical state pattern to prevent conflicting animations and logic (e.g., preventing the player from swinging a sword while jumping).
* **Grounded States:** `StandingState`, `GroundedState`, `DuckingState` handle basic terrestrial locomotion.
* **Airborne States:** `JumpingState` handles gravity application and landing transitions.
* **Combat States:** `AttackIdleState`, `MeleeState`, `SwingSwordState`, and `MagicState` handle weapon swapping, animation frame synchronization, and hitbox toggling (preventing multi-hit bugs via `Sword.cs`).

### Creature AI (FSM Architecture)
* **`Creature.cs`:** The brain of the FSM.
* **`WanderState.cs`:** Calculates random NavMesh points for patrolling.
* **`ChaseState.cs`:** Calculates optimal paths to intercept the player.
* **`AttackState.cs` / `EatState.cs`:** Contextual actions triggered by proximity and player status.

### Monster AI (Behavior Tree Architecture)
* **`Monster.cs`:** Bridges the C# logic with the visual Unity Behavior Graph, updating blackboard variables based on real-time physics overlaps.
* **`AttackAction.cs`:** A custom Unity Behavior Graph Action node that triggers attack execution, manages combat cooldowns, and interfaces with `MonsterHitBox.cs` to apply damage.

---

## 🚀 Getting Started

Follow these instructions to run the project locally in the Unity Editor.

### Prerequisites
* Unity Editor: Version 2022.3 LTS (or the specific version used for this project).
* Git: To clone the repository.

### Installation & Execution

1. Clone the repository:
   > git clone https://github.com/7450N/Game-AI-Assignment-3.git

2. Open in Unity:
   > Launch the Unity Hub, click **Add project from disk**, and select the cloned Game-AI-Assignment-3 folder.

3. Load the Scene:
   > In the Unity Project window, navigate to `Assets/RW/Scenes/` and double-click the **Main** scene.

4. Play:
   > Hit the **Play** button at the top of the Unity Editor to test the player combat and observe the AI pathfinding!

---
*Developed by Min Thaw Kaung for Game AI Assignment 3.*
</div>
