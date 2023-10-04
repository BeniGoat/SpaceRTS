using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    public class OrthographicZoomStrategy : IZoomStrategy
    {
        public float NearZoomLimit { get; } = 2f;

        public float FarZoomLimit { get; } = 16f;

        public float StartingZoom { get; } = 8f;

        public OrthographicZoomStrategy(Camera cam, float cameraOffsetY, float cameraOffsetZ)
        {
            cam.transform.localPosition = new Vector3(0f, cameraOffsetY, cameraOffsetZ);
            cam.orthographicSize = this.StartingZoom;
            cam.farClipPlane = 100f;
            cam.nearClipPlane = -5f;
        }

        public void ZoomIn(Camera cam, float delta)
        {
            if (cam.orthographicSize == this.NearZoomLimit) { return; }

            cam.orthographicSize = Mathf.Max(cam.orthographicSize - delta, this.NearZoomLimit);
        }

        public void ZoomOut(Camera cam, float delta)
        {
            if (cam.orthographicSize == this.FarZoomLimit) { return; }

            cam.orthographicSize = Mathf.Min(cam.orthographicSize + delta, this.FarZoomLimit);
        }
    }
}
