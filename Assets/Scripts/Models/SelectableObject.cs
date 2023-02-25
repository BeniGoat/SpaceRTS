using UnityEngine;

namespace SpaceRTS.Models
{
        public class SelectableObject : MonoBehaviour
        {
                public bool IsSelected { get; private set; }

                protected void OnMouseDown()
                {
                        // Set the selected status of the object
                        this.IsSelected = !this.IsSelected;

                        // Perform other logic when the object is clicked
                        Debug.Log($"{this.transform.parent.name} clicked!");
                }
        }
}
