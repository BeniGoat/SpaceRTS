using SpaceRTS.Events;
using SpaceRTS.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles keyboard input for camera movement, rotation, and zooming in the game. 
	/// It listens for specific key presses (W, A, S, D for movement; Q, E for lateral rotation; R, F for vertical rotation; Z, X for zooming)
	/// and publishes corresponding events via the EventBus to notify other components of the input actions. 
	/// This class ensures that input is only processed when the pointer is not over a UI element.
	/// </summary>
	public class CameraKeyboardInputManager : MonoBehaviour
    {
        private void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

			// Check if WASD keys are being pressed
			if (Input.GetKey(KeyCode.W))
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.forward });
			if (Input.GetKey(KeyCode.S))
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.back });
			if (Input.GetKey(KeyCode.D))
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.right });
			if (Input.GetKey(KeyCode.A))
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.left });

			// Rotate the camera left/right using the Q and E keys
			if (Input.GetKey(KeyCode.Q))
				EventBus.Publish(new RotateLateralInputEvent { Amount = 1f });
			if (Input.GetKey(KeyCode.E))
				EventBus.Publish(new RotateLateralInputEvent { Amount = -1f });

			// Rotate the camera up/down using the R and F keys
			if (Input.GetKey(KeyCode.R))
				EventBus.Publish(new RotateVerticalInputEvent { Amount = 1f });
			if (Input.GetKey(KeyCode.F))
				EventBus.Publish(new RotateVerticalInputEvent { Amount = -1f });

			// Zoom in or out using the Z and X keys
			if (Input.GetKey(KeyCode.Z))
				EventBus.Publish(new ZoomInputEvent { Amount = 1f });
			if (Input.GetKey(KeyCode.X))
				EventBus.Publish(new ZoomInputEvent { Amount = -1f });
		}
    }
}
