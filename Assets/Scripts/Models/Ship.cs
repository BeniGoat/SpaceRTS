using SpaceRTS.Factories;
using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents a ship in the game, which can be selected, moved to a destination system body, and visualized with a path line.
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	[RequireComponent(typeof(SelectableComponent))]
	public class Ship : MonoBehaviour
	{
		[SerializeField] private float travelSpeed = 5f;
		[SerializeField] private Color pathLineColour = new Color(0.1f, 1, 0.1f, 0.5f);
		[SerializeField] private float travelFacingYawOffsetDegrees = 0f;

		private ISelectable selectable;
		private LineRenderer path;
		private SystemBody destinationBody;

		/// <summary>
		/// Tracks the factory at which this ship currently has a reserved destination slot.
		/// </summary>
		private ShipFactory destinationFactory;

		/// <summary>
		/// Reservation handle for the destination slot while the ship is in transit.
		/// </summary>
		private OrbitalSlotReservation destinationReservation;

		/// <summary>
		/// Occupied slot handle for the ship's current orbit location.
		/// </summary>
		private OrbitalOccupiedSlot occupiedSlot;

		/// <summary>
		/// Gets or sets the current system body where the ship is located.
		/// </summary>
		public SystemBody CurrentSystemBody { get; set; }

		/// <summary>
		/// Gets a value indicating whether the ship has arrived at its destination system body.
		/// </summary>
		internal bool HasArrived { get; private set; }

		private void Awake()
		{
			this.selectable = this.GetComponent<SelectableComponent>();
			this.selectable.ConfigureSelectionOutline(1f);
			this.ConfigureShipPathLine();
		}

		/// <summary>
		/// Attempts to set the destination system body for the ship.
		/// A valid destination reserves a specific slot at the target factory.
		/// </summary>
		/// <param name="target">The target system body to set as the destination.</param>
		/// <returns>True if the destination was successfully set; otherwise, false.</returns>
		public bool SetDestination(SystemBody target)
		{
			// Reserve a slot at the target factory for the ship's arrival.
			ShipFactory targetFactory = ShipFactory.GetForBody(target);
			if (targetFactory == null || !targetFactory.TryReserveSlot(out OrbitalSlotReservation targetReservation))
				return false;

			// Release the current occupied slot if the ship is already in orbit.
			if (this.occupiedSlot.IsValid)
			{
				ShipFactory sourceFactory = ShipFactory.GetForBody(this.CurrentSystemBody);
				if (sourceFactory == null || !sourceFactory.NotifyDeparture(this.occupiedSlot))
				{
					targetFactory.ReleaseReservation(targetReservation);
					return false;
				}

				this.occupiedSlot = OrbitalOccupiedSlot.None;
				this.transform.SetParent(null, worldPositionStays: true);
			}

			// Replace previous destination reservation only after the new reservation is secured.
			if (this.destinationFactory != null && this.destinationReservation.IsValid)
			{
				this.destinationFactory.ReleaseReservation(this.destinationReservation);
			}

			this.destinationBody = target;
			this.destinationFactory = targetFactory;
			this.destinationReservation = targetReservation;
			this.HasArrived = false;
			this.path.enabled = true;
			this.path.forceRenderingOff = false;
			return true;
		}

		/// <summary>
		/// Initializes and configures the ship path LineRenderer with path display defaults, width, color gradient,
		/// world-space rendering, and two positions.
		/// </summary>
		/// <remarks>Disables rendering initially, sets non-looping world-space behavior, applies a semi-transparent
		/// start color based on the configured path line color, and sets a fixed thin line width.</remarks>
		private void ConfigureShipPathLine()
		{
			this.path = this.GetComponent<LineRenderer>();
			this.path.enabled = false;
			this.path.forceRenderingOff = true;
			this.path.useWorldSpace = true;
			this.path.loop = false;
			this.path.startWidth = 0.01f;
			this.path.endWidth = 0.01f;
			this.path.startColor = new Color(
				this.pathLineColour.r,
				this.pathLineColour.g,
				this.pathLineColour.b,
				this.pathLineColour.a * 0.5f);
			this.path.endColor = this.pathLineColour;
			this.path.positionCount = 2;
		}

		/// <summary>
		/// Updates the path line endpoints to the current object position and the destination body's position.
		/// </summary>
		private void UpdatePathLine()
		{
			this.path.SetPosition(0, this.transform.position);
			this.path.SetPosition(1, this.destinationBody.transform.position);
		}

		/// <summary>
		/// Commits the reserved destination slot and places the ship in orbit.
		/// </summary>
		/// <returns><c>true</c> if the travel was successfully completed; otherwise, <c>false</c>.</returns>
		internal bool CompleteTravel()
		{
			if (this.destinationFactory == null || !this.destinationReservation.IsValid)			
				return false;

			// Commit the reserved slot at the destination factory.
			// If the commit fails, keep the route context and continue trying on later frames.
			if (!this.destinationFactory.CommitReservedArrival(
				this,
				this.destinationReservation,
				out OrbitalOccupiedSlot committedSlot))
			{
				// Keep route context and continue trying on later frames.
				this.HasArrived = false;
				return false;
			}

			this.occupiedSlot = committedSlot;
			this.destinationFactory = null;
			this.destinationReservation = OrbitalSlotReservation.None;
			this.destinationBody = null;
			this.HasArrived = false;
			this.path.enabled = false;
			this.path.forceRenderingOff = true;
			return true;
		}

		/// <summary>
		/// Advances travel toward the current destination body, updates orientation and path visualization, and marks arrival
		/// when within the destination orbital radius.
		/// </summary>
		/// <remarks>Returns immediately when no destination body is set.</remarks>
		/// <param name="deltaTime">Elapsed time since the previous update, in seconds.</param>
		internal void ProcessTravel(float deltaTime)
		{
			if (this.destinationBody == null)			
				return;

			// Move the ship towards the destination body at the specified travel speed, scaled by deltaTime.
			Vector3 targetPosition = this.destinationBody.transform.position;
			this.transform.position = Vector3.MoveTowards(
				this.transform.position,
				targetPosition,
				this.travelSpeed * deltaTime);

			// Update the ship's rotation to face the direction of travel, applying any yaw offset for visual orientation.
			Vector3 direction = targetPosition - this.transform.position;
			if (direction.sqrMagnitude > 0f)
			{
				this.transform.rotation =
					Quaternion.LookRotation(direction) *
					Quaternion.Euler(0f, this.travelFacingYawOffsetDegrees, 0f);
			}

			this.UpdatePathLine();

			if (Vector3.Distance(this.transform.position, targetPosition) <= this.destinationBody.OrbitalRadius)
			{
				this.HasArrived = true;
			}
		}

		/// <summary>
		/// Marks this ship as occupying a specific orbit slot.
		/// </summary>
		/// <param name="slot">The orbital slot to occupy.</param>
		internal void SetInOrbit(OrbitalOccupiedSlot slot) => this.occupiedSlot = slot;
	}
}
