using SpaceRTS.Events;
using SpaceRTS.Factories;
using SpaceRTS.Models;
using SpaceRTS.Models.Interfaces;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
    /// <summary>
    /// Manages spawning of ships when a system body is selected and the build input is received.
    /// </summary>
    public class ShipSpawner : MonoBehaviour
    {
        private ISelectable currentSelection;

		private void Awake()
		{
			ServiceLocator.Register(this);
		}

		private void OnEnable()
        {
            Debug.Log("[ShipSpawner] OnEnable - Subscribing to events");
            EventBus.Subscribe<SelectionChangedEvent>(this.HandleSelectionChanged);
            EventBus.Subscribe<BuildInputEvent>(this.HandleBuildInput);
        }

        private void OnDisable()
        {
            Debug.Log("[ShipSpawner] OnDisable - Unsubscribing from events");
            EventBus.Unsubscribe<SelectionChangedEvent>(this.HandleSelectionChanged);
            EventBus.Unsubscribe<BuildInputEvent>(this.HandleBuildInput);
        }

        private void HandleSelectionChanged(SelectionChangedEvent evt)
        {
            Debug.Log($"[ShipSpawner] Selection changed to: {(evt.Selection != null ? evt.Selection.GetName() : "null")}");
            this.currentSelection = evt.Selection;
        }

		/// <summary>
		/// Handles the build input event by attempting to spawn a ship at the currently selected system body.
		/// </summary>
		/// <param name="evt">The build input event.</param>
		private void HandleBuildInput(BuildInputEvent evt)
        {
            this.TrySpawnShipAtSelection();
        }

		/// <summary>
		/// Attempts to spawn a ship at the currently selected system body if it has a ShipFactory.
		/// </summary>
		private void TrySpawnShipAtSelection()
        {
            // Check if we have a selected system body
            if (this.currentSelection == null)
                return;

            // Try to get the SystemBody component from the selected object
            SystemBody selectedBody = this.GetSystemBodyFromSelection(this.currentSelection);
            if (selectedBody == null)
                return;

			ShipFactory shipFactory = ShipFactory.GetForBody(selectedBody);
			if (shipFactory != null)
			{
			    // Attempt to spawn a new ship using the ShipFactory
				Ship newShip = shipFactory.GenerateShipInOrbit();

				if (newShip != null)
				{
					// Mark the ship as being in orbit so departure tracking works correctly
					newShip.SetInOrbit();

					// If a new ship was successfully spawned, select it using the SelectionManager
					ISelectable selectableShip = newShip.GetComponent<ISelectable>();
					if (selectableShip != null)
					{
						ServiceLocator.Get<SelectionManager>().Select(selectableShip);
					}
				}
			}
        }

		/// <summary>
		/// Attempts to retrieve the SystemBody component from the selected object.
        /// If the selected object is a Ship, it will return the CurrentSystemBody of that ship.
        /// If no valid SystemBody is found, it returns null.
		/// </summary>
		/// <param name="selectable">The selectable object to retrieve the SystemBody from.</param>
		/// <returns>The SystemBody component if found; otherwise, null.</returns>
		private SystemBody GetSystemBodyFromSelection(ISelectable selectable)
        {
			// Check if the selectable is a SelectableComponent
			if (selectable is SelectableComponent selectableComponent)
            {
				// First, check if the selectable's GameObject has a SystemBody component
				SystemBody body = selectableComponent != null 
                    ? selectableComponent.gameObject.GetComponent<SystemBody>() 
                    : null;
                if (body != null)                
                    return body;                

				// If not, check if the selectable's GameObject has a Ship component and get its CurrentSystemBody
				Ship ship = selectableComponent != null 
                    ? selectableComponent.gameObject.GetComponent<Ship>() 
                    : null;
                if (ship != null && ship.CurrentSystemBody != null)                
                    return ship.CurrentSystemBody;                
            }
            
            return null;
        }
    }
}