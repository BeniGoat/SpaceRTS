using SpaceRTS.Models;
using SpaceRTS.Models.Interfaces;

namespace SpaceRTS.Services
{
	/// <summary>
	/// Resolves the effective <see cref="SystemBody"/> from the current selection.
	/// </summary>
	public interface ISelectionBodyResolver
	{
		/// <summary>
		/// Resolves the selected object to a system body.	
		/// </summary>
		/// <param name="selection">The selected object to resolve.</param>
		/// <returns>The resolved system body, or <see langword="null"/> 
		/// when the selection cannot be resolved to a system body.</returns>
		SystemBody Resolve(ISelectable selection);
	}
}
