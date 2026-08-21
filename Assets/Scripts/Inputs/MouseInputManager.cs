using SpaceRTS.Events;
using SpaceRTS.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceRTS.Inputs
{
	/// <summary>
	/// Handles user input from the mouse for camera movement, rotation, and zooming. 
	/// It detects mouse position, button presses, and scroll wheel input to publish corresponding events
	/// via the EventBus that can be consumed by other components in the game. 
	/// TODO: Move this to a new InputManagers GameObject in the scene (it no longer lives on the GameManager GameObject)
	/// </summary>
	public class MouseInputManager : MonoBehaviour
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
