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
		[SerializeField] private float travelSpeed = 1f;
		[SerializeField] private Color pathLineColour = new Color(0.1f, 1, 0.1f, 0.5f);

		private ISelectable selectable;
		private LineRenderer path;
		private SystemBody destinationBody;

		/// <summary>
		/// Tracks the factory at which this ship has a reserved orbital slot.
		/// </summary>
		private ShipFactory destinationFactory;

		/// <summary>
		/// True while the ship is physically present in an orbital slot (not in transit).
		/// </summary>
		private bool isInOrbit;

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
			// Configure the selection outline for the ship on awake
			this.selectable = this.GetComponent<SelectableComponent>();
			this.selectable.ConfigureSelectionOutline(1f);
			this.ConfigureShipPathLine();
		}

		/// <summary>
		/// Attempts to set the destination system body for the ship.
		/// If the destination is valid and has a free slot, the ship reserves that slot,
		/// releases its current orbital slot exactly once, and detaches from its source parent.
		/// If the ship already has a pending reservation for a different factory, that reservation
		/// is released before the new one is made.
		/// </summary>
		/// <param name="target">The target system body to set as the destination.</param>
		/// <returns><c>true</c> if the destination was accepted; <c>false</c> if the target has no valid factory or no free slots.</returns>
		public bool SetDestination(SystemBody target)
		{
			ShipFactory targetFactory = ShipFactory.GetForBody(target);
			if (targetFactory == null || !targetFactory.TryReserveSlot())
				return false;

			// Release any existing reservation at the previous destination factory
			if (this.destinationFactory != null)
				this.destinationFactory.ReleaseReservation();

			// If the ship is still in orbit (first order since spawning or arrival), notify departure
			if (this.isInOrbit)
			{
				ShipFactory sourceFactory = ShipFactory.GetForBody(this.CurrentSystemBody);
				if (sourceFactory == null)
				{
					Debug.LogWarning($"Ship {this.name} is in orbit at {this.CurrentSystemBody.name}, but no source factory was found.");
					targetFactory.ReleaseReservation(); // ← release the slot we just claimed
					return false;
				}
				
				sourceFactory.NotifyDeparture();
				this.isInOrbit = false;

				// Detach from the source parent so the ship travels in world space
				this.transform.SetParent(null, worldPositionStays: true);
			}

			this.destinationBody = target;
			this.destinationFactory = targetFactory;
			this.HasArrived = false;
			this.path.enabled = true;
			this.path.forceRenderingOff = false;
			return true;
		}

		/// <summary>
		/// Configures the LineRenderer component for the ship's path line visualization.
		/// </summary>
		private void ConfigureShipPathLine()
		{
			// Configure the LineRenderer component for the ship's path line visualization
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
		/// Updates the positions of the path line renderer to visualize the path
		/// from the current system body to the destination system body.
		/// </summary>
		private void UpdatePathLine()
		{
			this.path.SetPosition(0, this.transform.position);
			this.path.SetPosition(1, this.destinationBody.transform.position);
		}

		/// <summary>
		/// Completes the ship's travel by committing its reserved orbital slot at the destination.
		/// The ship is moved into orbit, its <see cref="CurrentSystemBody"/> is updated, and
		/// the path line is disabled. If placement fails, the reservation has already been consumed.
		/// </summary>
		internal void CompleteTravel()
		{
			if (this.destinationFactory == null)
				return;

			// Commit the reserved slot and place the ship in orbit
			this.destinationFactory.CommitReservedArrival(this);

			this.destinationFactory = null;
			this.destinationBody = null;
			this.HasArrived = false;
			this.isInOrbit = true;
			this.path.enabled = false;
			this.path.forceRenderingOff = true;
		}

		/// <summary>
		/// Processes the ship's travel towards its destination system body based on the specified delta time.
		/// When the ship enters the destination's orbital radius, <see cref="HasArrived"/> is set to <c>true</c>.
		/// </summary>
		/// <param name="deltaTime">The time elapsed since the last frame.</param>
		internal void ProcessTravel(float deltaTime)
		{
			// If there is no destination body set, return early
			if (this.destinationBody == null)
			{
				return;
			}

			// Move the ship towards the destination body at the specified travel speed
			Vector3 targetPosition = this.destinationBody.transform.position;
			this.transform.position = Vector3.MoveTowards(
				this.transform.position,
				targetPosition,
				this.travelSpeed * deltaTime);

			// Rotate the ship so that the ship is pointing towards the destination body
			this.transform.LookAt(targetPosition);

			//Update the path line to reflect the ship's current position and the destination
			this.UpdatePathLine();

			// Check if the ship has arrived at the destination body based on the orbital radius
			if (Vector3.Distance(this.transform.position, targetPosition) <= this.destinationBody.OrbitalRadius)
			{
				this.HasArrived = true;
			}
		}

		/// <summary>
		/// Marks this ship as being in orbit. Called after the ship is first placed in an orbital slot.
		/// </summary>
		internal void SetInOrbit()
		{
			this.isInOrbit = true;
		}
	}
}
