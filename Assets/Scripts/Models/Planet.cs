using SpaceRTS.Factories;
using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents a planet in the star system. Holds its body reference and configuration.
	/// Moon spawning is handled externally by the <see cref="SystemBodyFactory"/> attached to this GameObject.
    /// </summary>
	public class Planet : MonoBehaviour
    {
		public SystemBody Body { get; private set; }

		private SystemBodyFactory bodyFactory;
		private MoonFactory moonFactory;

		private void Awake()
        {
			// Get references to the SystemBodyFactory and MoonFactory components attached to this GameObject
			this.bodyFactory = this.GetComponent<SystemBodyFactory>();
            this.moonFactory = this.GetComponent<MoonFactory>();
        }

		/// <summary>
		/// Spawns a planet body with the specified index, orbital distance, and size.
		/// The planet's name is set based on the index, and its body is created using the SystemBodyFactory.
		/// After spawning the planet body, moons are spawned around it using the MoonFactory.
		/// </summary>
		/// <param name="index">The index of the planet.</param>
		/// <param name="orbitalDistance">The orbital distance of the planet from its parent body.</param>
		/// <param name="size">The size of the planet.</param>
		public void Initialise(int index, float orbitalDistance, float size)
        {
			this.name = $"Planet_{index}";
			this.Body = this.bodyFactory.SpawnChildBody(orbitalDistance, size);
			this.moonFactory.SpawnMoons(this.Body);
		}
	}
}
