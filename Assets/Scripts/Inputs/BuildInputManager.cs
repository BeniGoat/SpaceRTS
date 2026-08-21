using System;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for build actions.
	/// TODO: Move this to its own GameObject in the scene (it no longer lives on the GameManager GameObject)
	/// TODO: Consider implementing a more flexible input system that allows for customizable key bindings and supports multiple input devices.
	/// </summary>
	public class BuildInputManager : MonoBehaviour
	{
		/// <summary>
		/// Event fired when the user presses the build key.
		/// </summary>
		public static event Action OnBuildInput;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
				OnBuildInput?.Invoke();
		}
	}
}