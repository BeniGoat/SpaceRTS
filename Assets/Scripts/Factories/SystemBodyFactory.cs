using SpaceRTS.Models;
using SpaceRTS.Models.Components;
using System;
using UnityEngine;

namespace SpaceRTS.Factories
{
    /// <summary>
    /// Factory responsible for spawning system bodies (planets and moons) in the star system.
    /// </summary>
    [RequireComponent(typeof(OrbitLine))]
    [RequireComponent(typeof(Rotator))]
	public class SystemBodyFactory : MonoBehaviour
    {
        private Rotator orbitRotator;

		[SerializeField] private SystemBody childBodyPrefab;

        /// <summary>
        /// Represents a band of orbital speed multipliers based on distance thresholds.
        /// </summary>
        [Serializable]
        private struct OrbitalSpeedBand
        {
            // Example Inspector configuration:
            // Minimum Distance: 0.5 - Speed Multiplier: 0.05
            // Minimum Distance: 2.0 - Speed Multiplier: 0.15
            // Minimum Distance: 5.0 - Speed Multiplier: 0.5
            // Minimum Distance: 15.0 - Speed Multiplier: 1.0

            [Min(0f)]
            [SerializeField] private float minimumDistance;

            [Min(0f)]
            [SerializeField] private float speedMultiplier;

            public float MinimumDistance => this.minimumDistance;
            public float SpeedMultiplier => this.speedMultiplier;
        }

        // Orbital speed multiplier for the default case when no bands are matched.
        [Header("Orbital Speed")]
        [Min(0f)]
        [SerializeField] private float orbitalSpeedMultiplier = 0.02f;

        // Array of orbital speed bands for stepped distance-based multipliers.
        [SerializeField]
        private OrbitalSpeedBand[] orbitalSpeedBands = Array.Empty<OrbitalSpeedBand>();

        private void Awake()
		{
			this.orbitRotator = this.GetComponent<Rotator>();
		}

		/// <summary>
		/// Spawns a system body (planet or moon) at the specified orbital distance and with the specified diameter.
		/// </summary>
		/// <param name="orbitalDistance">The orbital distance of the system body from its parent.</param>
		/// <param name="bodyDiameter">The diameter of the system body.</param>
		/// <returns>The spawned system body.</returns>
		public SystemBody SpawnSystemBody(float orbitalDistance, float bodyDiameter)
        {
            // Calculate a random position in the orbit for the system body
            int positionInOrbit = UnityEngine.Random.Range(0, 360);
            float angle = positionInOrbit * Mathf.Deg2Rad;
            float x = orbitalDistance * Mathf.Cos(angle);
            float z = orbitalDistance * Mathf.Sin(angle);

            // Instantiate the system body prefab and set its properties
            SystemBody systemBody = Instantiate(this.childBodyPrefab, this.transform);
            systemBody.transform.localPosition = new Vector3(x, 0, z);
            systemBody.SetBodySize(bodyDiameter);

            // Get the orbital speed based on the orbital distance
            float orbitalSpeed = this.CalculateOrbitalSpeed(orbitalDistance);
            this.orbitRotator.SetRotationSpeed(orbitalSpeed, Vector3.up);

            // Create the orbit line
            OrbitLine orbitLine = this.GetComponent<OrbitLine>();
            float lineWidth = bodyDiameter * 0.1f;
            orbitLine.CreateOrbitalPathLine(orbitalDistance, positionInOrbit, lineWidth);

            return systemBody;
        }

        /// <summary>
        /// Calculates orbital speed using Kepler's third law and a stepped
        /// distance-based multiplier.
        /// </summary>
        /// <param name="orbitalDistance">The orbital distance of the body.</param>
        /// <returns>The orbital speed in degrees per second.</returns>	
        private float CalculateOrbitalSpeed(float orbitalDistance)
        {
            float safeDistance = Mathf.Max(orbitalDistance, 0.001f);
            float orbitalPeriod = Mathf.Pow(safeDistance, 1.5f);
            float multiplier = this.GetOrbitalSpeedMultiplier(safeDistance);

            return (360f / orbitalPeriod) * multiplier;
        }

        /// <summary>
        /// Gets the multiplier belonging to the highest distance threshold
        /// reached by the orbital distance.
        /// </summary>
        /// <param name="orbitalDistance">The orbital distance of the body.</param>
        /// <returns>The orbital speed multiplier.</returns>
        private float GetOrbitalSpeedMultiplier(float orbitalDistance)
        {
            float selectedMultiplier = this.orbitalSpeedMultiplier;
            float selectedThreshold = 0f;

            foreach (OrbitalSpeedBand band in this.orbitalSpeedBands)
            {
                if (orbitalDistance >= band.MinimumDistance &&
                    band.MinimumDistance >= selectedThreshold)
                {
                    selectedThreshold = band.MinimumDistance;
                    selectedMultiplier = band.SpeedMultiplier;
                }
            }

            return selectedMultiplier;
        }
    }
}
