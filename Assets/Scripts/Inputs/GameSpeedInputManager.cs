using SpaceRTS.Events;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for changing the game's speed. It listens for specific key presses
	/// and publishes events via the EventBus to notify other systems of the requested game speed change.
	/// </summary>
	public class GameSpeedInputManager : MonoBehaviour
	{
		private void Update()
		{
            if (Input.GetKeyDown(KeyCode.Space))
                PublishGameSpeedEvent(GameSpeed.Paused);
            else if (Input.GetKeyDown(KeyCode.Alpha1))
                PublishGameSpeedEvent(GameSpeed.x1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                PublishGameSpeedEvent(GameSpeed.x2);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                PublishGameSpeedEvent(GameSpeed.x5);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                PublishGameSpeedEvent(GameSpeed.x10);
        }

        /// <summary>
        /// Publishes a GameSpeedInputEvent with the requested speed.
        /// </summary>
        private void PublishGameSpeedEvent(GameSpeed speed)
        {
            EventBus.Publish(new GameSpeedInputEvent { RequestedSpeed = speed });
        }
    }
}