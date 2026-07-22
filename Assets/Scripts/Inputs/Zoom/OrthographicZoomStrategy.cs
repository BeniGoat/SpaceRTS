using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    /// <summary>
    /// Implements a zoom strategy for an orthographic camera, allowing for zooming in and out within specified limits.
    /// </summary>
    public class OrthographicZoomStrategy : IZoomStrategy
    {
        /// <inheritdoc/>
        public float NearZoomLimit { get; } = 2f;

        /// <inheritdoc/>
        public float FarZoomLimit { get; } = 16f;

        /// <inheritdoc/>
        public float StartingZoom { get; } = 8f;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthographicZoomStrategy"/> class, setting up the camera's position and zoom level.
        /// </summary>
        /// <param name="cam">The camera to initialize.</param>
        /// <param name="cameraOffsetY">The offset for the camera's Y position.</param>
        /// <param name="cameraOffsetZ">The offset for the camera's Z position.</param>
        public OrthographicZoomStrategy(Camera cam, float cameraOffsetY, float cameraOffsetZ)
        {
            cam.transform.localPosition = new Vector3(0f, cameraOffsetY, cameraOffsetZ);
            cam.orthographicSize = this.StartingZoom;
            cam.farClipPlane = 100f;
            cam.nearClipPlane = -5f;
        }

        /// <inheritdoc/>
        public void ZoomIn(Camera cam, float delta)
        {
            if (cam.orthographicSize == this.NearZoomLimit) { return; }

            cam.orthographicSize = Mathf.Max(cam.orthographicSize - delta, this.NearZoomLimit);
        }

        /// <inheritdoc/>
        public void ZoomOut(Camera cam, float delta)
        {
            if (cam.orthographicSize == this.FarZoomLimit) { return; }

            cam.orthographicSize = Mathf.Min(cam.orthographicSize + delta, this.FarZoomLimit);
        }
    }
}
