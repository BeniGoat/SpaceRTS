using SpaceRTS.Inputs;
using SpaceRTS.Inputs.Zoom;
using SpaceRTS.Managers.Enums;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Offset")]
        public float cameraOffsetY;
        public float cameraOffsetZ;

        [Header("Move Controls")]
        public float moveSpeed = 100f;
        public float rotateSpeed = 100f;

        [Header("Zoom Controls")]
        public float zoomSpeed = 100f;
        public float nearZoomLimit, farZoomLimit, startingZoom;

        [Header("Move Bounds")]
        public Vector2 minBounds, maxBounds;
        public float minVerticalAngle = 10f;
        public float maxVerticalAngle = 80f;

        private IZoomStrategy zoomStrategy;
        private Vector3 frameMove;
        private float frameLateralRotate, frameVerticalRotate, frameZoom;
        private Camera cam;

        [Header("Target")]
        private Transform target;
        private Vector3 movementVelocity = Vector3.zero;

        private void Awake()
        {
            this.cam = this.GetComponentInChildren<Camera>();           
        }

        private void OnEnable()
        {
            KeyboardInputManager.OnMoveInput += this.UpdateFrameMove;
            KeyboardInputManager.OnRotateLateralInput += this.UpdateFrameLateralRotate;
            KeyboardInputManager.OnRotateVerticalInput += this.UpdateFrameVerticalRotate;
            KeyboardInputManager.OnZoomInput += this.UpdateFrameZoom; 
            MouseInputManager.OnMoveInput += this.UpdateFrameMove;
            MouseInputManager.OnRotateLateralInput += this.UpdateFrameLateralRotate;
            MouseInputManager.OnRotateVerticalInput += this.UpdateFrameVerticalRotate;
            MouseInputManager.OnZoomInput += this.UpdateFrameZoom;
        }

        public void SetCamera(CameraMode cameraMode, float range)
        {
            switch (cameraMode)
            {
                case CameraMode.Perspective:
                    this.zoomStrategy = new PerspectiveZoomStrategy(this.cam, this.cameraOffsetY, this.cameraOffsetZ);
                    break;
                case CameraMode.Orthographic:
                    this.zoomStrategy = new OrthographicZoomStrategy(this.cam, this.cameraOffsetY, this.cameraOffsetZ);
                    break;
            }

            this.startingZoom = this.zoomStrategy.StartingZoom;
            this.nearZoomLimit = this.zoomStrategy.NearZoomLimit;
            this.farZoomLimit = this.zoomStrategy.FarZoomLimit;

            this.maxBounds = new Vector2(range, range);
            this.minBounds = new Vector2(-range, -range);

            this.cam.transform.LookAt(this.transform.position);
        }

        private void OnDisable()
        {
            KeyboardInputManager.OnMoveInput -= this.UpdateFrameMove;
            KeyboardInputManager.OnRotateLateralInput -= this.UpdateFrameLateralRotate;
            KeyboardInputManager.OnRotateVerticalInput -= this.UpdateFrameVerticalRotate;
            KeyboardInputManager.OnZoomInput -= this.UpdateFrameZoom;
            MouseInputManager.OnMoveInput -= this.UpdateFrameMove;
            MouseInputManager.OnRotateLateralInput -= this.UpdateFrameLateralRotate;
            MouseInputManager.OnRotateVerticalInput -= this.UpdateFrameVerticalRotate;
            MouseInputManager.OnZoomInput -= this.UpdateFrameZoom;
        }

        private void Update()
        {
            if (this.target != null)
            {
                this.MoveToPosition(this.target.position);
            }
        }

        private void LateUpdate()
        {
            if (this.frameMove != Vector3.zero)
            {
                float speed = this.moveSpeed * this.GetCameraDistanceFactor();
                var modifiedFrameMove = new Vector3(
                    this.frameMove.x * speed,
                    this.frameMove.y,
                    this.frameMove.z * speed);
                this.transform.position += this.transform.TransformDirection(modifiedFrameMove) * Time.unscaledDeltaTime;
                this.LockPositionInBounds();
                this.frameMove = Vector3.zero;

                if (this.target != null)
                {
                    this.SetTarget(null);
                }
            }

            if (this.frameLateralRotate != 0)
            {
                this.transform.RotateAround(this.transform.position, this.transform.up, this.frameLateralRotate * this.rotateSpeed * Time.unscaledDeltaTime);
                this.frameLateralRotate = 0f;
            }

            if (this.frameVerticalRotate != 0)
            {
                float rotationAmount = this.frameVerticalRotate * this.rotateSpeed * Time.unscaledDeltaTime;

                // Get the current vertical angle
                float currentVerticalAngle = this.cam.transform.eulerAngles.x;

                // Calculate the new vertical angle after applying rotationAmount
                float newVerticalAngle = currentVerticalAngle + rotationAmount;

                // Ensure the new angle stays within the specified range
                newVerticalAngle = Mathf.Clamp(newVerticalAngle, this.minVerticalAngle, this.maxVerticalAngle);

                // Calculate the clamped rotationAmount
                float clampedRotationAmount = Mathf.Round((newVerticalAngle - currentVerticalAngle) * 100f) * 0.01f;

                this.cam.transform.RotateAround(this.transform.position, this.transform.right, clampedRotationAmount);
                this.frameVerticalRotate = 0f;                
            }

            if (this.frameZoom > 0f)
            {
                float speed = this.zoomSpeed * this.GetCameraDistanceFactor() * 10f;
                this.zoomStrategy.ZoomIn(this.cam, this.frameZoom * speed * Time.unscaledDeltaTime);
                this.frameZoom = 0f;
            }
            else if (this.frameZoom < 0f)
            {
                float speed = this.zoomSpeed * this.GetCameraDistanceFactor() * 10f;
                this.zoomStrategy.ZoomOut(this.cam, Mathf.Abs(this.frameZoom) * speed * Time.unscaledDeltaTime);
                this.frameZoom = 0f;
            }
        }

        private float GetCameraDistanceFactor()
        {
            Vector3 distance = this.cam.transform.position - this.transform.position;
            return distance.magnitude * 0.005f;
        }

        private void LockPositionInBounds()
        {
            // Clamp the position values to the system map size
            this.transform.position = new Vector3(
                Mathf.Clamp(this.transform.position.x, this.minBounds.x, this.maxBounds.x),
                this.transform.position.y,
                Mathf.Clamp(this.transform.position.z, this.minBounds.y, this.maxBounds.y));
        }

        private void UpdateFrameMove(Vector3 moveVector)
        {
            this.frameMove += moveVector;
        }

        private void UpdateFrameLateralRotate(float rotateAmount)
        {
            this.frameLateralRotate += rotateAmount;
        }

        private void UpdateFrameVerticalRotate(float rotateAmount)
        {
            this.frameVerticalRotate += rotateAmount;
        }

        private void UpdateFrameZoom(float zoomAmount)
        {
            this.frameZoom += zoomAmount;
        }

        private void MoveToPosition(Vector3 newPosition)
        {
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position,
                newPosition,
                ref this.movementVelocity,
                0.1f,
                Mathf.Infinity,
                Time.unscaledDeltaTime);
        }

        public Ray SendRay(Vector3 position)
        {
            return this.cam.ScreenPointToRay(position);
        }

        public void SetTarget(Transform newTarget)
        {
            this.target = newTarget;
        }
    }
}
