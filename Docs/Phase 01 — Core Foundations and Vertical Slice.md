# Phase 01 — Core Foundations and Vertical Slice

## Objective

Establish a playable single-system prototype with core world generation, camera/navigation controls, unit selection and command flow, ship orbit/travel behavior, and a deterministic simulation clock baseline.

## Why this phase

This phase captures the existing foundation so later phases have a clear baseline. It also removes roadmap discontinuity before Phase 02.

## Scope

- Procedural single-star-system generation
- Camera setup and zoom mode support
- Input-driven selection and command handling
- Ship spawning, travel, and orbital slot lifecycle model
- Simulation clock integration with speed controls
- Initial test coverage for simulation determinism

## Implemented Features Snapshot

### 1) World generation baseline
- Star system generation with randomized planet counts, diameters, and spacing.
- Configurable min/max ranges and validation.

### 2) Camera and navigation baseline
- Camera manager with movement, lateral rotation, and zoom strategy support.
- Perspective and orthographic zoom strategies.
- Selection-follow camera targeting behavior.

### 3) Selection and command loop baseline
- Click-based selection service and selection events.
- Command input manager flow using raycast targets.
- Ship destination assignment via movement manager orchestration.

### 4) Ship/orbit lifecycle baseline
- Ship spawning through `ShipFactory`.
- Orbit placement via orbital layout/controller collaborators.
- Slot state model: free -> reserved -> occupied with explicit handles.
- Path visualization for active ship travel.

### 5) Simulation baseline
- Authoritative `SimulationClock` with pause/speed model.
- Clock manager bridge and deterministic tick pipeline ordering.
- Speed change propagation events consumed by runtime components.

### 6) Test baseline
- Deterministic simulation clock test suite present.
- Coverage includes pause handling, speed multipliers, queue ordering, and tick pipeline ordering.

## Deliverables (Completed)

- Playable prototype loop (spawn -> select -> command -> move/arrive).
- Service registration and event bus integration across managers.
- Deterministic simulation clock core and related tests.
- Baseline UI surfaces for speed/calendar feedback.

## Known Gaps Carried to Phase 02+

- Frame-based ship movement not yet fully clock-authoritative.
- Determinism not yet guaranteed for all gameplay systems.
- Limited command feedback for invalid/full destinations.
- Single-selection workflow; no fleet controls.
- Lifecycle edge cases (cancel/destroy/failure) need stronger guarantees.

## Acceptance Criteria (Historical)

- Prototype systems integrated and interactive in-scene.
- Simulation clock initializes paused and responds to speed inputs.
- Core simulation tests exist and validate deterministic clock behavior.
- Basic ship destination flow functions with orbital slot constraints.

## Outcome

Phase 01 is considered complete as a baseline foundation.  
Phase 02 is the first hardening phase focused on deterministic simulation consistency, especially ship movement.