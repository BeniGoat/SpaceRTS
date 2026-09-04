using SpaceRTS.Events;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using SpaceRTS.Simulation;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the game speed and pause state, delegating requests to the authoritative simulation clock.
	/// </summary>
	public class GameSpeedManager : MonoBehaviour
	{
		private ISimulationClock simulationClock;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			// Publish the initial speed state to ensure all systems are aware of the current game speed.
			if (this.TryResolveClock())
			{
				EventBus.Publish(new SpeedChangedEvent { Speed = this.simulationClock.EffectiveSpeed });
			}
		}

		private void OnEnable()
		{
			EventBus.Subscribe<GameSpeedInputEvent>(this.HandleGameSpeedInput);
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<GameSpeedInputEvent>(this.HandleGameSpeedInput);
		}

		/// <summary>
		/// Sets the current game speed to the value specified by the event.
		/// </summary>
		private void HandleGameSpeedInput(GameSpeedInputEvent evt)
		{
			this.SetGameSpeed(evt.RequestedSpeed);
		}

		/// <summary>
		/// Delegates speed and pause requests to the authoritative simulation clock.
		/// </summary>
		/// <param name="speed">Requested speed input.</param>
		public void SetGameSpeed(GameSpeed speed)
		{
			// Ensure the simulation clock is resolved before attempting to change the speed.
			if (!this.TryResolveClock())
				return;

			// Apply the requested speed change and publish the resulting effective speed to all subscribers.
			GameSpeed appliedSpeed = this.simulationClock.ApplySpeedRequest(speed);
			EventBus.Publish(new SpeedChangedEvent { Speed = appliedSpeed });
			Debug.Log($"[GameSpeedManager] Game speed changed to: {appliedSpeed}.");
		}

		/// <summary>
		/// Ensures a simulation clock instance is available by using the existing reference or resolving it from the service
		/// locator.
		/// </summary>
		/// <remarks>Logs an error when the simulation clock service is not registered.</remarks>
		/// <returns>true if a simulation clock is available; otherwise, false when the clock service cannot be resolved.</returns>
		private bool TryResolveClock()
		{
			if (this.simulationClock != null)
				return true;

			if (!ServiceLocator.TryGet(out this.simulationClock))
			{
				Debug.LogError("[GameSpeedManager] ISimulationClock is not registered. Ensure SimulationClockManager initializes first.");
				return false;
			}

			return true;
		}
	}
}
