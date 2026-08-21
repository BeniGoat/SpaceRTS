using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input from the mouse for camera movement, rotation, and zooming. 
	/// It detects mouse position, button presses, and scroll wheel input to trigger corresponding events that can be subscribed to by other components in the game. 
	/// The class ensures that input is only processed when the mouse is within a valid area of the screen and not over any UI elements.
	/// TODO: Move this to a new InputManagers GameObject in the scene (it no longer lives on the GameManager GameObject)
	/// TODO: Consider implementing a more flexible input system that allows for customizable key bindings and supports multiple input devices.
	/// </summary>
	public class MouseInputManager : InputManager
    {
        private Vector2Int screen;
        private Vector3 mousePos;
        private Vector2 mousePosOnRotateStart;

        // Events
        public static event MoveInputHandler OnMoveInput;
        public static event RotateLateralInputHandler OnRotateLateralInput;
        public static event RotateVerticalInputHandler OnRotateVerticalInput;
        public static event ZoomInputHandler OnZoomInput;

        private void Awake()
        {
            this.screen = new Vector2Int(Screen.width, Screen.height);
        }

        private void Update()
        {
            this.mousePos = Input.mousePosition;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Create a 5% buffer around the screen beyond which it won't control the camera
            bool isMouseValid =
                this.mousePos.x <= this.screen.x * 1.05f && this.mousePos.x >= this.screen.x * -0.05f &&
                this.mousePos.y <= this.screen.y * 1.05f && this.mousePos.y >= this.screen.y * -0.05f;

            if (!isMouseValid) { return; }

            // Edge of screen movement
            if (this.mousePos.x > this.screen.x * 0.95f)
            {
                OnMoveInput?.Invoke(Vector3.right);
            }
            else if (this.mousePos.x < this.screen.x * 0.05f)
            {
                OnMoveInput?.Invoke(Vector3.left);
            }

            if (this.mousePos.y > this.screen.y * 0.95f)
            {
                OnMoveInput?.Invoke(Vector3.forward);
            }
            else if (this.mousePos.y < this.screen.y * 0.05f)
            {
                OnMoveInput?.Invoke(Vector3.back);
            }

            // Mouse button rotate
            if (Input.GetMouseButtonDown(2))
            {
                this.mousePosOnRotateStart = new Vector2(this.mousePos.x, this.mousePos.y);
            }
            else if (Input.GetMouseButton(2))
            {
                if (this.mousePos.x < this.mousePosOnRotateStart.x)
                {
                    OnRotateLateralInput?.Invoke(-1f);
                }
                else if (this.mousePos.x > this.mousePosOnRotateStart.x)
                {
                    OnRotateLateralInput?.Invoke(1f);
                }

                if (this.mousePos.y < this.mousePosOnRotateStart.y)
                {
                    OnRotateVerticalInput?.Invoke(1f);
                }
                else if (this.mousePos.y > this.mousePosOnRotateStart.y)
                {
                    OnRotateVerticalInput?.Invoke(-1f);
                }
            }

            // Mouse scroll zoom
            if (Input.mouseScrollDelta.y > 0)
            {
                OnZoomInput?.Invoke(3f);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                OnZoomInput?.Invoke(-3f);
            }
        }
    }
}
