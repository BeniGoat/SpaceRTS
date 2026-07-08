using System;
using UnityEngine;
using SpaceRTS.Managers.Enums;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for changing the game's speed. It listens for specific key presses
	/// and invokes an event to notify other systems of the requested game speed change.
	/// </summary>
	public class GameSpeedInputManager : InputManager
	{
		/// <summary>
		/// Event fired when the user requests a change in game speed. The parameter indicates the requested speed.
		/// </summary>
		public static event Action<GameSpeed> OnGameSpeedChanged;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				OnGameSpeedChanged?.Invoke(GameSpeed.Paused);
			else if (Input.GetKeyDown(KeyCode.Alpha1))
				OnGameSpeedChanged?.Invoke(GameSpeed.x1);
			else if (Input.GetKeyDown(KeyCode.Alpha2))
				OnGameSpeedChanged?.Invoke(GameSpeed.x2);
			else if (Input.GetKeyDown(KeyCode.Alpha3))
				OnGameSpeedChanged?.Invoke(GameSpeed.x5);
			else if (Input.GetKeyDown(KeyCode.Alpha4))
				OnGameSpeedChanged?.Invoke(GameSpeed.x10);
		}
	}
}