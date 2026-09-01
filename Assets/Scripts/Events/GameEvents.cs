using SpaceRTS.Managers.Enums;
using SpaceRTS.Models.Interfaces;

namespace SpaceRTS.Events
{
	/// <summary>
	/// Raised when the current selection changes. Selection may be null if cleared.
	/// Using structs for events to avoid garbage collection overhead.
	/// </summary>
	public struct SelectionChangedEvent
	{
		public ISelectable Selection;
	}

	/// <summary>
	/// Raised when a ship is successfully built at a body.
	/// </summary>
	public struct ShipBuiltEvent
	{
		public ISelectable ShipSelection;
	}

	/// <summary>
	/// Raised when the effective game speed changes (including pause/unpause).
	/// </summary>
	public struct SpeedChangedEvent
	{
		public GameSpeed Speed;
	}
}