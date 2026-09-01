using SpaceRTS.Models;
using SpaceRTS.Models.Interfaces;

namespace SpaceRTS.Services
{
	/// <summary>
	/// Resolves a system body from a selectable object.
	/// Supports direct SystemBody selections and Ship selections via CurrentSystemBody.
	/// </summary>
	public sealed class SelectionBodyResolver : ISelectionBodyResolver
	{
		/// <inheritdoc/>
		public SystemBody Resolve(ISelectable selection)
		{
			if (selection is not SelectableComponent selectableComponent)			
				return null;			

			if (selectableComponent.TryGetComponent(out SystemBody body))
				return body;
			
			if (selectableComponent.TryGetComponent(out Ship ship))
				return ship.CurrentSystemBody;
			
			return null;
		}
	}
}
