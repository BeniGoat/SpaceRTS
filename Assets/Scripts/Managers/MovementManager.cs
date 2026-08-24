using System.Collections.Generic;
using SpaceRTS.Models;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the movement of ships in the game, including setting destinations and processing travel over time.
	/// </summary>
	public class MovementManager : MonoBehaviour
    {
		private readonly List<Ship> activeShips = new();

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Update()
		{
			// Process the movement of all registered ships.
			// We iterate backwards to safely remove ships that have arrived or are null.
			// This prevents issues with modifying the collection while iterating over it.
			for (int i = this.activeShips.Count - 1; i >= 0; i--)
			{
				Ship ship = this.activeShips[i];
				if (ship == null)
				{
					// If the ship is null, remove it from the list and continue to the next iteration.
					this.activeShips.RemoveAt(i);
					continue;
				}

				// Check if the ship has arrived at its destination.
				// If it has, complete its travel and remove it from the active ships list.
				if (ship.HasArrived)
				{
					ship.CompleteTravel();
					this.activeShips.RemoveAt(i);
					continue;
				}

				// If the ship is still traveling, process its movement for this frame.
				ship.ProcessTravel(Time.deltaTime);				
			}
		}

		/// <summary>
		/// Sets the destination for a given ship and adds it to the active ships list if it's not already present.
		/// </summary>
		/// <param name="ship">The ship to set the destination for.</param>
		/// <param name="destination">The destination system body.</param>
		public void SetDestination(Ship ship, SystemBody destination)
		{
			// Validate the ship and destination before proceeding.
			if (ship == null || destination == null)
			{
				return;
			}

			ship.SetDestination(destination);

			// Add the ship to the active ships list if it's not already present.
			if (!this.activeShips.Contains(ship))
			{
				this.activeShips.Add(ship);
			}
		}

		/// <summary>
		/// Clears the destination for a given ship and removes it from the active ships list.
		/// </summary>
		/// <param name="ship">The ship to clear the destination for.</param>
		public void ClearDestination(Ship ship)
		{
			// Validate the ship before proceeding.
			if (ship == null)
			{
				return;
			}

			ship.ClearDestination();
			this.activeShips.Remove(ship);
		}
	}
}