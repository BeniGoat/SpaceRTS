using System;
using System.Collections.Generic;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Provides core travel-execution logic for active ships by advancing movement,
	/// completing arrivals, and removing null or finished entries.
	/// </summary>
	/// <remarks>Designed as a stateless helper for in-place updates of active travel collections, including safe
	/// removal during reverse iteration.</remarks>
	internal static class MovementExecuteCore
	{
		/// <summary>
		/// Advances travel state for each active ship, completes arrived ships, and removes ships that are null or finished.
		/// </summary>
		/// <remarks>Iterates the list from end to start so items can be removed safely during traversal.</remarks>
		/// <typeparam name="TShip">The reference type used to represent a ship in the active travel list.</typeparam>
		/// <param name="activeShips">The list of ships currently in transit, updated in place by removing null entries and ships that finish travel.</param>
		/// <param name="tickStepSeconds">The simulation time step, in seconds, passed to per-ship travel processing.</param>
		/// <param name="isMissing">Determines whether a ship reference is considered missing and should be removed from the active list.</param>
		/// <param name="hasArrived">Determines whether a ship has reached its destination.</param>
		/// <param name="completeTravel">Finalizes arrival handling for a ship and returns whether it should be removed from the active list.</param>
		/// <param name="processTravelTick">Processes one travel tick for a ship that has not yet arrived.</param>
		internal static void Execute<TShip>(
			List<TShip> activeShips,
			float tickStepSeconds,
			Func<TShip, bool> isMissing,
			Func<TShip, bool> hasArrived,
			Func<TShip, bool> completeTravel,
			Action<TShip, float> processTravelTick)
			where TShip : class
		{
			// Iterate backwards through the list of active ships to
			// safely remove any that have completed travel or are null.
			for (int i = activeShips.Count - 1; i >= 0; i--)
			{
				TShip ship = activeShips[i];
				if (ship == null || isMissing(ship))
				{
					activeShips.RemoveAt(i);
					continue;
				}

				if (hasArrived(ship))
				{
					if (completeTravel(ship))
					{
						activeShips.RemoveAt(i);
					}

					continue;
				}

				processTravelTick(ship, tickStepSeconds);
			}
		}
	}
}
