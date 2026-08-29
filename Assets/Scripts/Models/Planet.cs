using SpaceRTS.Factories;
using UnityEngine;

namespace SpaceRTS.Models
{
    /// <summary>
    /// Represents a planet in the star system. Holds its body reference and configuration.
    /// Actual system body spawning is handled by the SystemBodyFactory, moon spawning is handled by the MoonFactory,
    /// and ship spawning is handled by the ShipFactory.
    /// </summary>
    [RequireComponent(typeof(SystemBodyFactory))]
    [RequireComponent(typeof(MoonFactory))]
    [RequireComponent(typeof(ShipFactory))]
    public class Planet : MonoBehaviour
    {
		public SystemBody Body { get; private set; }

		private SystemBodyFactory bodyFactory;
		private MoonFactory moonFactory;

        private void Awake()
        {
			this.bodyFactory = this.GetComponent<SystemBodyFactory>();
            this.moonFactory = this.GetComponent<MoonFactory>();
        }

        /// <summary>
        /// Spawns a planet body with the specified index, orbital distance, size, and angular velocity.
        /// The planet's name is set based on the index, and its body is created using the SystemBodyFactory.
        /// After spawning the planet body, moons are spawned around it using the MoonFactory.
        /// </summary>
        /// <param name="index">The index of the planet.</param>
        /// <param name="orbitalDistance">The orbital distance of the planet from its parent body.</param>
        /// <param name="size">The size of the planet.</param>
        /// <param name="angularVelocity">The angular velocity of the planet in degrees per second.</param>
        public void Initialise(int index, float orbitalDistance, float size, float angularVelocity)
        {
			this.name = $"Planet_{index}";
			this.Body = this.bodyFactory.SpawnSystemBody(orbitalDistance, size, angularVelocity);
			this.moonFactory.SpawnMoons(this.Body);
		}
	}
}
