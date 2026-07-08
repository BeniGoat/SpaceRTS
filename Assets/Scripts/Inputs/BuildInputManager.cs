using System;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for build actions.
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