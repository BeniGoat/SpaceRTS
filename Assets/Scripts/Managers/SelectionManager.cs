using SpaceRTS.Events;
using SpaceRTS.Models.Interfaces;
using SpaceRTS.Services;
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
			EventBus.Subscribe<SelectInputEvent>(this.HandleSelectInput);
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<SelectInputEvent>(this.HandleSelectInput);
		}

		/// <summary>
		/// Handles the selection input from the user. It performs a raycast to determine if
		/// a selectable object was clicked, and updates the current selection accordingly.
		/// If a new object is selected, it deselects the previous one and sets the camera target to the new selection. 
		/// If no object is selected, it clears the selection and resets the camera target.
		/// </summary>
		/// <param name="evt">The selection input event containing the screen position of the selection.</param>
		private void HandleSelectInput(SelectInputEvent evt)
		{
			// Perform a raycast to determine if a selectable object was clicked
			ISelectable clicked = this.Raycast(evt.ScreenPosition);

			// If a new object is selected, deselect the previous one and set the camera target to the new selection.
			this.Select(clicked != this.currentSelection ? clicked : null);
		}

		/// <summary>
		/// Selects the specified item, updates the camera target, and publishes a selection changed event.
		/// </summary>
		/// <param name="selection">The item to select, or null to clear the selection.</param>
		public void Select(ISelectable selection)
		{
			// Deselect the current selection if it exists
			this.Deselect();
			this.currentSelection = selection;

			// If a new selection is made, mark it as selected and set the camera target to it.
			if (this.currentSelection != null)
			{
				this.currentSelection.IsSelected = true;
				this.cameraManager.SetTarget(this.currentSelection.GetTransform());				
			}
			else
			{
				// If no selection is made, clear the camera target.
				this.cameraManager.SetTarget(null);
			}

			// Notify other systems about the selection change
			EventBus.Publish(new SelectionChangedEvent { Selection = this.currentSelection });
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
			Ray ray = this.cameraManager.SendRay(screenPosition);
			
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Debug.Log($"Raycast HIT: {hit.transform.name} at distance {hit.distance}");
				if (hit.transform.TryGetComponent<ISelectable>(out var selectable))
				{
                    return selectable;
				}
			}
			
			return null;
		}
	}
}