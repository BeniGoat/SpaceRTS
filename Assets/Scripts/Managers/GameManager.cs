using SpaceRTS.Factories;
using SpaceRTS.Inputs;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the overall game state, including the initialization of the star system and camera settings.
	/// </summary>
	public class GameManager : MonoBehaviour
    {
		[SerializeField] private SystemFactory systemFactory;
		[SerializeField] private CameraManager cameraManager;
		[SerializeField] private CameraMode cameraMode = CameraMode.Perspective;

        private void Start()
		{
			// Spawn the star system and get its size
			float systemSize = this.systemFactory.Initialise();

			// Initialize camera with system bounds
			this.cameraManager.SetCamera(this.cameraMode, (int)Mathf.Ceil(systemSize));
		}

		private void OnDestroy()
		{
			// Clear the service locator when the game manager is destroyed
			ServiceLocator.Clear();
		}
	}
}