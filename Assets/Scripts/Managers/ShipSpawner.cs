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
		/// <summary>
		/// The currently selected object in the game.
		/// This is used to determine where to spawn a ship when the build input is received.
		/// </summary>
		private ISelectable currentSelection;

		/// <summary>
		/// Resolves selection bodies for selection operations.
		/// </summary>
		private ISelectionBodyResolver selectionBodyResolver;

		private void Awake()
		{
			ServiceLocator.Register(this);

			// Ensure that a SelectionBodyResolver is registered in the ServiceLocator. If not, create and register a new instance.
			if (!ServiceLocator.TryGet(out this.selectionBodyResolver))
			{
				this.selectionBodyResolver = new SelectionBodyResolver();
				ServiceLocator.Register(this.selectionBodyResolver);
			}
		}

		private void OnEnable()
		{
			EventBus.Subscribe<SelectionChangedEvent>(this.HandleSelectionChanged);
			EventBus.Subscribe<BuildInputEvent>(this.HandleBuildInput);
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<SelectionChangedEvent>(this.HandleSelectionChanged);
			EventBus.Unsubscribe<BuildInputEvent>(this.HandleBuildInput);
		}

		/// <summary>
		/// Handles the selection changed event to update the current selection.
		/// </summary>
		/// <param name="evt">The selection changed event.</param>
		private void HandleSelectionChanged(SelectionChangedEvent evt) => this.currentSelection = evt.Selection;

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
			if (this.currentSelection == null)			
				return;

			// Resolve the selected system body from the current selection. If no system body is found, return early.
			SystemBody selectedBody = this.selectionBodyResolver.Resolve(this.currentSelection);
			if (selectedBody == null)
				return;			
			
			// Get the ship factory for the selected system body. If no ship factory is found, return early.
			ShipFactory shipFactory = ShipFactory.GetForBody(selectedBody);
			if (shipFactory == null)
				return;

			// Generate a new ship in orbit using the ship factory. If no ship can be generated, return early.
			Ship newShip = shipFactory.GenerateShipInOrbit(out OrbitalOccupiedSlot occupiedSlot);
			if (newShip == null)
				return;

			// Set the ship in orbit at the occupied slot and publish a ShipBuiltEvent if the ship is selectable.
			newShip.SetInOrbit(occupiedSlot);
			if (newShip.TryGetComponent<ISelectable>(out var selectableShip))
			{
				EventBus.Publish(new ShipBuiltEvent { ShipSelection = selectableShip });
			}
		}
	}
}
