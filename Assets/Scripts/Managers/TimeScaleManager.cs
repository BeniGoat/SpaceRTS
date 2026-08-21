using SpaceRTS.Events;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the game's time scale based on user input and programmatic changes. It allows for pausing, unpausing,
	/// and changing the game speed while ensuring that the effective game speed is communicated to other systems via events.
	/// </summary>
	public class TimeScaleManager : MonoBehaviour
	{
		private const float TimeScaleDivisor = 5f;

		private GameSpeed storedSpeed = GameSpeed.x1;
		private bool isPaused;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void OnEnable()
		{
			EventBus.Subscribe<GameSpeedInputEvent>(this.HandleSpeedInput);
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<GameSpeedInputEvent>(this.HandleSpeedInput);
		}

		private void Start()
		{
			// Default to paused at start
			this.isPaused = true;
			this.ApplyTimeScale(GameSpeed.Paused);
		}

		/// <summary>
		/// Gets the current effective game speed, which is either the stored speed or paused if the game is currently paused.
		/// </summary>
		public GameSpeed CurrentSpeed => this.isPaused ? GameSpeed.Paused : this.storedSpeed;

		/// <summary>
		/// Gets a value indicating whether the game is currently paused.
		/// </summary>
		public bool IsPaused => this.isPaused;

		/// <summary>
		/// Programmatically set game speed from other systems.
		/// </summary>
		/// <param name="speed">The game speed to set.</param>
		public void SetSpeed(GameSpeed speed)
		{
			if (speed == GameSpeed.Paused)
			{
				this.Pause();
			}
			else
			{
				this.storedSpeed = speed;
				this.isPaused = false;
				this.ApplyTimeScale(speed);
			}
		}

		/// <summary>
		/// Toggles pause state. When unpausing, restores the previously stored speed.
		/// </summary>
		public void TogglePause()
		{
			this.isPaused = !this.isPaused;
			GameSpeed effectiveSpeed = this.isPaused ? GameSpeed.Paused : this.storedSpeed;
			this.ApplyTimeScale(effectiveSpeed);
		}

		/// <summary>
		/// Applies the time scale based on the given game speed.
		/// </summary>
		/// <param name="evt">The event containing the game speed requested by the input.</param>
		private void HandleSpeedInput(GameSpeedInputEvent evt)
		{
			if (evt.RequestedSpeed == GameSpeed.Paused)
			{
				this.TogglePause();
			}
			else
			{
				this.storedSpeed = evt.RequestedSpeed;
				this.isPaused = false;
				this.ApplyTimeScale(evt.RequestedSpeed);
			}
		}

		/// <summary>
		/// Pauses the game by setting the time scale to zero and updating the pause state.
		/// </summary>
		private void Pause()
		{
			this.isPaused = true;
			this.ApplyTimeScale(GameSpeed.Paused);
		}

		/// <summary>
		/// Applies the time scale based on the provided game speed.
		/// </summary>
		/// <param name="speed">The game speed to apply.</param>
		private void ApplyTimeScale(GameSpeed speed)
		{
			Time.timeScale = (int)speed / TimeScaleDivisor;
			EventBus.Publish(new SpeedChangedEvent { Speed = speed });
		}
	}
}
