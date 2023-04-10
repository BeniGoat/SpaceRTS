using UnityEngine;
using UnityEngine.Events;

namespace SpaceRTS.Models
{
        public class SelectableObject : MonoBehaviour
        {
                public UnityEvent onSelected;
                public UnityEvent onDeselected;
                public bool IsSelected { get; private set; }

                protected void OnMouseDown()
                {
                        // Set the selected status of the object
                        if (!this.IsSelected)
                        {
                                // select this object
                                this.IsSelected = true;
                                this.onSelected.Invoke();
                        }
                        else
                        {
                                // deselect this object
                                this.IsSelected = false;
                                this.onDeselected.Invoke();
                        }

                        // Perform other logic when the object is clicked
                        Debug.Log($"{this.transform.parent.name} clicked!");
                }
        }
}
