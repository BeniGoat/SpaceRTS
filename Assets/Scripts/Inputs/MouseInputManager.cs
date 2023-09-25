using UnityEngine;

namespace SpaceRTS.Inputs
{
    public class MouseInputManager : InputManager
    {
        private Vector2Int screen;
        private Vector3 mousePos;
        private float mousePosOnRotateStart;

        // Events
        public static event MoveInputHandler OnMoveInput;
        public static event RotateInputManager OnRotateInput;
        public static event ZoomInputManager OnZoomInput;

        private void Awake()
        {
            this.screen = new Vector2Int(Screen.width, Screen.height);
        }

        private void Update()
        {
            this.mousePos = Input.mousePosition;

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
                this.mousePosOnRotateStart = this.mousePos.x;
            }
            else if (Input.GetMouseButton(2))
            {
                if (this.mousePos.x < this.mousePosOnRotateStart)
                {
                    OnRotateInput?.Invoke(-1f);
                }
                else if (this.mousePos.x > this.mousePosOnRotateStart)
                {
                    OnRotateInput?.Invoke(1f);
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
