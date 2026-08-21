using SpaceRTS.Events;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input for build actions.
	/// TODO: Move this to its own GameObject in the scene (it no longer lives on the GameManager GameObject)
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