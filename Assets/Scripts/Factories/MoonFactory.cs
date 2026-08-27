using SpaceRTS.Models;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceRTS.Factories
{
	/// <summary>
	/// Factory responsible for spawning moons around a parent celestial body.
	/// It generates a random number of moons, each with a random orbital distance and size,
	/// based on the specified configuration parameters.
	/// Attach this component to the <see cref="Planet"/> prefab alongside <see cref="SystemBodyFactory"/>.
	/// </summary>
	public class MoonFactory : MonoBehaviour
	{
		[SerializeField] private Moon moonPrefab;

		[Min(0)]
		[SerializeField] private int minMoons = 0;

		[Min(0)]
		[SerializeField] private int maxMoons = 3;

		[Min(0.01f)]
		[SerializeField] private float minOrbitalClearance = 0.05f;

		[Min(0.01f)]
		[SerializeField] private float maxOrbitalClearance = 0.15f;

		[Min(0.01f)]
		[SerializeField] private float minMoonDiameter = 0.01f;

		[Min(0.01f)]
		[SerializeField] private float maxMoonDiameter = 0.03f;

		private readonly List<Moon> moons = new List<Moon>();

		private void OnValidate()
		{
			if(this.moonPrefab == null)
				Debug.LogError("Moon prefab is not assigned in the MoonFactory.", this);

			this.maxMoons = Mathf.Max(this.maxMoons, this.minMoons);
			this.maxOrbitalClearance = Mathf.Max(this.maxOrbitalClearance, this.minOrbitalClearance);
			this.maxMoonDiameter = Mathf.Max(this.maxMoonDiameter, this.minMoonDiameter);
		}

		/// <summary>
		/// Spawns a random number of moons for the given parent body.
		/// Moon spacing accounts for the radii of adjacent bodies.
		/// </summary>
		/// <param name="parentBody">The parent body around which the moons will orbit.</param>
		public void SpawnMoons(SystemBody parentBody)
		{
			// Determine a random number of moons to spawn for the given parent body
			int numMoons = Random.Range(this.minMoons, this.maxMoons + 1);

			// Initialize the previous orbital distance and moon radius for calculating the next moon's orbit
			float previousOrbitalDistance = parentBody.LocalRadius;
			float previousMoonRadius = 0f;

			// Spawn each moon with a random orbital distance and size
			for (int i = 0; i < numMoons; i++)
			{
				// Determine a random size for the moon within the specified range
				float moonDiameter = Random.Range(this.minMoonDiameter, this.maxMoonDiameter);
				float moonRadius = moonDiameter * 0.5f;

				// Calculate a random clearance distance between the previous moon and the current moon
				float clearance = Random.Range(this.minOrbitalClearance, this.maxOrbitalClearance);
				float orbitalDistance = previousOrbitalDistance
									  + previousMoonRadius
									  + moonRadius
									  + clearance;

				// Instantiate the moon prefab and set its properties
				Moon moon = Instantiate(this.moonPrefab, this.transform);
				moon.transform.position = parentBody.transform.position;
				moon.Initialise(i + 1, orbitalDistance, moonDiameter);

				this.moons.Add(moon);

				// Update the previous orbital distance and moon radius for the next iteration
				previousOrbitalDistance = orbitalDistance;
				previousMoonRadius = moonRadius;
			}
		}
	}
}
