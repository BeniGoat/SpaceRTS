using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    public interface IZoomStrategy
    {
        float NearZoomLimit { get; }
        float FarZoomLimit { get; }
        float StartingZoom { get; }

        void ZoomIn(Camera cam, float delta);
        void ZoomOut(Camera cam, float delta);
    }
}
