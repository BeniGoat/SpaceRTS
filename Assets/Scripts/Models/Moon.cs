using SpaceRTS.Factories;
using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents a moon in the star system. Holds its body reference and configuration.
	/// Body creation is handled by <see cref="SystemBodyFactory"/>.
	/// </summary>
	public class Moon : MonoBehaviour
    {
		public SystemBody Body { get; private set; }

		private SystemBodyFactory bodyFactory;

		private void Awake()
        {
            this.bodyFactory = this.GetComponent<SystemBodyFactory>();
        }

		/// <summary>
		/// Initialises the moon body with the given orbital parameters.
		/// </summary>
		/// <param name="index">The index of the moon.</param>
		/// <param name="orbitalDistance">The orbital distance of the moon from its parent body.</param>
		/// <param name="size">The size of the moon.</param>
		public void Initialise(int index, float orbitalDistance, float size)
        {
            this.name = $"Moon_{index}";
            this.Body = this.bodyFactory.SpawnChildBody(orbitalDistance, size);
        }
    }
}
