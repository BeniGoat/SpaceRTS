using UnityEngine;

namespace SpaceRTS.Models.Interfaces
{
	/// <summary>
	/// Computes local-space orbital placement for a slot index.
	/// </summary>
	public interface IOrbitalLayoutStrategy
	{
		/// <summary>
		/// Calculates the local position and local rotation for the specified slot.
		/// </summary>
		/// <param name="slotIndex">The zero-based orbital slot index.</param>
		/// <param name="slotCount">The total number of orbital slots.</param>
		/// <param name="orbitalRadius">The orbital radius in world units.</param>
		/// <returns>The local-space rotation and position tuple.</returns>
		(Quaternion localRotation, Vector3 localPosition) CalculateLocalPose(int slotIndex, int slotCount, float orbitalRadius);
	}
}
