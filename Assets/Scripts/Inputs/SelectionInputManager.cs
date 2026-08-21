using SpaceRTS.Events;
using SpaceRTS.Services;
using System.Diagnostics;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for selection and command actions in the game.
	/// It listens for mouse button clicks and publishes events via the EventBus
	/// to notify other systems of the user's selection or command input.
	/// TODO: Move this to a new InputManagers GameObject in the scene (it no longer lives on the GameManager GameObject)
	/// </summary>
	public class SelectionInputManager : MonoBehaviour
	{
		private void Update()
		{
			// Check for left mouse button click (selection) 
			if (Input.GetMouseButtonDown(0))
			{
				Debug.Log("Left mouse button clicked at position: " + Input.mousePosition);
				EventBus.Publish(new SelectInputEvent { ScreenPosition = Input.mousePosition });
			}
			// Check for right mouse button click (command)
			else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Right mouse button clicked at position: " + Input.mousePosition);
				EventBus.Publish(new CommandInputEvent { ScreenPosition = Input.mousePosition });
			}
        }
	}
}