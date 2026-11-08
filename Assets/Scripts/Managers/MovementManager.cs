using System.Collections.Generic;
using SpaceRTS.Models;
using SpaceRTS.Services;
using SpaceRTS.Simulation;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the movement of ships in the game, including setting destinations and processing travel over time.
	/// </summary>
	public class MovementManager : MonoBehaviour, ISimulationSystem
	{
		private readonly List<Ship> activeShips = new();

		private ISimulationClock simulationClock;
		private SimulationClockManager simulationClockManager;
		private float realSecondsPerSimMinute;

		/// <summary>
		/// Gets the priority value used to order this strategy.
		/// </summary>
		public int Priority => 100;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			// Attempt to retrieve the simulation clock and its manager from the service locator.
			if (!ServiceLocator.TryGet(out this.simulationClock) || !ServiceLocator.TryGet(out this.simulationClockManager))
			{
				Debug.LogError("Simulation clock or manager not found. MovementManager requires a simulation clock to function.");
				return;
			}

			// Calculate the real seconds per simulation minute based on the simulation clock's speed.
			this.realSecondsPerSimMinute = (float) (1d / this.simulationClock.BaseSimMinutesPerRealSecond);
			this.simulationClockManager.RegisterSystem(this);
		}

		private void OnDestroy()
		{
			// Unregister from the simulation clock manager when this object is destroyed.
			if (this.simulationClockManager != null)
				this.simulationClockManager.UnregisterSystem(this);
		}

		/// <summary>
		/// Advances travel state for all active ships for the current simulation tick.
		/// </summary>
		/// <remarks>Computes a non-negative time step, removes missing ship references, finalizes ships that have
		/// arrived, and updates in-transit ships.</remarks>
		/// <param name="tick">Simulation tick data that supplies the elapsed simulation minutes used to compute the real-time step.</param>
		public void Execute(in SimulationTick tick)
		{
			// Calculate the time step in real seconds for this simulation tick.
			float tickStepSeconds = Mathf.Max(0f, tick.TickMinutes * this.realSecondsPerSimMinute);

			// Process the movement of all registered ships for this simulation tick.
			MovementExecuteCore.Execute(
				this.activeShips,
				tickStepSeconds,
				ship => ship == null || ship.CurrentSystemBody == null, // Consider a ship missing if it is null or has no current system body.
				ship => ship.HasArrived,
				ship => ship.CompleteTravel(),
				(ship, step) => ship.ProcessTravelTick(step));
		}

		/// <summary>
		/// Attempts to set the destination for a given ship.
		/// If the destination is accepted, the ship is added to the active ships list if not already present.
		/// If the destination is rejected (no factory or no free slot), the existing order is left unchanged.
		/// </summary>
		/// <param name="ship">The ship to set the destination for.</param>
		/// <param name="destination">The destination system body.</param>
		public void SetDestination(Ship ship, SystemBody destination)
		{
			// Validate the ship and destination before proceeding.
			if (ship == null || destination == null)
				return;

			bool accepted = ship.SetDestination(destination);

			// Only register the ship if the order was accepted and it is not already traveling.
			if (accepted && !this.activeShips.Contains(ship))
			{
				this.activeShips.Add(ship);
			}
		}
	}
}