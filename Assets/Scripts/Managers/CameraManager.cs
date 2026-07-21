using SpaceRTS.Inputs;
using SpaceRTS.Inputs.Zoom;
using SpaceRTS.Managers.Enums;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Camera cam;
		[SerializeField] private float cameraOffsetY;
		[SerializeField] private float cameraOffsetZ;

        [Header("Move Controls")]
		[SerializeField] private float moveSpeed = 100f;
		[SerializeField] private float rotateSpeed = 100f;
		[SerializeField] private float smoothTime = 0.1f;

		[Header("Zoom Controls")]
		[SerializeField] private float zoomSpeed = 100f;

        [Header("Move Bounds")]
		[SerializeField] private Vector2 minBounds;
		[SerializeField] private Vector2 maxBounds;
		[SerializeField] private float minVerticalAngle = 10f;
		[SerializeField] private float maxVerticalAngle = 80f;
		

		private const float DistanceScaleFactor = 0.005f;
		private const float ZoomMultiplier = 10f;

		private IZoomStrategy zoomStrategy;
        private Vector3 frameMove;
        private float frameLateralRotate, frameVerticalRotate, frameZoom;
        private Transform target;
        private Vector3 movementVelocity = Vector3.zero;

        private void Awake()
        {
            this.ResolveCamera();
        }

		/// <summary>
		/// Resolves the camera reference. If the camera is not assigned, it attempts to
		/// find a Camera component in the children of this GameObject or the main camera in the scene.
		/// If no camera is found, it creates a new camera GameObject and attaches a Camera component to it.
		/// </summary>
        private void ResolveCamera()
        {
			// Attempt to find the camera in the children of this GameObject
            if (this.cam == null)
            {
                this.cam = this.GetComponentInChildren<Camera>(true);
            }

			// If no camera is found, try to find the main camera in the scene
            if (this.cam == null)
            {
                this.cam = Camera.main;
            }

			// If still no camera is found, create a new camera GameObject and attach a Camera component to it
            if (this.cam == null)
            {
                GameObject cameraObject = new("MainCamera");
                cameraObject.transform.SetParent(this.transform, false);
                this.cam = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

			// Ensure the camera is a child of this GameObject and reset its local position and rotation
            if (this.cam.transform.parent != this.transform)
            {
                this.cam.transform.SetParent(this.transform, false);
                this.cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        private void OnEnable()
        {
			// Subscribe to input events
			KeyboardInputManager.OnMoveInput += this.UpdateFrameMove;
            KeyboardInputManager.OnRotateLateralInput += this.UpdateFrameLateralRotate;
            KeyboardInputManager.OnRotateVerticalInput += this.UpdateFrameVerticalRotate;
            KeyboardInputManager.OnZoomInput += this.UpdateFrameZoom; 
            MouseInputManager.OnMoveInput += this.UpdateFrameMove;
            MouseInputManager.OnRotateLateralInput += this.UpdateFrameLateralRotate;
            MouseInputManager.OnRotateVerticalInput += this.UpdateFrameVerticalRotate;
            MouseInputManager.OnZoomInput += this.UpdateFrameZoom;
        }

        private void OnDisable()
        {
			// Unsubscribe from input events
			KeyboardInputManager.OnMoveInput -= this.UpdateFrameMove;
            KeyboardInputManager.OnRotateLateralInput -= this.UpdateFrameLateralRotate;
            KeyboardInputManager.OnRotateVerticalInput -= this.UpdateFrameVerticalRotate;
            KeyboardInputManager.OnZoomInput -= this.UpdateFrameZoom;
            MouseInputManager.OnMoveInput -= this.UpdateFrameMove;
            MouseInputManager.OnRotateLateralInput -= this.UpdateFrameLateralRotate;
            MouseInputManager.OnRotateVerticalInput -= this.UpdateFrameVerticalRotate;
            MouseInputManager.OnZoomInput -= this.UpdateFrameZoom;
        }

		/// <summary>
		/// Sets the camera mode (Perspective or Orthographic) and the movement bounds based on the provided range.
		/// </summary>
		/// <param name="cameraMode">The desired camera mode.</param>
		/// <param name="range">The range for the camera movement bounds.</param>
		public void SetCamera(CameraMode cameraMode, float range)
        {
			this.ResolveCamera();

			this.zoomStrategy = cameraMode switch
			{
				CameraMode.Perspective => new PerspectiveZoomStrategy(this.cam, this.cameraOffsetY, this.cameraOffsetZ),
				CameraMode.Orthographic => new OrthographicZoomStrategy(this.cam, this.cameraOffsetY, this.cameraOffsetZ),
				_ => this.zoomStrategy
			};
			
			this.maxBounds = new Vector2(range, range);
			this.minBounds = new Vector2(-range, -range);
			this.cam.transform.LookAt(this.transform.position);
        }

		/// <summary>
		/// Sets the target Transform for the camera to follow. If a target is set,
        /// the camera will smoothly move towards the target's position each frame.
		/// </summary>
		/// <param name="newTarget">The Transform of the new target for the camera to follow.</param>
		public void SetTarget(Transform newTarget)
		{
			this.target = newTarget;
		}

		/// <summary>
		/// Sends a ray from the camera through the specified screen position. 
        /// This can be used for raycasting to detect objects in the scene based on user input (e.g., mouse clicks).
		/// </summary>
		/// <param name="position">The screen position from which to send the ray.</param>
		/// <returns>A Ray starting from the camera and passing through the specified screen position.</returns>
		public Ray SendRay(Vector3 position) => this.cam.ScreenPointToRay(position);

		/// <summary>
		/// Updates the camera's position, rotation, and zoom based on the accumulated frame inputs.
        /// This method is called once per frame after all Update methods have been called.
		/// </summary>
		private void LateUpdate()
        {
			this.HandleTargetFollow();
			this.HandleMovement();
			this.HandleLateralRotation();
			this.HandleVerticalRotation();
			this.HandleZoom();
		}

		/// <summary>
		/// Handles the camera's movement towards the target if a target is set.
		/// It smoothly moves the camera to the target's position using a damping function.
		private void HandleTargetFollow()
		{
			if (this.target != null)
			{
				this.MoveToPosition(this.target.position);
			}
		}

		/// <summary>
		/// Handles the camera's movement based on the accumulated frameMove vector.
        /// It scales the movement by the moveSpeed and the camera's distance factor,
        /// applies the movement in the camera's local space, and ensures the camera stays within defined bounds.
		/// </summary>
		private void HandleMovement()
		{
			if (this.frameMove == Vector3.zero) return;

			float speed = this.moveSpeed * this.GetCameraDistanceFactor();
			Vector3 scaledMove = new Vector3(
				this.frameMove.x * speed,
				this.frameMove.y,
				this.frameMove.z * speed);

			this.transform.position += this.transform.TransformDirection(scaledMove) * Time.unscaledDeltaTime;
			this.LockPositionInBounds();
			this.frameMove = Vector3.zero;

			if (this.target != null)
			{
				this.SetTarget(null);
			}
		}

		/// <summary>
		/// Handles the camera's lateral rotation based on the accumulated frameLateralRotate value.
		/// It rotates the camera around its up axis by the specified amount, scaled by rotateSpeed and deltaTime.
		/// </summary>
		private void HandleLateralRotation()
		{
			if (this.frameLateralRotate == 0f) return;

			this.transform.RotateAround(
				this.transform.position,
				this.transform.up,
				this.frameLateralRotate * this.rotateSpeed * Time.unscaledDeltaTime);
			this.frameLateralRotate = 0f;
		}

		/// <summary>
		/// Handles the camera's vertical rotation based on the accumulated frameVerticalRotate value.
		/// It calculates the new angle, clamps it within the defined min and max vertical angles,
		/// and rotates the camera around its right axis by the clamped amount.
		/// </summary>
		private void HandleVerticalRotation()
		{
			if (this.frameVerticalRotate == 0f) return;

			float rotationAmount = this.frameVerticalRotate * this.rotateSpeed * Time.unscaledDeltaTime;
			float currentAngle = this.cam.transform.eulerAngles.x;
			float newAngle = Mathf.Clamp(currentAngle + rotationAmount, this.minVerticalAngle, this.maxVerticalAngle);
			float clampedAmount = newAngle - currentAngle;

			this.cam.transform.RotateAround(this.transform.position, this.transform.right, clampedAmount);
			this.frameVerticalRotate = 0f;
		}

		/// <summary>
		/// Handles the camera's zoom based on the accumulated frameZoom value.
		/// It calculates the zoom delta, applies it using the current zoom strategy (Perspective or Orthographic),
		/// and resets the frameZoom value for the next frame.
		/// </summary>
		private void HandleZoom()
		{
			if (this.frameZoom == 0f || this.zoomStrategy == null) return;

			float speed = this.zoomSpeed * this.GetCameraDistanceFactor() * ZoomMultiplier;
			float delta = Mathf.Abs(this.frameZoom) * speed * Time.unscaledDeltaTime;

			if (this.frameZoom > 0f)
				this.zoomStrategy.ZoomIn(this.cam, delta);
			else
				this.zoomStrategy.ZoomOut(this.cam, delta);

			this.frameZoom = 0f;
		}

		/// <summary>
		/// Calculates a scaling factor for camera movement and zoom based on the distance between the camera and its target.
		/// This factor is used to ensure consistent movement speed and zoom behavior regardless of the camera's distance from the target.
		/// </summary>
		/// <returns>The scaling factor based on the camera's distance from its target.</returns>
		private float GetCameraDistanceFactor() => (this.cam.transform.position - this.transform.position).magnitude * DistanceScaleFactor;

		/// <summary>
		/// Locks the camera's position within the defined min and max bounds. It clamps the camera's x and z
		/// coordinates to ensure it stays within the specified area, while keeping the y coordinate unchanged.
		/// </summary>
		private void LockPositionInBounds()
        {
            this.transform.position = new Vector3(
                Mathf.Clamp(this.transform.position.x, this.minBounds.x, this.maxBounds.x),
                this.transform.position.y,
                Mathf.Clamp(this.transform.position.z, this.minBounds.y, this.maxBounds.y));
        }

		/// <summary>
		/// Moves the camera smoothly to the specified new position using a damping function.
		/// This creates a smooth transition effect when the camera moves to a new location.
		/// </summary>
		/// <param name="newPosition">The target position to move the camera to.</param>
		private void MoveToPosition(Vector3 newPosition)
        {
			this.transform.position = Vector3.SmoothDamp(
				this.transform.position,
				newPosition,
				ref this.movementVelocity,
				this.smoothTime,
				Mathf.Infinity,
				Time.unscaledDeltaTime);
		}

		/// <summary>
		/// Updates the frameMove vector based on user input. This method is called when movement input is received,
		/// and it accumulates the movement vector for processing in the LateUpdate method.
		/// </summary>
		/// <param name="moveVector">The movement vector to add to the frameMove.</param>
		private void UpdateFrameMove(Vector3 moveVector) => this.frameMove += moveVector;

		/// <summary>
		/// Updates the frameLateralRotate value based on user input. This method is called when lateral rotation input is received,
		/// and it accumulates the rotation amount for processing in the LateUpdate method.
		/// </summary>
		/// <param name="amount">The rotation amount to add to the frameLateralRotate.</param>
		private void UpdateFrameLateralRotate(float amount) => this.frameLateralRotate += amount;

		/// <summary>
		/// Updates the frameVerticalRotate value based on user input. This method is called when vertical rotation input is received,
		/// and it accumulates the rotation amount for processing in the LateUpdate method.
		/// </summary>
		/// <param name="amount">The rotation amount to add to the frameVerticalRotate.</param>
		private void UpdateFrameVerticalRotate(float amount) => this.frameVerticalRotate += amount;

		/// <summary>
		/// Updates the frameZoom value based on user input. This method is called when zoom input is received,
		/// and it accumulates the zoom amount for processing in the LateUpdate method.
		/// </summary>
		/// <param name="amount">The zoom amount to add to the frameZoom.</param>
		private void UpdateFrameZoom(float amount) => this.frameZoom += amount;
	}
}