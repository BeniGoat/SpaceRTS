using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Factories
{
	/// <summary>
	/// Factory responsible for spawning ships in orbit around a source body and managing
	/// orbital slot capacity through a two-phase reserve-then-commit model.
	/// </summary>
	public class ShipFactory : MonoBehaviour
	{
		[SerializeField] private Ship shipPrefab;

		[SerializeField, Min(1)] private int minOrbitalSlots = 4;
		[SerializeField, Min(1)] private int maxOrbitalSlots = 16;
		[SerializeField, Min(0.01f)] private float bodyRadiusPerAdditionalSlot = 0.05f;
		[SerializeField, Min(0.1f)] private float shipPrefabScale = 0.25f;

		private SystemBody sourceBody;
		private Transform orbit;

		/// <summary>Number of ships physically in orbit (committed occupants).</summary>
		private int occupiedSlots;

		/// <summary>Number of en-route ships that have reserved a slot but not yet arrived.</summary>
		private int reservedSlots;

		private int orbitalSlotCount;

		/// <summary>
		/// Total slots committed (occupied + reserved). Used to prevent overbooking.
		/// </summary>
		private int CommittedSlots => this.occupiedSlots + this.reservedSlots;

		private void Start()
		{
			// Initialize the source body reference from the child SystemBody component and
			// calculate the number of orbital slots based on the source body's radius
			this.sourceBody = this.GetComponentInChildren<SystemBody>();
			this.orbitalSlotCount = this.GetOrbitalSlotCount();

			// Create a dedicated "Orbit" GameObject to serve as the parent for ships in orbit around the source body
			// Its rotation is kept in sync with the body in LateUpdate so that ships
			// parented here follow the body's axial spin without inheriting its scale.
			this.orbit = new GameObject("Orbit").transform;
			this.orbit.SetParent(this.transform, worldPositionStays: false);
			this.orbit.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		}

		private void LateUpdate()
		{
			// Mirror the source body's local position and rotation so orbital positions
			// automatically follow both the body's movement and its axial spin.
			if (this.sourceBody != null && this.orbit != null)
			{
				this.orbit.SetLocalPositionAndRotation(
					this.sourceBody.transform.localPosition,
					this.sourceBody.transform.localRotation);
			}
		}

		/// <summary>
		/// Returns the <see cref="ShipFactory"/> associated with a given <see cref="SystemBody"/>,
		/// searching up the hierarchy if needed.
		/// </summary>
		/// <param name="body">The system body to find a factory for.</param>
		/// <returns>The associated <see cref="ShipFactory"/>, or <c>null</c> if none exists.</returns>
		public static ShipFactory GetForBody(SystemBody body)
		{
			if (body == null)
				return null;

			// Walk up the hierarchy to find the nearest ShipFactory
			return body.GetComponentInParent<ShipFactory>(includeInactive: false);
		}

		/// <summary>
		/// Attempts to reserve an orbital slot for a ship that is about to travel to this body.
		/// A reservation prevents overbooking while the ship is in transit.
		/// </summary>
		/// <returns><c>true</c> if a slot was successfully reserved; otherwise <c>false</c>.</returns>
		public bool TryReserveSlot()
		{
			if (this.CommittedSlots >= this.orbitalSlotCount)
				return false;

			this.reservedSlots++;
			return true;
		}

		/// <summary>
		/// Releases a previously reserved slot without completing travel.
		/// Call this when a travel order is replaced before the ship arrives.
		/// </summary>
		public void ReleaseReservation()
		{
			this.reservedSlots = Mathf.Max(0, this.reservedSlots - 1);
		}

		/// <summary>
		/// Notifies this factory that a ship has departed from its orbit, freeing one occupied slot.
		/// </summary>
		public void NotifyDeparture()
		{
			this.occupiedSlots = Mathf.Max(0, this.occupiedSlots - 1);
		}

		/// <summary>
		/// Converts an existing reservation into an occupied slot and places the ship in orbit.
		/// The caller must have previously called <see cref="TryReserveSlot"/> on this factory.
		/// </summary>
		/// <param name="ship">The arriving ship to place in orbit.</param>
		/// <returns>The ship after being placed, or <c>null</c> if placement was unsuccessful.</returns>
		public Ship CommitReservedArrival(Ship ship)
		{
			// Consume the reservation regardless of whether placement succeeds
			this.reservedSlots = Mathf.Max(0, this.reservedSlots - 1);

			if (this.sourceBody == null || ship == null)
				return null;

			// Calculate the local position and rotation for the arriving ship
			(Quaternion localRotation, Vector3 localPosition) = this.CalculateOrbitalLocalTransform();

			// Place the ship in orbit around the source body
			ship.transform.SetParent(this.orbit, worldPositionStays: true);
			ship.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			ship.CurrentSystemBody = this.sourceBody;

			this.occupiedSlots++;
			return ship;
		}

		/// <summary>
		/// Spawns a new ship prefab into orbit around the configured source body.
		/// </summary>
		/// <returns>The newly spawned ship, or <c>null</c> if spawning was unsuccessful.</returns>
		public Ship GenerateShipInOrbit()
		{
			// Check if the source body is valid and if there are available slots
			if (this.sourceBody == null || this.CommittedSlots >= this.orbitalSlotCount)
				return null;

			// Calculate the local position and rotation for the new ship based on the next available orbital slot
			(Quaternion localRotation, Vector3 localPosition) = this.CalculateOrbitalLocalTransform();

			// Instantiate the ship prefab and set its transform relative to the source body
			Ship newShip = Instantiate(this.shipPrefab, this.orbit);
			newShip.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			newShip.transform.localScale = this.shipPrefab.transform.localScale * this.shipPrefabScale;
			newShip.CurrentSystemBody = this.sourceBody;

			this.occupiedSlots++;
			return newShip;
		}

		/// <summary>
		/// Calculates the local position and local rotation for placing an object at the
		/// next available orbital slot around the source body.
		/// </summary>
		/// <returns>A tuple containing the local rotation and local position for the orbital placement.</returns>
		private (Quaternion, Vector3) CalculateOrbitalLocalTransform()
		{
			// Place the ship at the next occupied-slot position
			float positionAngle = this.occupiedSlots * 360f / this.orbitalSlotCount;
			float angle = positionAngle * Mathf.Deg2Rad;
			float orbitalDistance = this.sourceBody.OrbitalRadius;

			// Calculate the local position and rotation based on the orbital distance and angle
			Vector3 localPosition = new(
				orbitalDistance * Mathf.Cos(angle),
				0,
				-orbitalDistance * Mathf.Sin(angle));

			// The tangent direction at angle θ is (-sinθ, 0, cosθ), which equals Euler(0, -θ, 0)
			// applied to Z+, so rotating by positionAngle around Y faces the ship tangentially
			Quaternion localRotation = Quaternion.Euler(0f, positionAngle + 180f, 0f);

			return (localRotation, localPosition);
		}

		/// <summary>
		/// Calculates the number of orbital slots based on the source body's radius.
		/// </summary>
		/// <returns>The calculated orbital slot count, clamped between the configured minimum and maximum values.</returns>
		private int GetOrbitalSlotCount()
		{
			// Calculate the number of additional orbital slots based on the source body's radius
			// and the configured body radius per additional slot
			int additionalSlots = Mathf.FloorToInt(
				this.sourceBody.WorldRadius / this.bodyRadiusPerAdditionalSlot);

			// Clamp the total number of orbital slots between the minimum and maximum values
			return Mathf.Clamp(
				this.minOrbitalSlots + additionalSlots,
				this.minOrbitalSlots,
				this.maxOrbitalSlots);
		}
	}
}