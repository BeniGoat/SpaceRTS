using SpaceRTS.Factories;
using SpaceRTS.Inputs;
using SpaceRTS.Managers.Enums;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the overall game state, including initializing the game world, setting up managers, and configuring the camera.
	/// It serves as the central point for coordinating various systems in the game.
	/// </summary>
	[RequireComponent(typeof(SystemFactory))]
	[RequireComponent(typeof(CommandManager))]
    [RequireComponent(typeof(MovementManager))]
	[RequireComponent(typeof(SelectionManager))]
    [RequireComponent(typeof(TimeScaleManager))]
	[RequireComponent(typeof(KeyboardInputManager))]
	[RequireComponent(typeof(MouseInputManager))]
	[RequireComponent(typeof(GameSpeedInputManager))]
	[RequireComponent(typeof(SelectionInputManager))]
	[RequireComponent(typeof(BuildInputManager))]
	[RequireComponent(typeof(ShipSpawner))]
	public class GameManager : MonoBehaviour
    {
		[Header("Factories")]
		private SystemFactory systemFactory;

		[Header("Managers")]
		[SerializeField] private CameraManager cameraManager;
		private CommandManager commandManager;
		private MovementManager movementManager;
		private SelectionManager selectionManager;
		private TimeScaleManager timeScaleManager;
        private KeyboardInputManager keyboardInputManager;
        private MouseInputManager mouseInputManager;
        private GameSpeedInputManager gameSpeedInputManager;
		private SelectionInputManager selectionInputManager;
        private BuildInputManager buildInputManager;
        private ShipSpawner shipSpawner;

        [Header("Settings")]
		[SerializeField] private CameraMode cameraMode = CameraMode.Perspective;

		private void Awake()
        {
            // Get references to required components
            this.systemFactory = this.GetComponent<SystemFactory>();
            this.commandManager = this.GetComponent<CommandManager>();
            this.movementManager = this.GetComponent<MovementManager>();
            this.selectionManager = this.GetComponent<SelectionManager>();
            this.timeScaleManager = this.GetComponent<TimeScaleManager>();
            this.keyboardInputManager = this.GetComponent<KeyboardInputManager>();
            this.mouseInputManager = this.GetComponent<MouseInputManager>();
            this.gameSpeedInputManager = this.GetComponent<GameSpeedInputManager>();
            this.selectionInputManager = this.GetComponent<SelectionInputManager>();
            this.shipSpawner = this.GetComponent<ShipSpawner>();
        }

        private void Start()
		{
			// Spawn the star system and get its size
			float systemSize = this.systemFactory.Initialise();

			// Initialize camera with system bounds
			this.cameraManager.SetCamera(this.cameraMode, (int)Mathf.Ceil(systemSize));
		}
	}
}