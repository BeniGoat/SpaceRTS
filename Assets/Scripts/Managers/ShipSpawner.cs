using SpaceRTS.Factories;
using SpaceRTS.Inputs;
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
            // Subscribe to selection change events and build input
            SelectionManager.OnSelectionChanged += this.UpdateSelection;
            BuildInputManager.OnBuildInput += this.TrySpawnShipAtSelection;
        }

        private void OnDisable()
        {
            Debug.Log("[ShipSpawner] OnDisable - Unsubscribing from events");
            // Unsubscribe from events
            SelectionManager.OnSelectionChanged -= this.UpdateSelection;
            BuildInputManager.OnBuildInput -= this.TrySpawnShipAtSelection;
        }

        /// <summary>
        /// Sets the current selection to the specified selectable.
        /// </summary>
        /// <param name="newSelection">The selectable to assign as the current selection; pass null to clear the selection.</param>
        private void UpdateSelection(ISelectable newSelection)
        {
            Debug.Log($"[ShipSpawner] Selection changed to: {(newSelection != null ? newSelection.GetName() : "null")}");
            this.currentSelection = newSelection;
        }

        /// <summary>
        /// Attempts to spawn a ship at the currently selected system body by locating a ShipFactory on the body's
        /// parent and logging the outcome.
        /// </summary>
        /// <remarks>Performs null checks for the current selection and associated SystemBody. If the
        /// parent's ShipFactory is found, calls TrySpawnShip and logs an informational message on success; logs
        /// warnings when spawning fails or when no ShipFactory is present.</remarks>
        private void TrySpawnShipAtSelection()
        {
            Debug.Log("[ShipSpawner] Build input received");

            // Check if we have a selected system body
            if (this.currentSelection == null)
                return;

            Debug.Log($"[ShipSpawner] Current selection: {this.currentSelection.GetName()}");

            // Try to get the SystemBody component from the selected object
            SystemBody selectedBody = this.GetSystemBodyFromSelection(this.currentSelection);
            if (selectedBody == null)
                return;

            Debug.Log($"[ShipSpawner] Got SystemBody: {selectedBody.name}");

            // The ShipFactory is on the Planet/Moon parent container
            ShipFactory shipFactory = selectedBody.transform.parent != null 
                ? selectedBody.transform.parent.GetComponent<ShipFactory>() 
                : null;
            if (shipFactory != null)
            {
                Debug.Log($"[ShipSpawner] Found ShipFactory on parent, attempting spawn");
                shipFactory.TrySpawnShip();
            }
            else
            {
                Debug.LogWarning($"Selected body {selectedBody.name} has no ShipFactory on parent");
            }
        }

        /// <summary>
        /// Retrieves the SystemBody associated with the provided selectable by checking for a SystemBody component on a
        /// SelectableComponent or by returning the CurrentSystemBody from an associated Ship.
        /// </summary>
        /// <remarks>The method first attempts to get a SystemBody component from the
        /// SelectableComponent's GameObject; if none is present it attempts to get a Ship component and returns its
        /// CurrentSystemBody. Returns null when neither component is present or the input is not a
        /// SelectableComponent.</remarks>
        /// <param name="selectable">Selectable to inspect; expected to be a SelectableComponent whose GameObject may contain a SystemBody or
        /// Ship component.</param>
        /// <returns>The found SystemBody, or null if the selectable is not a SelectableComponent and no SystemBody can be
        /// determined.</returns>
        private SystemBody GetSystemBodyFromSelection(ISelectable selectable)
        {
            // The selectable should be a SystemBody
            if (selectable is SelectableComponent selectableComponent)
            {
                SystemBody body = selectableComponent != null 
                    ? selectableComponent.gameObject.GetComponent<SystemBody>() 
                    : null;
                if (body != null)
                {
                    Debug.Log($"[ShipSpawner] Found SystemBody on same GameObject: {body.name}");
                    return body;
                }

                // If this is a ship, spawn from its current system body
                Ship ship = selectableComponent != null 
                    ? selectableComponent.gameObject.GetComponent<Ship>() 
                    : null;
                if (ship != null && ship.CurrentSystemBody != null)
                {
                    Debug.Log($"[ShipSpawner] Selection is a Ship, using CurrentSystemBody: {ship.CurrentSystemBody.name}");
                    return ship.CurrentSystemBody;
                }
            }
            
            Debug.LogWarning("[ShipSpawner] No valid SystemBody found from selection");
            return null;
        }
    }
}