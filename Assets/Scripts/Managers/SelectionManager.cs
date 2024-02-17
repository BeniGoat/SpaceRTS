using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        //public CameraRig CameraRig;
        public CameraManager CameraManager;
        private SelectableObject currentSelectedObject;

        private void Update()
        {
            this.HandleMouseClick();
        }

        private void HandleMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectableObject newSelectedObject = this.GetSelectableObjectClicked();

                // If the raycast hits a selectable object
                if (newSelectedObject != null && newSelectedObject != this.currentSelectedObject)
                {
                    // Deselect the currently selected object (if there is one)
                    if (this.currentSelectedObject != null)
                    {
                        this.currentSelectedObject.IsSelected = false;
                    }

                    // Set the new object as the selected object
                    this.currentSelectedObject = newSelectedObject;
                    this.currentSelectedObject.IsSelected = true;

                    // Move the camera to the location of the selected object and lock on to it
                    this.CameraManager.SetTarget(this.currentSelectedObject.transform);
                }
                else if (this.currentSelectedObject != null)
                {
                    if (this.currentSelectedObject is Ship ship)
                    {
                        ship.HandleMovement();
                    }

                    // Deselect the currently selected object (if there is one)
                    this.currentSelectedObject.IsSelected = false;
                    this.currentSelectedObject = null;
                    this.CameraManager.SetTarget(null);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (this.currentSelectedObject is Ship ship)
                {
                    SelectableObject newSelectedObject = this.GetSelectableObjectClicked();

                    if (newSelectedObject != this.currentSelectedObject)
                    {                    
                        ship.HandleMovement(newSelectedObject);
                    }
                }
            }
        }

        private SelectableObject GetSelectableObjectClicked()
        {
            SelectableObject selectedObject = null;

            // If the raycast hits a selectable object
            Ray ray = this.CameraManager.SendRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.TryGetComponent(out SelectableObject selectableObject))
            {
                selectedObject = selectableObject;
            }

            return selectedObject;
        }
    }
}