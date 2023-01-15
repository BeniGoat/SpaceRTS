using UnityEngine;

namespace SpaceRTS.Camera
{
        public class CameraZoom : MonoBehaviour
        {
                public float minZoom = 1f;
                public float maxZoom = 20f;
                public float zoomSpeed = 5f;

                private void Update()
                {
                        // Zoom in or out based on the mouse wheel scroll
                        float scroll = Input.GetAxis("Mouse ScrollWheel");
                        if (scroll != 0)
                        {
                                float newZoom = this.transform.position.y - scroll * this.zoomSpeed;
                                newZoom = Mathf.Clamp(newZoom, this.minZoom, this.maxZoom);
                                this.transform.position = new Vector3(this.transform.position.x, newZoom, this.transform.position.z);
                        }
                }
        }
}