using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents a star system in the game. Holds metadata about the system's bounds.
	/// Spawning is handled by <see cref="SpaceRTS.Factories.SystemFactory"/>.
	/// </summary>
	public class System : MonoBehaviour
    {
		public float Size { get; set; }
	}
}