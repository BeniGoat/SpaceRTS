using UnityEngine;

namespace SpaceRTS.Models
{
    public class Ship : SelectableObject
    {
        public Color pathLineColour = new Color(0.1f, 1, 0.1f, 0.5f);

        private LineRenderer path;
        private SystemBody destinationBody;

        public SystemBody CurrentSystemBody { get; set; }

        private void Awake()
        {
            this.ConfigureSelectionOutline(1f);
            this.ConfigureShipPathLine();
        }

        private void Update()
        {
            this.HandleObjectSelection();
            if (this.path.enabled && this.destinationBody != null)
            {
                this.UpdatePathLine();
            }
        }

        public void HandleMovement(SelectableObject destinationObject = null)
        {
            if (destinationObject == null || 
                !(destinationObject is SystemBody destinationBody) || 
                destinationBody == this.CurrentSystemBody)
            {
                this.path.enabled = false;
                this.path.forceRenderingOff = true;
                this.destinationBody = null;
            }
            else
            {
                Debug.Log("Valid destination body clicked");
                this.path.enabled = true;
                this.path.forceRenderingOff = false;
                this.destinationBody = destinationBody;
            }
        }

        private void ConfigureShipPathLine()
        {
            // Initialize the ship path line
            this.path = this.GetComponent<LineRenderer>();
            this.path.enabled = false;
            this.path.forceRenderingOff = false;
            this.path.useWorldSpace = true;
            this.path.loop = false;
            this.path.startWidth = 0.01f;
            this.path.endWidth = 0.01f;
            this.path.startColor = new Color(
                    this.pathLineColour.r,
                    this.pathLineColour.g,
                    this.pathLineColour.b,
                    this.pathLineColour.a * 0.5f);
            this.path.endColor = this.pathLineColour;
            this.path.positionCount = 2;
        }

        private void UpdatePathLine()
        {
            this.path.SetPosition(0, this.CurrentSystemBody.transform.position);
            this.path.SetPosition(1, this.destinationBody.transform.position);
        }
    }
}