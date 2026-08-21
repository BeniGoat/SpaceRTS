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
		private ISelectable currentSelection;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void Start()
		{
			this.cameraManager = ServiceLocator.Get<CameraManager>();
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

		private void HandleCommandInput(CommandInputEvent evt)
		{
			// If the current selection is either null or not a ship, do nothing
			if (this.currentSelection == null || !this.currentSelection.TryGetComponent<Ship>(out var ship))
			{
				return;
			}

			// Perform a raycast from the camera to the screen position of the command input event
			Ray ray = this.cameraManager.SendRay(evt.ScreenPosition);

			// If a system body is hit and it's not the current system body of the ship, set it as the destination
			if (Physics.Raycast(ray, out RaycastHit hit) &&
				hit.transform.TryGetComponent(out SystemBody targetBody) &&
				targetBody != ship.CurrentSystemBody)
			{
				ship.SetDestination(targetBody);
			}
			else
			{
				// If no valid system body is hit, clear the ship's destination
				ship.ClearDestination();
			}
		}
	}
}
