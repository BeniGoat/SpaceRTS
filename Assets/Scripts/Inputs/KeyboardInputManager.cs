using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles keyboard input for camera movement, rotation, and zooming in the game. 
	/// It listens for specific key presses (W, A, S, D for movement; Q, E for lateral rotation; R, F for vertical rotation; Z, X for zooming)
	/// and invokes corresponding events to notify other components of the input actions. 
	/// This class ensures that input is only processed when the pointer is not over a UI element.
	/// TODO: Move this to a new InputManagers GameObject in the scene (it no longer lives on the GameManager GameObject)
	/// TODO: Consider implementing a more flexible input system that allows for customizable key bindings and supports multiple input devices.
	/// </summary>
	public class KeyboardInputManager : InputManager
    {
        // Events
        public static event MoveInputHandler OnMoveInput;
        public static event RotateLateralInputHandler OnRotateLateralInput;
        public static event RotateVerticalInputHandler OnRotateVerticalInput;
        public static event ZoomInputHandler OnZoomInput;

        private void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Check if WASD keys are being pressed
            if (Input.GetKey(KeyCode.W))
            {
                OnMoveInput?.Invoke(Vector3.forward);
            }
            if (Input.GetKey(KeyCode.S))
            {
                OnMoveInput?.Invoke(Vector3.back);
            }
            if (Input.GetKey(KeyCode.D))
            {
                OnMoveInput?.Invoke(Vector3.right);
            }
            if (Input.GetKey(KeyCode.A))
            {
                OnMoveInput?.Invoke(Vector3.left);
            }

            // Rotate the camera left/right using the Q and E keys
            if (Input.GetKey(KeyCode.Q))
            {
                OnRotateLateralInput?.Invoke(1f);
            }
            if (Input.GetKey(KeyCode.E))
            {
                OnRotateLateralInput?.Invoke(-1f);
            }

            // Rotate the camera up/down using the R and F keys
            if (Input.GetKey(KeyCode.R))
            {
                OnRotateVerticalInput?.Invoke(1f);
            }
            if (Input.GetKey(KeyCode.F))
            {
                OnRotateVerticalInput?.Invoke(-1f);
            }

            // Zoom in or out using the Z and X keys
            if (Input.GetKey(KeyCode.Z))
            {
                OnZoomInput?.Invoke(1f);
            }
            if (Input.GetKey(KeyCode.X))
            {
                OnZoomInput?.Invoke(-1f);
            }
        }
    }
}
