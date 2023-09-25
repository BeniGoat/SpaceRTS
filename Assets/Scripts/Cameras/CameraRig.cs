using UnityEngine;

namespace SpaceRTS.Cameras
{
    public class CameraRig : MonoBehaviour
    {
        private Camera mainCamera;
        private Transform target;

        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private float rotateSpeed = 0.5f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 120f;

        private float zOffset;
        private Vector3 newPosition;
        private Quaternion newRotation;
        private Vector3 newZoom;
        private Vector3 zoomAmount;
        private Vector3 movementVelocity = Vector3.zero;
        private float rotationVelocity = 0f;
        private Vector3 zoomVelocity = Vector3.zero;
        private float currentDistance;

        public float Range { get; set; }

        private void Awake()
        {
            this.mainCamera = Camera.main;
        }

        private void Start()
        {
            this.zOffset = this.transform.position.z;
            this.newPosition = this.transform.position;
            this.newRotation = this.transform.rotation;
            this.newZoom = this.mainCamera.transform.localPosition;
            this.zoomAmount = new Vector3(0, -this.zoomSpeed, this.zoomSpeed);
        }

        private void Update()
        {
            this.currentDistance = this.mainCamera.transform.localPosition.magnitude;
            this.HandleInput();

            if (this.target != null)
            {
                this.FollowTarget();
            }
        }

        private void FollowTarget()
        {
            //TODO: get camera to smoothly move to target position before follow
            this.newPosition = this.target.position;
            this.MoveToPosition(this.newPosition);
        }

        public Ray SendRay(Vector3 position)
        {
            return this.mainCamera.ScreenPointToRay(position);
        }

        public void SetTarget(Transform newTarget)
        {
            this.target = newTarget;
        }

        private void HandleInput()
        {
            this.HandleMovementInput();
            this.HandleRotationInput();
            this.HandleZoomInput();
        }

        private void HandleMovementInput()
        {
            bool isPlayerMovingCamera = false;

            // Check if WASD keys are being pressed
            if (Input.GetKey(KeyCode.W))
            {
                this.newPosition += this.transform.forward * this.moveSpeed;
                isPlayerMovingCamera = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.newPosition -= this.transform.forward * this.moveSpeed;
                isPlayerMovingCamera = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.newPosition += this.transform.right * this.moveSpeed;
                isPlayerMovingCamera = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.newPosition -= this.transform.right * this.moveSpeed;
                isPlayerMovingCamera = true;
            }

            if (isPlayerMovingCamera)
            {
                this.SetTarget(null);
            }

            this.moveSpeed = this.currentDistance * 0.005f;

            // Clamp the new position values to the system map size
            this.newPosition.x = Mathf.Clamp(this.newPosition.x, -this.Range, this.Range);
            this.newPosition.z = Mathf.Clamp(this.newPosition.z, -this.Range + this.zOffset, this.Range + this.zOffset);

            this.MoveToPosition(this.newPosition);
        }

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

        private void HandleRotationInput()
        {
            // Rotate the camera left/right using the Q and E keys
            if (Input.GetKey(KeyCode.E))
            {
                this.newRotation *= Quaternion.Euler(Vector3.down * this.rotateSpeed);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                this.newRotation *= Quaternion.Euler(Vector3.up * this.rotateSpeed);
            }

            this.transform.rotation = SmoothDampQuaternion(
                this.transform.rotation, 
                this.newRotation,
                ref this.rotationVelocity, 
                Mathf.Infinity,
                this.smoothTime,
                Time.unscaledDeltaTime);
        }

        private static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float currentVelocity, float maxSpeed, float smoothTime, float deltaTime)
        {
            float delta = Quaternion.Angle(current, target);
            if (delta != 0f)
            {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
                t = 1.0f - (t / delta);
                return Quaternion.Slerp(current, target, t);
            }

            return current;
        }

        private void HandleZoomInput()
        {
            // Zoom in or out based on the mouse wheel scroll
            float scrollValue = Mathf.Round(Input.GetAxis("Mouse ScrollWheel") * 100f) * 0.01f;

            if (scrollValue != 0)
            {
                this.zoomSpeed = this.currentDistance * 0.05f;

                this.zoomAmount = new Vector3(0, -this.zoomSpeed, this.zoomSpeed);
                this.newZoom += this.zoomAmount * Mathf.Sign(scrollValue);
                this.newZoom.y = Mathf.Clamp(this.newZoom.y, this.minZoom, this.maxZoom);
                this.newZoom.z = Mathf.Clamp(this.newZoom.z, -this.maxZoom, -this.minZoom);
            }

            this.mainCamera.transform.localPosition = Vector3.SmoothDamp(
                this.mainCamera.transform.localPosition, 
                this.newZoom, 
                ref this.zoomVelocity, 
                this.smoothTime,
                Mathf.Infinity,
                Time.unscaledDeltaTime);
        }
    }
}
