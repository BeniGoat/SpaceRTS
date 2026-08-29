using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Factories
{
	/// <summary>
	/// Orchestrates the creation of an entire star system including the sun, planets, and moons.
	/// </summary>
	public class SystemFactory : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField] private GameObject sunPrefab;
		[SerializeField] private Planet planetPrefab;

		[Header("System Generation Settings")]
		[Min(1f)]
		[SerializeField] private float minOrbitalClearance = 2f;

		[Min(1f)]
		[SerializeField] private float maxOrbitalClearance = 5f;

		[Min(1)]
		[SerializeField] private int minPlanets = 3;

		[Min(1)]
		[SerializeField] private int maxPlanets = 8;

		[Min(0.1f)]
		[SerializeField] private float minPlanetDiameter = 0.2f;

		[Min(0.1f)]
		[SerializeField] private float maxPlanetDiameter = 1f;

		private void OnValidate()
		{
			if (this.sunPrefab == null)
				Debug.LogError("Sun prefab is not assigned in the SystemFactory.", this);
			if (this.planetPrefab == null)
				Debug.LogError("Planet prefab is not assigned in the SystemFactory.", this);

			this.maxOrbitalClearance = Mathf.Max(this.maxOrbitalClearance, this.minOrbitalClearance);
			this.maxPlanets = Mathf.Max(this.maxPlanets, this.minPlanets);
			this.maxPlanetDiameter = Mathf.Max(this.maxPlanetDiameter, this.minPlanetDiameter);
		}

		/// <summary>
		/// Spawns a complete star system with randomly sized and spaced planets.
		/// Planet spacing accounts for the radii of adjacent bodies.
		/// </summary>
		/// <returns>The extent of the star system, defined as the distance from the center of the sun to the farthest point of the outermost planet.</returns>
		public float Initialise()
		{
			// Spawn the sun
			GameObject sun = Instantiate(this.sunPrefab);
			sun.name = "Sun_1";

			Vector3 sunScale = sun.transform.localScale;
			float sunRadius = Mathf.Max(sunScale.x, sunScale.y, sunScale.z) * 0.5f;

			int numPlanets = Random.Range(this.minPlanets, this.maxPlanets + 1);

			float previousOrbitalDistance = sunRadius;
			float previousPlanetRadius = 0f;
			float systemExtent = sunRadius;

			// Spawn planets
			for (int i = 0; i < numPlanets; i++)
			{
				// Determine a random size for the planet within the specified range
				float planetDiameter = Random.Range(this.minPlanetDiameter, this.maxPlanetDiameter);
				float planetRadius = planetDiameter * 0.5f;

				// Calculate a random clearance distance between the previous planet and the current planet
				float clearance = Random.Range(this.minOrbitalClearance, this.maxOrbitalClearance);
				float orbitalDistance = previousOrbitalDistance
									  + previousPlanetRadius
									  + planetRadius
									  + clearance;

				// Instantiate the planet prefab and set its properties
				Planet planet = Instantiate(this.planetPrefab);
				planet.Initialise(i + 1, orbitalDistance, planetDiameter);

				// Update the previous orbital distance and planet radius for the next iteration
				previousOrbitalDistance = orbitalDistance;
				previousPlanetRadius = planetRadius;

				// Update the system extent to ensure it encompasses the farthest planet
				systemExtent = Mathf.Max(systemExtent, orbitalDistance + planetRadius);
			}

			return systemExtent;
		}
	}
}
