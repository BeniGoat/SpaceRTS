# Phase 02 — Simulation Consistency and Deterministic Movement

## Objective
Make simulation behavior consistent across frame rates and fully governed by the simulation clock, with ship movement as the first priority.

## Why this phase
Current behavior mixes frame-driven updates (`Time.deltaTime`) with clock-driven speed and pause state. This can cause drift and reduce determinism.

## Scope
- Ship travel
- Arrival and orbital slot transitions
- Pause/speed consistency across systems
- Deterministic simulation stepping and replayability foundations

## Features

### 1) Tick-driven ship movement
- Move ship travel progression from frame-based update loops to simulation tick processing.
- Represent travel as simulation state (origin, destination, progress, speed, ETA).
- Apply transform updates from simulation state (render interpolation allowed).

### 2) Deterministic travel integration
- Ensure travel advancement uses simulation tick duration only.
- Avoid direct dependence on frame delta for authoritative movement.
- Keep visual smoothing separate from authoritative state.

### 3) Clock-authoritative speed + pause behavior
- Guarantee pause stops movement, rotation, and system progression uniformly.
- Guarantee speed multipliers affect all simulation systems consistently.

### 4) Robust arrival and reservation lifecycle
- Harden reservation-to-occupancy transitions for slot commits.
- Define deterministic handling for failures (placement failure, destroyed destination, cancellation).
- Ensure no leaked reservations/occupied slots on ship destruction.

### 5) Deterministic system execution policy
- Execute movement as a registered simulation system with explicit priority.
- Preserve deterministic order: clock -> scheduled events -> tick event -> systems.

### 6) Test expansion for consistency
- Add tests for:
  - Ship movement under pause and all speeds
  - Travel progress invariance across variable frame rates
  - Reservation/arrival edge cases and cleanup
  - Deterministic system ordering including movement

## Deliverables
- New/updated simulation system(s) for ship movement.
- Refactored movement flow with clock-authoritative progression.
- Integration updates to ship/factory/orbital slot lifecycle handling.
- Automated tests covering deterministic travel and lifecycle consistency.

## Acceptance Criteria
- Ship travel outcomes are identical for equal simulation inputs, regardless of frame rate.
- Paused state produces zero authoritative movement progression.
- Speed changes produce expected deterministic progression ratios.
- No reservation or occupancy leaks after cancel/destroy/failure paths.
- Added tests pass and cover critical movement + lifecycle paths.

## Out of Scope (this phase)
- Combat simulation
- AI strategy behavior
- UI polish beyond minimal debugging/validation visibility

## Dependencies
- `SimulationClockManager`
- `SimulationClock`
- `MovementManager` (refactor target)
- `Ship`
- `ShipFactory`
- `OrbitalSlotRegistry`

## Suggested Milestones
1. Introduce tick-driven movement system and wire registration.
2. Migrate ship travel state updates to simulation ticks.
3. Add deterministic arrival lifecycle handling.
4. Add and pass consistency test suite.
5. Verify pause/speed integration end-to-end.

## Risk Notes
- Refactor touches core gameplay flow; regression risk is moderate.
- Visual smoothness may temporarily degrade until interpolation pass is added.