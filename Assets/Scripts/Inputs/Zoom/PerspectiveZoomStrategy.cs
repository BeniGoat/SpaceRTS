using UnityEngine;

namespace SpaceRTS.Inputs.Zoom
{
    public class PerspectiveZoomStrategy : IZoomStrategy
    {
        private Vector3 normalisedCameraPosition;
        private float currentZoomLevel;
        private readonly bool isCameraInitialised;

        public float NearZoomLimit { get; } = 5f;

        public float FarZoomLimit { get; } = 120f;

        public float StartingZoom { get; } = 50f;

        public PerspectiveZoomStrategy(Camera cam, float cameraOffsetY, float cameraOffsetZ)
        {
            this.normalisedCameraPosition = new Vector3(0f, cameraOffsetY, cameraOffsetZ).normalized;
            this.currentZoomLevel = this.StartingZoom;
            this.PositionCamera(cam);
            this.isCameraInitialised = true;
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
            if (this.isCameraInitialised)
            {
                this.normalisedCameraPosition = new Vector3(0f, cam.transform.localPosition.y, cam.transform.localPosition.z).normalized;
            }

            cam.transform.localPosition = this.normalisedCameraPosition * this.currentZoomLevel;
        }
    }
}
