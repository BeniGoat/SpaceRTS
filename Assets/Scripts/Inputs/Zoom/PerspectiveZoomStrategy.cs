using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    /// <summary>
    /// Implements a zoom strategy for a perspective camera, allowing for zooming in and out within
    /// specified limits while maintaining the camera's position relative to its initial orientation.
    /// </summary>
    /// <seealso cref="BaseZoomStrategy"/>
    public class PerspectiveZoomStrategy : BaseZoomStrategy
    {
        private readonly Vector3 baseDirection;
        private float currentZoomLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveZoomStrategy"/> class.
        /// </summary>
        /// <param name="cam">The camera to initialize.</param>
        /// <param name="cameraOffsetY">The offset for the camera's Y position.</param>
        /// <param name="cameraOffsetZ">The offset for the camera's Z position.</param>
        /// <param name="nearZoomLimit">Minimum allowed zoom distance.</param>
        /// <param name="farZoomLimit">Maximum allowed zoom distance.</param>
        /// <param name="startingZoom">Starting zoom distance.</param>
        public PerspectiveZoomStrategy(
            Camera cam,
            float cameraOffsetY,
            float cameraOffsetZ,
            float nearZoomLimit = 5f,
            float farZoomLimit = 120f,
            float startingZoom = 50f)
            : base(nearZoomLimit, farZoomLimit, startingZoom)
        {
            // Direction from manager -> camera; keep normalized so distance = zoom level
            this.baseDirection = new Vector3(0f, cameraOffsetY, cameraOffsetZ).normalized;
            this.currentZoomLevel = this.StartingZoom;

            // Position camera consistently along the base direction
            this.PositionCamera(cam);

            // Ensure reasonable clip planes
            cam.nearClipPlane = Mathf.Max(0.01f, cam.nearClipPlane);
            cam.farClipPlane = Mathf.Max(cam.farClipPlane, this.FarZoomLimit * 2f);
        }

        /// <inheritdoc/>
        public override void ZoomIn(Camera cam, float delta)
        {
            this.currentZoomLevel = Mathf.Max(this.currentZoomLevel - delta, this.NearZoomLimit);
            this.PositionCamera(cam);
        }

        /// <inheritdoc/>
        public override void ZoomOut(Camera cam, float delta)
        {
            this.currentZoomLevel = Mathf.Min(this.currentZoomLevel + delta, this.FarZoomLimit);
            this.PositionCamera(cam);
        }

        /// <summary>
        /// Positions the camera based on the current zoom level and the normalized camera position.
        /// </summary>
        /// <param name="cam">The camera to position.</param>
        private void PositionCamera(Camera cam)
        {
            cam.transform.localPosition = this.baseDirection * this.currentZoomLevel;
        }
    }
}
