using SpaceRTS.Events;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Selects ships after successful user build events.
	/// </summary>
	public class ShipBuildSelectionHandler : MonoBehaviour
	{
		private ISelectionService selectionService;

		private void Awake()
		{
			// Avoid throwing if SelectionManager has not registered yet.
			ServiceLocator.TryGet(out this.selectionService);
		}

		private void OnEnable()
		{
			EventBus.Subscribe<ShipBuiltEvent>(this.HandleShipBuilt);
		}

		private void OnDisable()
		{
			EventBus.Unsubscribe<ShipBuiltEvent>(this.HandleShipBuilt);
		}

		/// <summary>
		/// Handles the ShipBuiltEvent by selecting the newly built ship if it has a valid selection.
		/// </summary>
		/// <param name="evt">The ShipBuiltEvent containing the ship selection information.</param>
		private void HandleShipBuilt(ShipBuiltEvent evt)
		{
			if (evt.ShipSelection == null)
			{
				return;
			}

			// Late resolve in case registration happened after this object's Awake.
			if (this.selectionService == null && !ServiceLocator.TryGet(out this.selectionService))
			{
				return;
			}

			this.selectionService.Select(evt.ShipSelection);
		}
	}
}
