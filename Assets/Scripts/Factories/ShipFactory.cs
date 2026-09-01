using SpaceRTS.Models;
using SpaceRTS.Models.Components;
using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Factories
{
	/// <summary>
	/// Coordinates ship creation and orbital placement for a single source body.
	/// Slot accounting and orbital transform behavior are delegated to focused collaborators.
	/// </summary>
	[RequireComponent(typeof(ShipOrbitController))]
	public class ShipFactory : MonoBehaviour
	{
		[SerializeField] private Ship shipPrefab;

		[SerializeField, Min(1)] private int minOrbitalSlots = 4;
		[SerializeField, Min(1)] private int maxOrbitalSlots = 16;
		[SerializeField, Min(0.01f)] private float bodyRadiusPerAdditionalSlot = 0.05f;
		[SerializeField, Min(0.1f)] private float shipPrefabScale = 0.25f;

		private SystemBody sourceBody;
		private ShipOrbitController orbitController;
		private OrbitalSlotRegistry slotRegistry;
		private IOrbitalLayoutStrategy orbitalLayout;

		/// <summary>
		/// Gets a value indicating whether there are available orbital slots.
		/// </summary>
		public bool HasOrbitalSlots => this.slotRegistry != null && this.slotRegistry.HasAvailableSlots;

		private void Start()
		{
			this.sourceBody = this.GetComponentInChildren<SystemBody>();
			if (this.sourceBody == null)
				return;

			// Initialize the orbital slot registry and layout strategy based on the source body's properties.
			int orbitalSlotCount = this.GetOrbitalSlotCount();
			this.slotRegistry = new OrbitalSlotRegistry(orbitalSlotCount);
			this.orbitalLayout = new ClockwiseCircularOrbitalLayout();

			// Initialize the orbit controller with the source body and layout strategy.
			this.orbitController = this.GetComponent<ShipOrbitController>();
			this.orbitController.Initialise(this.sourceBody, this.orbitalLayout);
		}

		/// <summary>
		/// Returns the <see cref="ShipFactory"/> associated with a given <see cref="SystemBody"/>,
		/// searching up the hierarchy if needed.
		/// </summary>
		/// <param name="body">The system body for which to find the associated ship factory.</param>
		/// <returns>The <see cref="ShipFactory"/> associated with the given system body, or null if none is found.</returns>
		public static ShipFactory GetForBody(SystemBody body)
		{
			if (body == null)			
				return null;			

			return body.GetComponentInParent<ShipFactory>(includeInactive: false);
		}

		/// <summary>
		/// Attempts to reserve an orbital slot for an incoming ship.
		/// </summary>
		/// <param name="reservation">The reserved orbital slot.</param>
		/// <returns>True if a slot was successfully reserved; otherwise, false.</returns>
		public bool TryReserveSlot(out OrbitalSlotReservation reservation)
		{
			if (this.slotRegistry == null)
			{
				reservation = OrbitalSlotReservation.None;
				return false;
			}

			return this.slotRegistry.TryReserve(out reservation);
		}

		/// <summary>
		/// Releases a previously reserved slot.
		/// </summary>
		/// <param name="reservation">The reserved orbital slot to release.</param>
		/// <returns>True if the reservation was successfully released; otherwise, false.</returns>
		public bool ReleaseReservation(OrbitalSlotReservation reservation)
		{
			return this.slotRegistry != null && this.slotRegistry.ReleaseReservation(reservation);
		}

		/// <summary>
		/// Frees a specific occupied slot when a ship departs.
		/// </summary>
		/// <param name="occupiedSlot">The occupied orbital slot to free.</param>
		/// <returns>True if the occupied slot was successfully freed; otherwise, false.</returns>
		public bool NotifyDeparture(OrbitalOccupiedSlot occupiedSlot)
		{
			return this.slotRegistry != null && this.slotRegistry.ReleaseOccupied(occupiedSlot);
		}

		/// <summary>
		/// Commits a reservation and places the arriving ship in the associated slot.
		/// </summary>
		/// <param name="ship">The ship to place in the reserved slot.</param>
		/// <param name="reservation">The reserved orbital slot.</param>
		/// <param name="occupiedSlot">The occupied orbital slot after the ship is placed.</param>
		/// <returns>True if the ship was successfully placed in the reserved slot; otherwise, false.</returns>
		public bool CommitReservedArrival(
			Ship ship,
			OrbitalSlotReservation reservation,
			out OrbitalOccupiedSlot occupiedSlot)
		{
			occupiedSlot = OrbitalOccupiedSlot.None;
			if (ship == null || this.slotRegistry == null || this.sourceBody == null || this.orbitController == null)
				return false;

			// If the reservation is invalid or cannot be committed, return false.
			if (!this.slotRegistry.TryCommitReservation(reservation, out OrbitalOccupiedSlot committedSlot))
				return false;

			// Attempt to place the ship in the committed slot.
			bool isPlaced = this.orbitController.TryPlaceShip(
				ship,
				committedSlot.SlotIndex,
				this.slotRegistry.SlotCount,
				this.sourceBody.OrbitalRadius);

			// If placement fails, release the occupied slot and return false.
			if (!isPlaced)
			{
				this.slotRegistry.ReleaseOccupied(committedSlot);
				return false;
			}

			ship.CurrentSystemBody = this.sourceBody;
			occupiedSlot = committedSlot;
			return true;
		}

		/// <summary>
		/// Spawns a ship and places it in the next available orbit slot.
		/// </summary>
		public Ship GenerateShipInOrbit(out OrbitalOccupiedSlot occupiedSlot)
		{
			occupiedSlot = OrbitalOccupiedSlot.None;
			if (this.slotRegistry == null || this.sourceBody == null || this.orbitController == null)
			{
				return null;
			}

			if (!this.slotRegistry.TryOccupyNext(out OrbitalOccupiedSlot committedSlot))
			{
				return null;
			}

			Ship newShip = Instantiate(this.shipPrefab);
			bool placed = this.orbitController.TryPlaceShip(
				newShip,
				committedSlot.SlotIndex,
				this.slotRegistry.SlotCount,
				this.sourceBody.OrbitalRadius);
			if (!placed)
			{
				Destroy(newShip.gameObject);
				this.slotRegistry.ReleaseOccupied(committedSlot);
				return null;
			}

			newShip.transform.localScale = this.shipPrefab.transform.localScale * this.shipPrefabScale;
			newShip.CurrentSystemBody = this.sourceBody;
			occupiedSlot = committedSlot;
			return newShip;
		}

		/// <summary>
		/// Backward-compatible overload returning only the spawned ship.
		/// </summary>
		public Ship GenerateShipInOrbit()
		{
			return this.GenerateShipInOrbit(out _);
		}

		private int GetOrbitalSlotCount()
		{
			int additionalSlots = Mathf.FloorToInt(
				this.sourceBody.WorldRadius / this.bodyRadiusPerAdditionalSlot);

			return Mathf.Clamp(
				this.minOrbitalSlots + additionalSlots,
				this.minOrbitalSlots,
				this.maxOrbitalSlots);
		}
	}
}
