using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    /// <summary>
    /// Implements a zoom strategy for a perspective camera, allowing for zooming in and out within
    /// specified limits while maintaining the camera's position relative to its initial orientation.
    /// </summary>
    public class PerspectiveZoomStrategy : IZoomStrategy
    {
        private Vector3 normalisedCameraPosition;
        private float currentZoomLevel;
        private readonly bool isCameraInitialised;

        /// <inheritdoc/>
        public float NearZoomLimit { get; } = 5f;

        /// <inheritdoc/>
        public float FarZoomLimit { get; } = 120f;

        /// <inheritdoc/>
        public float StartingZoom { get; } = 50f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveZoomStrategy"/> class, setting up the camera's position and zoom level.
        /// </summary>
        /// <param name="cam">The camera to initialize.</param>
        /// <param name="cameraOffsetY">The offset for the camera's Y position.</param>
        /// <param name="cameraOffsetZ">The offset for the camera's Z position.</param>
        public PerspectiveZoomStrategy(Camera cam, float cameraOffsetY, float cameraOffsetZ)
        {
            this.normalisedCameraPosition = new Vector3(0f, cameraOffsetY, cameraOffsetZ).normalized;
            this.currentZoomLevel = this.StartingZoom;
            this.PositionCamera(cam);
            this.isCameraInitialised = true;
        }

        /// <inheritdoc/>
        public void ZoomIn(Camera cam, float delta)
        {
            if (this.currentZoomLevel <= this.NearZoomLimit) { return; }

            this.currentZoomLevel = Mathf.Max(this.currentZoomLevel - delta, this.NearZoomLimit);
            this.PositionCamera(cam);
        }

        /// <inheritdoc/>
        public void ZoomOut(Camera cam, float delta)
        {
            if (this.currentZoomLevel >= this.FarZoomLimit) { return; }

            this.currentZoomLevel = Mathf.Min(this.currentZoomLevel + delta, this.FarZoomLimit);
            this.PositionCamera(cam);
        }

        /// <summary>
        /// Positions the camera based on the current zoom level and the normalized camera position.
        /// </summary>
        /// <param name="cam">The camera to position.</param>
        private void PositionCamera(Camera cam)
        {
            // Update the normalized camera position if the camera has been initialized
            if (this.isCameraInitialised)
            {
                this.normalisedCameraPosition = new Vector3(0f, cam.transform.localPosition.y, cam.transform.localPosition.z).normalized;
            }

            cam.transform.localPosition = this.normalisedCameraPosition * this.currentZoomLevel;
        }
    }
}
