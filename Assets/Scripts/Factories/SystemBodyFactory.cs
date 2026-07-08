using SpaceRTS.Models;
using SpaceRTS.Models.Components;
using System;
using UnityEngine;

namespace SpaceRTS.Factories
{
	/// <summary>
	/// Factory responsible for spawning system bodies (planets and moons) in the star system.
	/// </summary>
	public class SystemBodyFactory : MonoBehaviour
    {
		[SerializeField] private SystemBody childBodyPrefab;

		/// <summary>
		/// Spawns a child body (planet or moon) at the specified orbital distance and with the specified diameter.
		/// </summary>
		/// <param name="orbitalDistance">The orbital distance of the child body from its parent.</param>
		/// <param name="bodyDiameter">The diameter of the child body.</param>
		/// <returns>The spawned child body.</returns>
		public SystemBody SpawnChildBody(float orbitalDistance, float bodyDiameter)
        {
			// Calculate a random position in the orbit for the child body
			int positionInOrbit = UnityEngine.Random.Range(0, 359);
            float angle = positionInOrbit * Mathf.Deg2Rad;
            float x = orbitalDistance * Mathf.Cos(angle);
            float z = orbitalDistance * Mathf.Sin(angle);

			// Instantiate the child body prefab and set its properties
			var childBody = Instantiate(this.childBodyPrefab, this.transform);
            childBody.transform.localPosition = new Vector3(x, 0, z);
            childBody.SetBodySize(bodyDiameter);
            childBody.OrbitalDistance = orbitalDistance;

            // Set the child body rotation speed
            Rotator bodyRotator = childBody.GetComponent<Rotator>();
            bodyRotator.SetOrbitalSpeed(30f, Vector3.up);

            // Get the orbital speed based on the orbital distance
            float orbitalSpeed = this.CalculateOrbitalSpeed(orbitalDistance);
            Rotator orbitRotator = this.GetComponent<Rotator>();
            orbitRotator.SetOrbitalSpeed(orbitalSpeed, Vector3.up);

            // Create the orbit line
            OrbitLine orbitLine = this.GetComponent<OrbitLine>();
            float lineWidth = bodyDiameter * 0.1f;
            orbitLine.CreateOrbitalPathLine(orbitalDistance, positionInOrbit, lineWidth);

            return childBody;
        }

        /// <summary>
        /// Gets the orbital speed based on the specified orbital distance.
        /// </summary>
        /// <param name="orbitalDistance">The orbital distance.</param>
        /// <returns>The orbital speed.</returns>
        private float CalculateOrbitalSpeed(float orbitalDistance)
        {
            double orbitalPeriod = Math.Sqrt(Math.Pow(orbitalDistance, 3));
            double orbitalLength = 2 * Math.PI * orbitalDistance;
            return (float)(orbitalLength / orbitalPeriod);
        }
    }
}
