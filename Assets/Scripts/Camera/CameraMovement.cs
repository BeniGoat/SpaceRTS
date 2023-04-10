using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform cameraTransform;
        private SelectableObject target;

        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private float rotateSpeed = 0.5f;
        [SerializeField] private float zoomSpeed = 2f;

        [SerializeField] private float smoothTime = 0.1f;

        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 120f;

        [SerializeField] private float distance;

        private bool isUserMovingCamera;
        private bool areSelectableObjectsRetrieved;

        private float zOffset;
        private Vector3 newPosition;
        private Quaternion newRotation;
        private Vector3 newZoom;
        private Vector3 zoomAmount;
        private Vector3 movementVelocity = Vector3.zero;
        private float rotationVelocity = 0f;
        private Vector3 zoomVelocity = Vector3.zero;

        public float Range { get; set; }

        private void Start()
        {
            this.zOffset = this.transform.position.z;
            this.newPosition = this.transform.position;
            this.newRotation = this.transform.rotation;
            this.newZoom = this.cameraTransform.localPosition;
            this.zoomAmount = new Vector3(0, -this.zoomSpeed, this.zoomSpeed);
        }

        private void Update()
        {
            if (!this.areSelectableObjectsRetrieved)
            {
                this.GetSelectableObjects();
            }

            this.distance = this.cameraTransform.localPosition.magnitude;

            this.HandleInput();

            if (this.target != null)
            {
                this.target.onDeselected.Invoke();
            }

            // Check if the camera should be view-locked to a selected object
            if (!this.isUserMovingCamera && this.target != null && this.target.IsSelected)
            {
                this.ViewSelectedObject();
            }
        }

        private void HandleInput()
        {
            this.HandleMovementInput();
            this.HandleRotationInput();
            this.HandleZoomInput();
        }

        private void HandleMovementInput()
        {
            // Check if WASD keys are being pressed
            if (Input.GetKey(KeyCode.W))
            {
                this.newPosition += this.transform.forward * this.moveSpeed;
                this.isUserMovingCamera = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.newPosition -= this.transform.forward * this.moveSpeed;
                this.isUserMovingCamera = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.newPosition += this.transform.right * this.moveSpeed;
                this.isUserMovingCamera = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.newPosition -= this.transform.right * this.moveSpeed;
                this.isUserMovingCamera = true;
            }

            this.moveSpeed = this.distance * 0.005f;

            // Clamp the new position values to the system map size
            this.newPosition.x = Mathf.Clamp(this.newPosition.x, -this.Range, this.Range);
            this.newPosition.z = Mathf.Clamp(this.newPosition.z, -this.Range + this.zOffset, this.Range + this.zOffset);

            this.transform.position = Vector3.SmoothDamp(this.transform.position, this.newPosition, ref this.movementVelocity, this.smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        }

        private void HandleRotationInput()
        {
            // Rotate the camera left/right using the Q and E keys
            if (Input.GetKey(KeyCode.E))
            {
                this.newRotation *= Quaternion.Euler(Vector3.down * this.rotateSpeed);
                this.isUserMovingCamera = true;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                this.newRotation *= Quaternion.Euler(Vector3.up * this.rotateSpeed);
                this.isUserMovingCamera = true;
            }

            //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.newRotation, Time.unscaledDeltaTime * this.moveTime);
            this.transform.rotation = SmoothDampQuaternion(this.transform.rotation, this.newRotation, ref this.rotationVelocity, Mathf.Infinity, this.smoothTime, Time.unscaledDeltaTime);
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
                this.zoomSpeed = this.distance * 0.05f;

                this.zoomAmount = new Vector3(0, -this.zoomSpeed, this.zoomSpeed);
                this.newZoom += this.zoomAmount * Mathf.Sign(scrollValue);
                this.newZoom.y = Mathf.Clamp(this.newZoom.y, this.minZoom, this.maxZoom);
                this.newZoom.z = Mathf.Clamp(this.newZoom.z, -this.maxZoom, -this.minZoom);

                this.isUserMovingCamera = true;
            }

            this.cameraTransform.localPosition = Vector3.SmoothDamp(this.cameraTransform.localPosition, this.newZoom, ref this.zoomVelocity, this.smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        }

        private void ViewSelectedObject()
        {

        }

        private void GetSelectableObjects()
        {
            SelectableObject[] selectableObjects = FindObjectsOfType<SelectableObject>();
            foreach (SelectableObject selectableObject in selectableObjects)
            {
                selectableObject.onSelected.AddListener(() => { this.OnTargetSelected(selectableObject); });
                selectableObject.onDeselected.AddListener(() => { this.OnTargetDeselected(selectableObject); });
            }

            this.areSelectableObjectsRetrieved = true;
        }

        private void OnTargetSelected(SelectableObject target)
        {
            // set the camera target to the clicked object
            this.target = target;
        }

        private void OnTargetDeselected(SelectableObject target)
        {
            // clear the selected camera target
            this.target = null;
        }
    }
}
