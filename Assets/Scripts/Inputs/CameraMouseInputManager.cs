using SpaceRTS.Events;
using SpaceRTS.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Converts mouse interactions into camera control events: edge-of-screen movement, middle‑mouse rotation, and
	/// mouse‑wheel zoom, and publishes corresponding events on the EventBus.
	/// </summary>
	/// <remarks>Ignores input when the pointer is over UI. Initializes screen dimensions in Awake and applies a 5%
	/// outer buffer to ignore out-of-range cursor positions. Edge movement publishes discrete MoveInputEvent directions
	/// when the cursor crosses 5% edge thresholds. Middle‑mouse dragging records the start position and publishes
	/// RotateLateralInputEvent and RotateVerticalInputEvent with ±1 amounts based on relative mouse movement. Mouse wheel
	/// scrolling publishes ZoomInputEvent with ±3 amounts. The class does not perform camera transforms itself; it only
	/// emits high-level input events.</remarks>
	public class CameraMouseInputManager : MonoBehaviour
    {
        private Vector2Int screen;
        private Vector3 mousePos;
        private Vector2 mousePosOnRotateStart;

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
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.right });
			else if (this.mousePos.x < this.screen.x * 0.05f)
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.left });

			if (this.mousePos.y > this.screen.y * 0.95f)
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.forward });
			else if (this.mousePos.y < this.screen.y * 0.05f)
				EventBus.Publish(new MoveInputEvent { Direction = Vector3.back });

			// Mouse button rotate
			if (Input.GetMouseButtonDown(2))
			{
				this.mousePosOnRotateStart = new Vector2(this.mousePos.x, this.mousePos.y);
			}
			else if (Input.GetMouseButton(2))
			{
				if (this.mousePos.x < this.mousePosOnRotateStart.x)
					EventBus.Publish(new RotateLateralInputEvent { Amount = -1f });
				else if (this.mousePos.x > this.mousePosOnRotateStart.x)
					EventBus.Publish(new RotateLateralInputEvent { Amount = 1f });

				if (this.mousePos.y < this.mousePosOnRotateStart.y)
					EventBus.Publish(new RotateVerticalInputEvent { Amount = 1f });
				else if (this.mousePos.y > this.mousePosOnRotateStart.y)
					EventBus.Publish(new RotateVerticalInputEvent { Amount = -1f });
			}

			// Mouse scroll zoom
			if (Input.mouseScrollDelta.y > 0)
				EventBus.Publish(new ZoomInputEvent { Amount = 3f });
			else if (Input.mouseScrollDelta.y < 0)
				EventBus.Publish(new ZoomInputEvent { Amount = -3f });
		}
    }
}
