using UnityEngine;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Abstract base class for managing user input in the game. It defines delegates for handling various types of input, such as movement, rotation, and zooming. 
    /// Derived classes should implement specific input handling logic based on the game's requirements.
	/// </summary>
	public abstract class InputManager : MonoBehaviour
    {
        public delegate void MoveInputHandler(Vector3 moveVector);
        public delegate void RotateLateralInputHandler(float rotateAmount);
        public delegate void RotateVerticalInputHandler(float rotateAmount);
        public delegate void ZoomInputHandler(float zoomAmount);
    }
}