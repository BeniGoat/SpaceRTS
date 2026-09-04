# Simulation Time Migration Boundary (Phase 1)

Phase 1 establishes authoritative simulation time only.

Out of scope in this phase (intentionally unchanged):
- `Assets/Scripts/Managers/MovementManager.cs`
- `Assets/Scripts/Models/Ship.cs` (`ProcessTravel`)
- `Assets/Scripts/Models/Components/Rotator.cs`
- Orbital behavior (`ShipOrbitController` and related orbital placement/reservation flow)
- Camera behavior (`CameraManager` and zoom strategies)

Current temporary behavior:
- Those systems still consume Unity frame delta timing and are not yet driven by simulation ticks.
- The new simulation clock remains the sole authority for simulation calendar, pause state, and simulation speed.

Next-phase migration target:
- Move simulation-domain systems to deterministic `SimulationTickEvent` consumption.
