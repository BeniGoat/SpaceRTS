using UnityEngine;

namespace SpaceRTS.Inputs
{
    public class KeyboardInputManager : InputManager
    {
        // Events
        public static event MoveInputHandler OnMoveInput;
        public static event RotateInputManager OnRotateInput;
        public static event ZoomInputManager OnZoomInput;

        private void Update()
        {
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
                OnRotateInput?.Invoke(1f);
            }
            if (Input.GetKey(KeyCode.E))
            {
                OnRotateInput?.Invoke(-1f);
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
