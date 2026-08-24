using SpaceRTS.Managers.Enums;
using UnityEngine;

namespace SpaceRTS.Events
{
	/// <summary>
	/// Raised when a movement input is detected (keyboard WASD or mouse edge scrolling).
	/// Using structs for events to avoid garbage collection overhead.
	/// </summary>
	public struct MoveInputEvent
	{
		public Vector3 Direction;
	}

	/// <summary>
	/// Raised when a lateral rotation input is detected (Q/E keys or middle mouse drag).
	/// </summary>
	public struct RotateLateralInputEvent
	{
		public float Amount;
	}

	/// <summary>
	/// Raised when a vertical rotation input is detected (R/F keys or middle mouse drag).
	/// </summary>
	public struct RotateVerticalInputEvent
	{
		public float Amount;
	}

	/// <summary>
	/// Raised when a zoom input is detected (Z/X keys or mouse scroll).
	/// </summary>
	public struct ZoomInputEvent
	{
		public float Amount;
	}

	/// <summary>
	/// Raised when the user clicks to select an object (left mouse button).
	/// </summary>
	public struct SelectInputEvent
	{
		public Vector3 ScreenPosition;
	}

	/// <summary>
	/// Raised when the user clicks to issue a command (right mouse button).
	/// </summary>
	public struct CommandInputEvent
	{
		public Vector3 ScreenPosition;
	}

	/// <summary>
	/// Raised when the user requests a game speed change.
	/// </summary>
	public struct GameSpeedInputEvent
	{
		public GameSpeed RequestedSpeed;
	}

	/// <summary>
	/// Raised when the user presses the build key.
	/// </summary>
	public struct BuildInputEvent { }
}