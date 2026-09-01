using SpaceRTS.Models.Interfaces;

namespace SpaceRTS.Services
{
	/// <summary>
	/// Minimal abstraction for selecting an item in the world.
	/// </summary>
	public interface ISelectionService
	{
		/// <summary>
		/// Selects the specified item, updates the camera target, and publishes a selection changed event.
		/// </summary>
		/// <param name="selection">The item to select, or null to clear the selection.</param>
		void Select(ISelectable selection);
	}
}
