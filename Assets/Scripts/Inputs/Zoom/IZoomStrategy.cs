using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    /// <summary>
    /// Defines the interface for zoom strategies, specifying the methods and properties
    /// required for implementing different zoom behaviors in a camera system.
    /// </summary>
    public interface IZoomStrategy
    {
        /// <summary>
        /// Gets the near zoom limit, which defines how close the camera can zoom in.
        /// </summary>
        float NearZoomLimit { get; }

        /// <summary>
        /// Gets the far zoom limit, which defines how far the camera can zoom out.
        /// </summary>
        float FarZoomLimit { get; }

        /// <summary>
        /// Gets the starting zoom level, which represents the initial zoom state of the camera.
        /// </summary>
        float StartingZoom { get; }

        /// <summary>
        /// Zooms the camera in by a specified delta, adjusting the camera's zoom level closer to the near zoom limit.
        /// </summary>
        /// <param name="cam">The camera to zoom in.</param>
        /// <param name="delta">The amount to zoom in.</param>
        void ZoomIn(Camera cam, float delta);

        /// <summary>
        /// Zooms the camera out by a specified delta, adjusting the camera's zoom level further from the near zoom limit.
        /// </summary>
        /// <param name="cam">The camera to zoom out.</param>
        /// <param name="delta">The amount to zoom out.</param>
        void ZoomOut(Camera cam, float delta);
    }
}
