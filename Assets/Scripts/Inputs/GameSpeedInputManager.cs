using SpaceRTS.Events;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for changing the game's speed. It listens for specific key presses
	/// and publishes events via the EventBus to notify other systems of the requested game speed change.
	/// TODO: Move this to a new InputManagers GameObject in the scene (it no longer lives on the GameManager GameObject)
	/// </summary>
	public class GameSpeedInputManager : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				EventBus.Publish(new GameSpeedInputEvent { RequestedSpeed = GameSpeed.Paused });
			else if (Input.GetKeyDown(KeyCode.Alpha1))
				EventBus.Publish(new GameSpeedInputEvent { RequestedSpeed = GameSpeed.x1 });
			else if (Input.GetKeyDown(KeyCode.Alpha2))
				EventBus.Publish(new GameSpeedInputEvent { RequestedSpeed = GameSpeed.x2 });
			else if (Input.GetKeyDown(KeyCode.Alpha3))
				EventBus.Publish(new GameSpeedInputEvent { RequestedSpeed = GameSpeed.x5 });
			else if (Input.GetKeyDown(KeyCode.Alpha4))
				EventBus.Publish(new GameSpeedInputEvent { RequestedSpeed = GameSpeed.x10 });
		}
	}
}