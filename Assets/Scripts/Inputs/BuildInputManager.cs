using SpaceRTS.Events;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for build actions.
	/// </summary>
	public class BuildInputManager : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
				EventBus.Publish(new BuildInputEvent());
		}
	}
}