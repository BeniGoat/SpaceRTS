using SpaceRTS.Inputs;
using SpaceRTS.Inputs.Zoom;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Offset")]
        public Vector3 cameraOffset;

        [Header("Move Controls")]
        public float moveSpeed = 100f;
        public float rotateSpeed = 100f;

        [Header("Zoom Controls")]
        public float zoomSpeed = 100f;
        public float nearZoomLimit;
        public float farZoomLimit;
        public float startingZoom;

        [Header("Move Bounds")]
        public Vector2 minBounds, maxBounds;

        private IZoomStrategy zoomStrategy;
        private Vector3 frameMove;
        private float frameRotate;
        private float frameZoom;
        private Camera cam;

        private void Awake()
        {
            this.cam = this.GetComponentInChildren<Camera>();

            this.zoomStrategy = this.cam.orthographic
                ? (IZoomStrategy)new OrthographicZoomStrategy(this.cam, this.cameraOffset)
                : new PerspectiveZoomStrategy(this.cam, this.cameraOffset);
            this.startingZoom = this.zoomStrategy.StartingZoom;
            this.nearZoomLimit = this.zoomStrategy.NearZoomLimit;
            this.farZoomLimit = this.zoomStrategy.FarZoomLimit;

            this.cam.transform.LookAt(this.transform.position);
        }

        private void OnEnable()
        {
            KeyboardInputManager.OnMoveInput += this.UpdateFrameMove;
            KeyboardInputManager.OnRotateInput += this.UpdateFrameRotate;
            KeyboardInputManager.OnZoomInput += this.UpdateFrameZoom; 
            MouseInputManager.OnMoveInput += this.UpdateFrameMove;
            MouseInputManager.OnRotateInput += this.UpdateFrameRotate;
            MouseInputManager.OnZoomInput += this.UpdateFrameZoom;
        }

        private void OnDisable()
        {
            KeyboardInputManager.OnMoveInput -= this.UpdateFrameMove;
            KeyboardInputManager.OnRotateInput -= this.UpdateFrameRotate;
            KeyboardInputManager.OnZoomInput -= this.UpdateFrameZoom;
            MouseInputManager.OnMoveInput -= this.UpdateFrameMove;
            MouseInputManager.OnRotateInput -= this.UpdateFrameRotate;
            MouseInputManager.OnZoomInput -= this.UpdateFrameZoom;
        }

        private void LateUpdate()
        {
            if (this.frameMove != Vector3.zero)
            {
                var modifiedFrameMove = new Vector3(
                    this.frameMove.x * this.moveSpeed,
                    this.frameMove.y,
                    this.frameMove.z * this.moveSpeed);
                this.transform.position += this.transform.TransformDirection(modifiedFrameMove) * Time.deltaTime;
                this.LockPositionInBounds();
                this.frameMove = Vector3.zero;
            }

            if (this.frameRotate != 0)
            {
                this.transform.Rotate(Vector3.up, this.frameRotate * this.rotateSpeed * Time.deltaTime);
                this.frameRotate = 0f;
            }

            if (this.frameZoom > 0f)
            {
                this.zoomStrategy.ZoomIn(this.cam, this.frameZoom * this.zoomSpeed * Time.deltaTime);
                this.frameZoom = 0f;
            }
            else if (this.frameZoom < 0f)
            {
                this.zoomStrategy.ZoomOut(this.cam, Mathf.Abs(this.frameZoom) * this.zoomSpeed * Time.deltaTime);
                this.frameZoom = 0f;
            }
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

        private void UpdateFrameRotate(float rotateAmount)
        {
            this.frameRotate += rotateAmount;
        }

        private void UpdateFrameZoom(float zoomAmount)
        {
            this.frameZoom += zoomAmount;
        }
    }
}
