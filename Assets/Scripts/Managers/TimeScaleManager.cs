using System;
using SpaceRTS.Inputs;
using SpaceRTS.Managers.Enums;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the game's time scale based on user input and programmatic changes. It allows for pausing, unpausing,
	/// and changing the game speed while ensuring that the effective game speed is communicated to other systems via events.
	/// </summary>
	public class TimeScaleManager : MonoBehaviour
	{
		/// <summary>
		/// Fired whenever the effective game speed changes (including pause/unpause).
		/// </summary>
		public static event Action<GameSpeed> OnSpeedChanged;

		private const float TimeScaleDivisor = 5f;

		private GameSpeed storedSpeed = GameSpeed.x1;
		private bool isPaused;

		public GameSpeed CurrentSpeed => this.isPaused ? GameSpeed.Paused : this.storedSpeed;
		public bool IsPaused => this.isPaused;

		private void OnEnable()
		{
			GameSpeedInputHandler.OnGameSpeedInput += this.HandleSpeedInput;
		}

		private void OnDisable()
		{
			GameSpeedInputHandler.OnGameSpeedInput -= this.HandleSpeedInput;
		}

		private void Start()
		{
			// Default to paused at start
			this.isPaused = true;
			this.ApplyTimeScale(GameSpeed.Paused);
		}

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
		/// Applies the time scale based on the given game speed and invokes the OnSpeedChanged event.
		/// </summary>
		/// <param name="requestedSpeed">The game speed requested by the input.</param>
		private void HandleSpeedInput(GameSpeed requestedSpeed)
		{
			if (requestedSpeed == GameSpeed.Paused)
			{
				this.TogglePause();
			}
			else
			{
				this.storedSpeed = requestedSpeed;
				this.isPaused = false;
				this.ApplyTimeScale(requestedSpeed);
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
		/// Applies the time scale based on the provided game speed and invokes the OnSpeedChanged event.
		/// </summary>
		/// <param name="speed">The game speed to apply.</param>
		private void ApplyTimeScale(GameSpeed speed)
		{
			Time.timeScale = (int)speed / TimeScaleDivisor;
			OnSpeedChanged?.Invoke(speed);
		}
	}
}
