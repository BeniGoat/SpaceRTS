using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    public class PerspectiveZoomStrategy : IZoomStrategy
    {
        private Vector3 normalisedCameraPosition;
        private float currentZoomLevel;

        public float NearZoomLimit { get; } = 5f;

        public float FarZoomLimit { get; } = 120f;

        public float StartingZoom { get; } = 50f;

        public PerspectiveZoomStrategy(Camera cam, Vector3 offset)
        {
            this.normalisedCameraPosition = new Vector3(0f, offset.y, offset.z).normalized;
            this.currentZoomLevel = this.StartingZoom;
            this.PositionCamera(cam);
        }

        public void ZoomIn(Camera cam, float delta)
        {
            if (this.currentZoomLevel <= this.NearZoomLimit) { return; }

            this.currentZoomLevel = Mathf.Max(this.currentZoomLevel - delta, this.NearZoomLimit);
            this.PositionCamera(cam);
        }

        public void ZoomOut(Camera cam, float delta)
        {
            if (this.currentZoomLevel >= this.FarZoomLimit) { return; }

            this.currentZoomLevel = Mathf.Min(this.currentZoomLevel + delta, this.FarZoomLimit);
            this.PositionCamera(cam);
        }

        private void PositionCamera(Camera cam)
        {
            cam.transform.localPosition = this.normalisedCameraPosition * this.currentZoomLevel;
        }
    }
}
