using SpaceRTS.Models;
using System;
using System.Collections.Generic;
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
		[SerializeField] private float minOrbitalSeparation = 2f;
		[SerializeField] private float maxOrbitalSeparation = 5f;
		[SerializeField] private int minPlanets = 3;
		[SerializeField] private int maxPlanets = 8;
		[SerializeField] private int minPlanetSize = 1;
		[SerializeField] private int maxPlanetSize = 5;

		/// <summary>
		/// Spawns a complete star system with a sun and a random number of planets, each with random sizes and orbital distances.
		/// </summary>
		/// <returns>The orbital distance of the farthest planet in the system.</returns>
		public float Initialise()
		{
			// Spawn the sun
			GameObject sun = Instantiate(this.sunPrefab);
			sun.name = "Sun_1";
			float minOrbitalDistance = sun.transform.localScale.x;

			// Calculate orbital distances
			int numPlanets = Random.Range(this.minPlanets, this.maxPlanets + 1);
			List<float> orbitalDistances = this.CalculateOrbitalDistances(numPlanets, minOrbitalDistance);

			// Spawn planets
			for (int i = 0; i < numPlanets; i++)
			{
				// Determine a random size for the planet within the specified range
				float planetSize = Random.Range(this.minPlanetSize, this.maxPlanetSize + 1) * 0.1f;
				Planet planet = Instantiate(this.planetPrefab);
				planet.Initialise(i + 1, orbitalDistances[i], planetSize);
			}

			return orbitalDistances[numPlanets - 1];
		}

		/// <summary>
		/// Calculates the orbital distances for a given number of planets, ensuring that each planet
		/// is spaced apart by a random separation distance within the specified range.
		/// </summary>
		/// <param name="count">The number of planets for which to calculate orbital distances.</param>
		/// <param name="minDistance">The minimum distance from the sun for the first planet.</param>
		/// <returns>A list of orbital distances for each planet.</returns>
		private List<float> CalculateOrbitalDistances(int count, float minDistance)
		{
			var distances = new List<float>(count);

			// Calculate orbital distances for each planet, ensuring they are spaced apart by a random separation distance
			for (int i = 0; i < count; i++)
			{
				float separation = Random.Range(this.minOrbitalSeparation, this.maxOrbitalSeparation);

				if (i == 0)
					distances.Add(minDistance + (separation * 0.5f));
				else
					distances.Add(distances[i - 1] + separation);
			}

			return distances;
		}
	}
}
