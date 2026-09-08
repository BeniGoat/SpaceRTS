# Physical Value Model

## Purpose
Define a single, coherent physical unit system for simulation and transfer calculations, including celestial bodies and ships. This model makes **body mass** and **physical orbital radii** directly compatible.

## Canonical Unit System
Use SI as canonical storage and simulation units:

- Length: **meter (m)**
- Mass: **kilogram (kg)**
- Time: **second (s)**
- Derived:
  - Density: **kg/m^3**
  - Velocity: **m/s**
  - Acceleration: **m/s^2**

## Authoritative vs. Derived Properties

| Property | Canonical Unit | Authority | Notes / Derivation |
|---|---:|---|---|
| Body diameter | m | Derived (if radius is authoritative) | `diameter = 2 * radius` |
| Body radius | m | Authoritative (preferred) | Primary body size value for physics and orbits |
| Body mass | kg | Authoritative | Primary gravitational mass |
| Density | kg/m^3 | Derived by default | `density = mass / volume`; may be authoritative for procedural generation |
| Orbital distance | m | Authoritative | Physical orbital radius / semi-major axis used by transfer math |
| Orbital period | s | Derived by default | Computed from orbit parameters and gravitational parameter; may be authoritative for authored systems |
| Axial rotation period | s | Authoritative | Positive duration for one full rotation |
| Ship mass | kg | Authoritative | `dryMass + fuelMass` if split values exist |
| Fuel | kg | Authoritative (mass basis) | Store fuel as mass, not volume or abstract units |
| Velocity | m/s | Derived runtime state | Simulation state; may be initialized from authored inputs |
| Acceleration | m/s^2 | Derived runtime state | From forces / thrust and mass |
| Rendered position | engine units | Derived | Converted from world-space meters at render boundary |
| Rendered scale | engine units | Derived | Converted from physical size with view-dependent scale strategy |

## Conversion Boundaries

1. **Simulation/physics boundary (internal):**
   - All calculations use canonical SI units (`m`, `kg`, `s`).
   - No mixed units inside orbital or transfer calculations.

2. **Content input boundary (authoring/import):**
   - External values (e.g., km, tons, days) must be converted to SI on ingest.
   - Preserve source metadata only for UI/display, never for physics computation.

3. **Rendering boundary:**
   - Convert world meters to render-space units immediately before rendering.
   - Keep render-space scaling isolated from simulation state.

4. **UI/output boundary:**
   - Convert SI to player-facing units (km, Mm, hours, days, etc.) only for display.

## Numeric Precision Policy

- Simulation canonical values: **double** precision.
- Long-lived orbital and transfer parameters (distance, mass, period): **double** required.
- Rendering-facing values: may downcast to **float** only at rendering boundary.
- Constants and conversion factors: define once, reuse globally (no ad hoc literals).
- Avoid repeated round-trip conversions between SI and display units.

## Validation Constraints

### Global
- All canonical values must be finite (`!NaN`, `!Infinity`).
- Units must be explicit at API boundaries.

### Per-property constraints
- Body radius: `> 0`
- Body diameter: `> 0` and `diameter == 2 * radius` within tolerance
- Body mass: `> 0`
- Density: `> 0` when authoritative or computed
- Orbital distance: `> 0`
- Orbital period: `> 0`
- Axial rotation period: `> 0`
- Ship mass: `> 0`
- Fuel mass: `>= 0` and `fuelMass <= shipMass` when ship mass is total mass
- Velocity/acceleration: finite vectors
- Rendered position/scale: finite values after conversion

### Recommended tolerances
- Equality checks for derived values should use relative tolerance (e.g., `1e-9` in simulation space for doubles), not exact float equality.

## Implementation Rule
Body mass (`kg`) and orbital distance/radius (`m`) are mandatory canonical inputs for transfer calculations. Any system that cannot provide these units must convert before entering simulation code.

## Concrete Type Boundaries

### Static configuration types (immutable)

- `SystemBodyId`
  - Stable Unity-independent identifier.
  - Non-empty, non-whitespace string semantics.
- `SystemBodyPhysicalProperties`
  - Authoritative SI values:
	- `RadiusMeters` (`> 0`, finite)
	- `MassKilograms` (`> 0`, finite)
	- `AxialRotationPeriodSeconds` (`> 0`, finite)
	- `IsTidallyLocked` (explicit lock semantics)
  - Derived values:
	- `DiameterMeters = 2 * RadiusMeters`
	- `VolumeCubicMeters = 4/3 * π * RadiusMeters^3`
	- `DensityKilogramsPerCubicMeter = MassKilograms / VolumeCubicMeters`
- `OrbitProperties` (circular-orbit scope only)
  - `ParentBodyId` (required valid identifier)
  - `OrbitalRadiusMeters` (`> 0`, finite)
  - `OrbitalPeriodSeconds` (`> 0`, finite)
  - `InitialPhaseRadians` (finite, normalized to `[0, 2π)`)
  - Inclination, eccentricity, and full orbital elements are deferred.
- `ShipPhysicalProperties`
  - `DryMassKilograms` (`> 0`, finite)
  - `FuelCapacityKilograms` (`>= 0`, finite)

### Runtime state types (mutable)

- `OrbitState`
  - Holds current orbital phase in radians.
  - Phase is kept normalized to `[0, 2π)` after construction and updates.
- `ShipFuelState`
  - Holds `CurrentFuelMassKilograms` bounded to `[0, FuelCapacityKilograms]`.
  - Derives total ship mass as:
	- `TotalMassKilograms = DryMassKilograms + CurrentFuelMassKilograms`

### Ownership convention

- Root bodies are represented by the absence of an orbit instance.
- Any `OrbitProperties` instance represents an orbiting child and therefore requires a valid `ParentBodyId`.

## Angle Convention

- All orbital phase values are in **radians**.
- Normalization target range is **`[0, 2π)`**.

## Tidal Lock Semantics

- Tidal locking is represented explicitly by `IsTidallyLocked`.
- Independent axial period remains a positive finite value; infinity is not used as a sentinel.
- Invariants that compare locked axial period to orbital period are enforced only where body and orbit data are composed.

## Exception Policy

- Invalid numeric values (`NaN`, `+∞`, `-∞`, or invalid ranges) throw `ArgumentOutOfRangeException`.
- Invalid identifiers and cross-property relationship violations throw `ArgumentException`.

## Deferred Integration Scope

- Unity-facing generation and movement code (`SystemBody`, factories, transforms, render scaling, travel logic) remains unchanged in this phase.
- Integration of these pure types into Unity factories and render conversion boundaries is intentionally deferred.
