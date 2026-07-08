using SpaceRTS.Models;
using System.Collections.Generic;
using UnityEngine;

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
		[SerializeField] private int minMoons = 0;
		[SerializeField] private int maxMoons = 3;
		[SerializeField] private int minOrbitalSeparation = 1;
		[SerializeField] private int maxOrbitalSeparation = 3;
		[SerializeField] private int minMoonSize = 1;
		[SerializeField] private int maxMoonSize = 3;

		private readonly List<Moon> moons = new List<Moon>();

		/// <summary>
		/// Spawns a random number of moons for the given parent body, each with a random orbital distance and size.
		/// </summary>
		/// <param name="parentBody">The parent body around which the moons will orbit.</param>
		public void SpawnMoons(SystemBody parentBody)
		{
			// Determine a random number of moons to spawn for the given parent body
			int numMoons = Random.Range(this.minMoons, this.maxMoons + 1);

			// Spawn each moon with a random orbital distance and size
			for (int i = 0; i < numMoons; i++)
			{
				Moon moon = Instantiate(this.moonPrefab, this.transform);
				moon.transform.position = parentBody.transform.position;

				// Calculate a random separation distance for the moon's orbit
				float separation = Random.Range(this.minOrbitalSeparation, this.maxOrbitalSeparation + 1) * 0.01f;
				float moonOrbit = i == 0
					? parentBody.MaxRadius + separation
					: this.moons[i - 1].Body.OrbitalDistance + separation;

				float moonSize = Random.Range(this.minMoonSize, this.maxMoonSize + 1) * 0.02f;
				moon.Initialise(i + 1, moonOrbit, moonSize);
				this.moons.Add(moon);
			}
		}
	}
}
