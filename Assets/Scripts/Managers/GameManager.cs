using SpaceRTS.Factories;
using SpaceRTS.Managers.Enums;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the overall game state, including initializing the game world, setting up managers, and configuring the camera.
	/// It serves as the central point for coordinating various systems in the game.
	/// </summary>
	public class GameManager : MonoBehaviour
    {
		[Header("Factories")]
		[SerializeField] private SystemFactory systemFactory;

		[Header("Managers")]
		[SerializeField] private CameraManager cameraManager;
		[SerializeField] private CommandManager commandManager;
		[SerializeField] private MovementManager movementManager;
		[SerializeField] private SelectionManager selectionManager;
		[SerializeField] private TimeScaleManager timeScaleManager;

		[Header("Settings")]
		[SerializeField] private CameraMode cameraMode = CameraMode.Perspective;

		private void Start()
		{
			// Spawn the star system and get its size
			float systemSize = this.systemFactory.Initialise();

			// Initialize camera with system bounds
			this.cameraManager.SetCamera(this.cameraMode, systemSize);
		}
	}
}