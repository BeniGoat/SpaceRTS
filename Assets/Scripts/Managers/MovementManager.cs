using System.Collections.Generic;
using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class MovementManager : MonoBehaviour
    {
		private readonly List<Ship> activeShips = new List<Ship>();

		/// <summary>
		/// Registers a ship to be managed for movement updates.
		/// </summary>
		/// <param name="ship">The ship to be registered for movement updates.</param>
		public void RegisterMovingShip(Ship ship)
		{
			if (!this.activeShips.Contains(ship))
				this.activeShips.Add(ship);
		}

		/// <summary>
		/// Unregisters a ship from being managed for movement updates.
		/// </summary>
		/// <param name="ship">The ship to be unregistered from movement updates.</param>
		public void UnregisterMovingShip(Ship ship)
		{
			this.activeShips.Remove(ship);
		}

		private void Update()
		{
			// Process the movement of all registered ships.
			// Ships that have arrived at their destination are completed and removed from the active list.
			for (int i = this.activeShips.Count - 1; i >= 0; i--)
			{
				Ship ship = this.activeShips[i];
				if (ship.HasArrived)
				{
					ship.CompleteTravel();
					this.activeShips.RemoveAt(i);
				}
				else
				{
					ship.ProcessTravel(Time.deltaTime);
				}
			}
		}
	}
}