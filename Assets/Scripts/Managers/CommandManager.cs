using SpaceRTS.Inputs;
using SpaceRTS.Models;
using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Handles user commands for selected ships in the game.
	/// It listens for command input events and performs raycasting to determine if a system body was clicked.
	/// </summary>
	public class CommandManager : MonoBehaviour
	{
		[SerializeField] private CameraManager cameraManager;
		private ISelectable currentSelection;

		private void OnEnable()
		{
			// Subscribe to the command input event and selection changed event
			SelectionInputManager.OnCommandInput += this.HandleCommandInput;
			SelectionManager.OnSelectionChanged += this.HandleSelectionChanged;
			BuildInputManager.OnBuildInput += this.HandleBuildInput;
		}

		private void OnDisable()
		{
			// Unsubscribe from the command input event and selection changed event
			SelectionInputManager.OnCommandInput -= this.HandleCommandInput;
			SelectionManager.OnSelectionChanged -= this.HandleSelectionChanged;
			BuildInputManager.OnBuildInput -= this.HandleBuildInput;
		}

		/// <summary>
		/// Handles the selection change event. It updates the current selection.
		/// </summary>
		/// <param name="selection">The newly selected object.</param>
		private void HandleSelectionChanged(ISelectable selection)
		{
			this.currentSelection = selection;
		}

		/// <summary>
		/// Handles the command input from the user. It performs a raycast to determine
		/// if a system body was clicked and sets the destination for the currently selected ship.
		/// </summary>
		/// <param name="screenPosition">The screen position of the mouse click.</param>
		private void HandleCommandInput(Vector3 screenPosition)
		{
			// If the current selection is not a ship, do nothing
			if (!this.currentSelection.TryGetComponent<Ship>(out var ship))
			{
				return;
			}

			// Perform a raycast to determine if a system body was clicked
			Ray ray = this.cameraManager.SendRay(screenPosition);

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

		/// <summary>
		/// Handles the build input. If a SystemBody is selected, attempts to spawn a ship
		/// using the ShipFactory attached to the body's parent planet.
		/// </summary>
		private void HandleBuildInput()
		{
    		if (!this.currentSelection.TryGetComponent<SystemBody>(out var selectedBody))
			{
				return;
			}

			// Find the ShipFactory on the selected body's parent hierarchy
			ShipFactory shipFactory = selectedBody.GetComponentInParent<ShipFactory>();
			if (shipFactory == null) return;

			shipFactory.TrySpawnShip();
		}
	}
}
