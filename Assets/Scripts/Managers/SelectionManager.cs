using System;
using SpaceRTS.Inputs;
using SpaceRTS.Models;
using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Manages the selection of objects in the game world.
	/// It listens for selection input events and performs raycasting to determine which object is selected.
	/// It maintains the current selection state and notifies other systems when the selection changes.
	/// </summary>
	public class SelectionManager : MonoBehaviour
    {
		/// <summary>
		/// Event fired when the selection changes. The parameter is the newly selected object, or null if no object is selected.
		/// </summary>
        public static event Action<ISelectable> OnSelectionChanged;
		
        [SerializeField] private CameraManager cameraManager;
		private ISelectable currentSelection;

		private void OnEnable()
		{
			SelectionInputManager.OnSelectInput += this.HandleSelectInput;
		}

		private void OnDisable()
		{
			SelectionInputManager.OnSelectInput -= this.HandleSelectInput;
		}

		/// <summary>
		/// Handles the selection input from the user. It performs a raycast to determine if
        /// a selectable object was clicked, and updates the current selection accordingly.
        /// If a new object is selected, it deselects the previous one and sets the camera target to the new selection. 
        /// If no object is selected, it clears the selection and resets the camera target.
		/// </summary>
		/// <param name="screenPosition">The screen position where the selection input occurred.</param>
		private void HandleSelectInput(Vector3 screenPosition)
		{
			// Perform a raycast to determine if a selectable object was clicked
			ISelectable clicked = this.Raycast(screenPosition);

			// If a new object is selected, deselect the previous one and set the camera target to the new selection
			if (clicked != null && clicked != this.currentSelection)
			{
				this.Deselect();
				this.currentSelection = clicked;
				this.currentSelection.IsSelected = true;
				
				if (this.currentSelection.GetTransform() is Transform targetTransform)
				{
					this.cameraManager.SetTarget(targetTransform);
				}
			}
			else
			{
				// If no object is selected, clear the selection and reset the camera target
				this.Deselect();
				this.cameraManager.SetTarget(null);
			}

			// Invoke the selection changed event to notify other systems of the new selection
			OnSelectionChanged?.Invoke(this.currentSelection);
		}

		/// <summary>
		/// Performs a raycast from the camera to the specified screen position
        /// and returns the first selectable object hit by the raycast, if any.
		/// </summary>
		private void Deselect()
		{
			if (this.currentSelection != null)
			{
				this.currentSelection.IsSelected = false;
				this.currentSelection = null;
			}
		}

		/// <summary>
		/// Performs a raycast from the camera to the specified screen position and
		/// returns the first selectable object hit by the raycast, if any.
		/// </summary>
		/// <param name="screenPosition">The screen position where the raycast should originate.</param>
		/// <returns>The first selectable object hit by the raycast, or null if none is hit.</returns>
		private ISelectable Raycast(Vector3 screenPosition)
		{
			// Perform a raycast from the camera to the specified screen position
			Ray ray = this.cameraManager.SendRay(screenPosition);

			// Check if the raycast hits a selectable object and return it, otherwise return null
			if (Physics.Raycast(ray, out RaycastHit hit) &&
                hit.transform.TryGetComponent<ISelectable>(out var selectable))
			{
				return selectable;
			}

			return null;
		}
	}
}