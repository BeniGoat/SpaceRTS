using System;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for selection and command actions in the game.
	/// It listens for mouse button clicks and invokes events to notify other systems of the user's selection or command input.
	/// </summary>
	public class SelectionInputManager : MonoBehaviour
	{
		/// <summary>
		/// Event fired when the user clicks the left mouse button. The parameter is the mouse position in screen coordinates.
		/// </summary>
		public static event Action<Vector3> OnSelectInput;

		/// <summary>
		/// Event fired when the user clicks the right mouse button. The parameter is the mouse position in screen coordinates.
		/// </summary>
		public static event Action<Vector3> OnCommandInput;

		private void Update()
		{
			// Check for left mouse button click (selection) 
			if (Input.GetMouseButtonDown(0))
				OnSelectInput?.Invoke(Input.mousePosition);
			// Check for right mouse button click (command)
			else if (Input.GetMouseButtonDown(1))
				OnCommandInput?.Invoke(Input.mousePosition);
		}
	}
}