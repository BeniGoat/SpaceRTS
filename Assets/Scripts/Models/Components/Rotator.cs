using SpaceRTS.Events;
using SpaceRTS.Services;
using SpaceRTS.Simulation;
using UnityEngine;

namespace SpaceRTS.Models.Components
{
    /// <summary>
    /// Component that manages the rotation of an object.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        /// <summary>
        /// The axis of rotation
        /// </summary>
        private Vector3 axis;

		/// <summary>
		/// Base rotation speed in degrees per second at x1 speed.
		/// </summary>
		private float baseDegreesPerSecond;

		/// <summary>
		/// Speed multiplier derived from the simulation clock/game speed.
		/// Defaults to 0 so startup remains paused until speed is known.
		/// </summary>
		private int speedMultiplier;

		private void OnEnable()
		{
			EventBus.Subscribe<SpeedChangedEvent>(this.HandleSpeedChanged);
			this.SyncWithCurrentClockSpeed();
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<SpeedChangedEvent>(this.HandleSpeedChanged);
		}

		private void Update()
        {
			// Rotate the object around its up axis at the effective speed,
			// which is the base speed multiplied by the current speed multiplier.
			float effectiveDegreesPerSecond = this.baseDegreesPerSecond * this.speedMultiplier;
			this.transform.Rotate(this.axis, effectiveDegreesPerSecond * Time.unscaledDeltaTime);
		}

		/// <summary>
		/// Sets the base rotation speed (x1 speed).
		/// </summary>
		/// <param name="degreesPerSecond">The rotation speed in degrees per second at x1 speed.</param>
		/// <param name="axis">The axis of rotation.</param>
		public void SetBaseRotationSpeed(float degreesPerSecond, Vector3 axis)
        {
            this.axis = axis;
            this.baseDegreesPerSecond = degreesPerSecond;
        }

		/// <summary>
		/// Updates the speed multiplier when a speed change event is received.
		/// </summary>
		/// <remarks>Converts the event speed to an integer before assigning it to the speed multiplier.</remarks>
		/// <param name="evt">Event data containing the new speed value.</param>
		private void HandleSpeedChanged(SpeedChangedEvent evt) => this.speedMultiplier = (int)evt.Speed;

		/// <summary>
		/// Synchronizes the local speed multiplier with the simulation clock’s current effective speed.
		/// </summary>
		/// <remarks>Sets the speed multiplier to 0 when a simulation clock instance is unavailable.</remarks>
		private void SyncWithCurrentClockSpeed()
		{
			if (ServiceLocator.TryGet(out ISimulationClock clock))
			{
				this.speedMultiplier = (int)clock.EffectiveSpeed;
				return;
			}

			this.speedMultiplier = 0;
		}
	}
}