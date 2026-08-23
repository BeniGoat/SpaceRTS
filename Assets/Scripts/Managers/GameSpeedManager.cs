using SpaceRTS.Events;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
    /// <summary>
    /// Manages the game's speed, including pausing and adjusting the time scale.
    /// It listens for game speed input events and updates the Time.timeScale accordingly. 
    /// It also publishes speed change events to notify other systems of the current game speed.
    /// </summary>
    public class GameSpeedManager : MonoBehaviour
	{
		private const float TimeScaleDivisor = 5f;
        private GameSpeed previousSpeed = GameSpeed.x1;
        private bool isPaused;

        private void Awake()
		{
			ServiceLocator.Register(this);
        }

        private void Start()
        {
            this.isPaused = true;
            this.ApplyTimeScale(GameSpeed.Paused);
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
        /// <remarks>Delegates to SetGameSpeed to apply the requested speed.</remarks>
        /// <param name="evt">Event providing the requested game speed.</param>
        private void HandleGameSpeedInput(GameSpeedInputEvent evt)
        {
            this.SetGameSpeed(evt.RequestedSpeed);
        }

        /// <summary>
        /// Sets the current game speed, updates Time.timeScale, publishes a SpeedChangedEvent, and logs the change.
        /// </summary>
        /// <remarks>If the supplied speed equals the current speed, no changes are made. Time.timeScale
        /// is calculated from the GameSpeed value divided by TimeScaleDivisor. A SpeedChangedEvent is published via
        /// EventBus and a diagnostic log entry is written.</remarks>
        /// <param name="speed">Desired game speed; used to adjust the Time.timeScale accordingly.</param>
        public void SetGameSpeed(GameSpeed speed)
		{
            GameSpeed appliedSpeed;

            // Check if the requested speed is pause request
            if (speed == GameSpeed.Paused)
            {
                if (this.isPaused)
                {
                    // Currently paused, so resume to the previous speed
                    appliedSpeed = this.previousSpeed;
                    this.ApplyTimeScale(this.previousSpeed);
                    this.isPaused = false;
                }
                else
                {
                    // Currently running, so pause
                    appliedSpeed = GameSpeed.Paused;
                    this.ApplyTimeScale(GameSpeed.Paused);
                    this.isPaused = true;
                }
            }
            else
            {
                // If the requested speed is not pause, store the previous speed and apply the new speed
                this.previousSpeed = speed;
                appliedSpeed = speed;
                this.isPaused = false;
                this.ApplyTimeScale(speed);
            }

            EventBus.Publish(new SpeedChangedEvent { Speed = appliedSpeed });
            Debug.Log($"[GameSpeedManager] Game speed changed to: {appliedSpeed}, Time.timeScale = {Time.timeScale}");
        }

        /// <summary>
        /// Applies the time scale based on the provided game speed.
        /// </summary>
        /// <param name="speed">The game speed to apply.</param>
        private void ApplyTimeScale(GameSpeed speed)
        {
            Time.timeScale = (int)speed / TimeScaleDivisor;
        }
    }
}
