using SpaceRTS.Managers.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the overall game state, including initializing the game world, setting up managers, and configuring the camera.
	/// It serves as the central point for coordinating various systems in the game.
	/// </summary>
	public partial class GameManager : MonoBehaviour
    {
		[Header("Prefabs")]
		[SerializeField] private Models.System systemPrefab;

		[Header("Managers")]
		[SerializeField] private CameraManager cameraManager;
		[SerializeField] private MovementManager movementManager;
		[SerializeField] private SelectionManager selectionManager;
		[SerializeField] private TimeScaleManager timeScaleManager;

		[Header("Settings")]
		[SerializeField] private CameraMode cameraMode = CameraMode.Perspective;

		private void Start()
		{
			// Instantiate the system
			Models.System system = Instantiate(this.systemPrefab);

			// Initialize camera with system bounds
			this.cameraManager.SetCamera(this.cameraMode, system.Size);
		}
	}
}