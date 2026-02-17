# Crowd Simulation – Unity 3D

This project was developed as part of a Computer Animation course and represents a crowd simulation system implemented in Unity.

The system contains:
- Character locomotion with animation blending
- Grid-based pathfinding (A* and bidirectional A*)
- Steering behaviors
- Collision avoidance
- Density-aware navigation heuristics
- Runtime algorithm switching

## Project Overview

The objective was to build a small-scale crowd simulation engine capable of:

- Handling multiple animated agents
- Navigating complex environments
- Avoiding collisions
- Recomputing paths dynamically
- Combining pathfinding and steering approaches

### 1. Locomotion System

#### Features
- Imported rigged 3D character with locomotion animations
- Blend Tree for animation blending
- Velocity-based motion tracking
- Automatic orientation alignment
- Manual orientation locking (F key toggle)

#### Core Scripts
- `Tracker` – Computes velocity and displacement
- `LocomotionController` – Updates Animator parameters
- `OrientationManager` – Smooth character rotation
- `PlayerMovement` – Input-driven testing

### 2. Multi-Agent Collision & Goal Navigation
#### Features
- Randomized non-overlapping agent spawning
- Centralized `Simulator` update loop
- Random goal assignment
- Goal reassignment upon arrival
- Unity-based physics collision handling

#### Core Scripts
- `CrowdGenerator`
- `Agent`
- `Simulator`

### 3. Grid-Based Pathfinding

#### Features
- Procedural grid generation with obstacles
- Graph construction using custom nodes and connections
- A* pathfinding implementation
- Bidirectional A* variant
- Runtime switching between algorithms
- Density-aware heuristic extension

#### Pathfinding Enhancements
- Euclidean heuristic
- Additional penalty for high-density cells
- Waypoint-based agent movement

#### Core Scripts
- `Grid`
- `GridCell`
- `CellConnection`
- `A_Star`
- `Bidirectional_A_Star`
- `GridHeuristic`

### 4. Steering Behaviors & Crowd Navigation
#### Implemented Behaviors
- Arrive behavior for smooth goal approach
- Look-ahead obstacle avoidance
- Velocity-based inter-agent avoidance
- Dynamic blending of steering forces

Agents can switch between:

- Pure grid-based pathfinding
- Steering-based movement
- Hybrid mode
