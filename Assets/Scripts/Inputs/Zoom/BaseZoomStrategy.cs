using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    /// <summary>
    /// Provides a base implementation for zoom strategies, encapsulating common properties
    /// and methods for managing zoom limits and starting zoom levels.
    /// </summary>
    public abstract class BaseZoomStrategy : IZoomStrategy
    {
        /// <summary>
        /// Gets the near zoom limit, which defines how close the camera can zoom in.
        /// </summary>
        public float NearZoomLimit { get; }

        /// <summary>
        /// Gets the far zoom limit, which defines how far the camera can zoom out.
        /// </summary>
        public float FarZoomLimit { get; }

        /// <summary>
        /// Gets the starting zoom level, which represents the initial zoom state of the camera.
        /// </summary>
        public float StartingZoom { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseZoomStrategy"/> class with specified zoom limits and starting zoom level.
        /// </summary>
        /// <param name="nearZoomLimit">Minimum allowed zoom distance.</param>
        /// <param name="farZoomLimit">Maximum allowed zoom distance.</param>
        /// <param name="startingZoom">Starting zoom distance.</param>
        protected BaseZoomStrategy(float nearZoomLimit, float farZoomLimit, float startingZoom)
        {
            this.NearZoomLimit = nearZoomLimit;
            this.FarZoomLimit = farZoomLimit;
            this.StartingZoom = Mathf.Clamp(startingZoom, nearZoomLimit, farZoomLimit);
        }

        /// <summary>
        /// Clamps the given zoom value between the near and far zoom limits.
        /// </summary>
        /// <param name="value">The zoom value to clamp.</param>
        /// <returns>The clamped zoom value.</returns>
        protected float ClampZoom(float value) => Mathf.Clamp(value, this.NearZoomLimit, this.FarZoomLimit);

        /// <summary>
        /// Zooms the camera in by a specified delta, adjusting the camera's zoom level closer to the near zoom limit.
        /// </summary>
        /// <param name="cam">The camera to zoom.</param>
        /// <param name="delta">The amount to zoom in.</param>
        public abstract void ZoomIn(Camera cam, float delta);

        /// <summary>
        /// Zooms the camera out by a specified delta, adjusting the camera's zoom level further from the near zoom limit.
        /// </summary>
        /// <param name="cam">The camera to zoom.</param>
        /// <param name="delta">The amount to zoom out.</param>
        public abstract void ZoomOut(Camera cam, float delta);
    }
}