using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        //public CameraRig CameraRig;
        public CameraManager CameraManager;
        private SelectableObject selectedObject; // the currently selected object

        private void Update()
        {
            this.HandleMouseClick();
        }

        private void HandleMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = this.CameraManager.SendRay(Input.mousePosition);

                // If the raycast hits a selectable object
                if (Physics.Raycast(ray, out RaycastHit hit) &&
                    hit.transform.TryGetComponent(out SelectableObject newSelectedObject) &&
                    newSelectedObject != this.selectedObject)
                {
                    // Deselect the currently selected object (if there is one)
                    if (this.selectedObject != null)
                    {
                        this.selectedObject.IsSelected = false;
                    }

                    // Set the new object as the selected object
                    this.selectedObject = newSelectedObject;
                    this.selectedObject.IsSelected = true;

                    // Move the camera to the location of the selected object and lock on to it
                    this.CameraManager.SetTarget(this.selectedObject.transform);
                }
                else if (this.selectedObject != null)
                {
                    // Deselect the currently selected object (if there is one)
                    this.selectedObject.IsSelected = false;
                    this.selectedObject = null;
                    this.CameraManager.SetTarget(null);
                }
            }
        }
    }
}