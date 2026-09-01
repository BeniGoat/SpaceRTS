using SpaceRTS.Events;
using SpaceRTS.Models;
using SpaceRTS.Models.Interfaces;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Handles user commands for selected ships in the game.
	/// It listens for command input events and performs raycasting to determine if a system body was clicked.
	/// </summary>
	public class CommandManager : MonoBehaviour
	{
		private CameraManager cameraManager;
		private MovementManager movementManager;
		private ISelectable currentSelection;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			this.cameraManager = ServiceLocator.Get<CameraManager>();
			this.movementManager = ServiceLocator.Get<MovementManager>();
		}

		private void OnEnable()
		{
			EventBus.Subscribe<CommandInputEvent>(this.HandleCommandInput);
			EventBus.Subscribe<SelectionChangedEvent>(this.HandleSelectionChanged);
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<CommandInputEvent>(this.HandleCommandInput);
			EventBus.Unsubscribe<SelectionChangedEvent>(this.HandleSelectionChanged);
		}

		/// <summary>
		/// Handles the selection changed event to update the current selection.
		/// </summary>
		/// <param name="evt">The selection changed event.</param>
		private void HandleSelectionChanged(SelectionChangedEvent evt)
		{
			this.currentSelection = evt.Selection;
		}

		/// <summary>
		/// Handles the command input event by checking if the current selection is a ship and performing a raycast to determine if a system body was clicked.
		/// </summary>
		/// <param name="evt">The command input event.</param>
		private void HandleCommandInput(CommandInputEvent evt)
		{
			// If the current selection is either null or not a ship, do nothing
			if (this.currentSelection == null || !this.currentSelection.TryGetComponent<Ship>(out var ship))
			{
				return;
			}

			// Perform a raycast from the camera to the screen position of the command input event
			Ray ray = this.cameraManager.SendRay(evt.ScreenPosition);

			// If a system body is hit and it's not the ship's current system body, attempt to set it as the destination.
			// Invalid targets, full destinations, and clicks on the source body are all silent no-ops.
			if (Physics.Raycast(ray, out RaycastHit hit) &&
				hit.transform.TryGetComponent(out SystemBody targetBody) &&
				targetBody != ship.CurrentSystemBody)
			{
				this.movementManager.SetDestination(ship, targetBody);
			}
		}
	}
}
