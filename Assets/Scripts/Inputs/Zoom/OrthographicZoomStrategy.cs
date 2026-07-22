using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    /// <summary>
    /// Implements a zoom strategy for an orthographic camera, allowing for zooming in and out within specified limits.
    /// </summary>
    /// <seealso cref="BaseZoomStrategy"/>
    public class OrthographicZoomStrategy : BaseZoomStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthographicZoomStrategy"/> class, setting up the camera's position and zoom level.
        /// </summary>
        /// <param name="cam">The camera to initialize.</param>
        /// <param name="cameraOffsetY">The offset for the camera's Y position.</param>
        /// <param name="cameraOffsetZ">The offset for the camera's Z position.</param>
        /// <param name="nearZoomLimit">Minimum orthographic size.</param>
        /// <param name="farZoomLimit">Maximum orthographic size.</param>
        /// <param name="startingZoom">Starting orthographic size.</param>
        public OrthographicZoomStrategy(
            Camera cam,
            float cameraOffsetY,
            float cameraOffsetZ,
            float nearZoomLimit = 2f,
            float farZoomLimit = 16f,
            float startingZoom = 8f)
            : base(nearZoomLimit, farZoomLimit, startingZoom)
        {
            cam.transform.localPosition = new Vector3(0f, cameraOffsetY, cameraOffsetZ);
            cam.orthographicSize = this.StartingZoom;
            cam.farClipPlane = Mathf.Max(100f, cam.farClipPlane);
            cam.nearClipPlane = Mathf.Min(cam.nearClipPlane, -5f);
        }

        /// <inheritdoc/>
        public override void ZoomIn(Camera cam, float delta)
        {
            cam.orthographicSize = Mathf.Max(cam.orthographicSize - delta, this.NearZoomLimit);
        }

        /// <inheritdoc/>
        public override void ZoomOut(Camera cam, float delta)
        {
            cam.orthographicSize = Mathf.Min(cam.orthographicSize + delta, this.FarZoomLimit);
        }
    }
}
