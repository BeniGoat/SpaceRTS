using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Computes clockwise circular slot placement and tangent-facing orientation.
	/// </summary>
	public sealed class ClockwiseCircularOrbitalLayout : IOrbitalLayoutStrategy
	{
		/// <inheritdoc/>
		public (Quaternion localRotation, Vector3 localPosition) CalculateLocalPose(
			int slotIndex,
			int slotCount,
			float orbitalRadius)
		{
			if (slotCount <= 0)
			{
				throw new global::System.ArgumentOutOfRangeException(nameof(slotCount));
			}

			if (slotIndex < 0 || slotIndex >= slotCount)
			{
				throw new global::System.ArgumentOutOfRangeException(nameof(slotIndex));
			}

			float positionAngle = slotIndex * 360f / slotCount;
			float angle = positionAngle * Mathf.Deg2Rad;

			Vector3 localPosition = new(
				orbitalRadius * Mathf.Cos(angle),
				0f,
				-orbitalRadius * Mathf.Sin(angle));

			Quaternion localRotation = Quaternion.Euler(0f, positionAngle + 180f, 0f);
			return (localRotation, localPosition);
		}
	}
}
