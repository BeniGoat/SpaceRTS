using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Models.Components
{
	/// <summary>
	/// Owns a unit-scale orbit anchor and places ships into local orbital slots
	/// around a source <see cref="SystemBody"/> without inheriting body scale.
	/// </summary>
	public class ShipOrbitController : MonoBehaviour
	{
		private const string OrbitAnchorName = "Orbit";

		private Transform orbitAnchor;
		private SystemBody sourceBody;
		private IOrbitalLayoutStrategy layoutStrategy;

		/// <summary>
		/// Initializes the orbit controller for a source body and optional layout strategy.
		/// </summary>
		/// <param name="body">The source body around which ships will orbit.</param>
		/// <param name="strategy">The optional orbital layout strategy to use. If null, a default strategy is used.</param>
		public void Initialise(SystemBody body, IOrbitalLayoutStrategy strategy = null)
		{
			this.sourceBody = body;
			this.layoutStrategy = strategy ?? new ClockwiseCircularOrbitalLayout();
			this.EnsureOrbitAnchor();
			this.SyncAnchorToSourceBody();
		}

		private void LateUpdate()
		{
			// Keep the orbit anchor in sync with the source body's position and rotation each frame.
			this.SyncAnchorToSourceBody();
		}

		/// <summary>
		/// Places a ship in the specified orbital slot in anchor-local space.
		/// </summary>
		/// <param name="ship">The ship to place in the orbital slot.</param>
		/// <param name="slotIndex">The index of the slot to place the ship in.</param>
		/// <param name="slotCount">The total number of slots available.</param>
		/// <param name="orbitalRadius">The radius of the orbit.</param>
		/// <returns>True if the ship was successfully placed; otherwise, false.</returns>
		public bool TryPlaceShip(Ship ship, int slotIndex, int slotCount, float orbitalRadius)
		{
			if (ship == null || this.sourceBody == null || this.orbitAnchor == null)
				return false;

			// Calculate the local position and rotation for the ship based on the slot index, slot count, and orbital radius.
			(Quaternion localRotation, Vector3 localPosition) =
				this.layoutStrategy.CalculateLocalPose(slotIndex, slotCount, orbitalRadius);

			// Set the ship's parent to the orbit anchor and apply the calculated local position and rotation.
			ship.transform.SetParent(this.orbitAnchor, worldPositionStays: false);
			ship.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			return true;
		}

		/// <summary>
		/// Ensures an orbit anchor transform exists as a child of the current object.
		/// </summary>
		/// <remarks>Reuses an existing child named OrbitAnchorName when available; otherwise creates one, parents it
		/// with worldPositionStays set to false, and sets its local scale to Vector3.one.</remarks>
		private void EnsureOrbitAnchor()
		{
			if (this.orbitAnchor != null)			
				return;

			// Check for an existing child transform named OrbitAnchorName
			Transform existing = this.transform.Find(OrbitAnchorName);
			if (existing != null)
			{
				this.orbitAnchor = existing;
			}
			else
			{
				this.orbitAnchor = new GameObject(OrbitAnchorName).transform;
				this.orbitAnchor.SetParent(this.transform, worldPositionStays: false);
			}

			this.orbitAnchor.localScale = Vector3.one;
		}

		/// <summary>
		/// Updates the orbit anchor's local position and local rotation to match the source body's transform when both
		/// references are available.
		/// </summary>
		private void SyncAnchorToSourceBody()
		{
			if (this.sourceBody == null || this.orbitAnchor == null)			
				return;
			
			this.orbitAnchor.SetLocalPositionAndRotation(
				this.sourceBody.transform.localPosition,
				this.sourceBody.transform.localRotation);
		}
	}
}
