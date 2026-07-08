using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents a ship in the game, which can be selected, moved to a destination system body, and visualized with a path line.
	/// </summary>
	public class Ship : SelectableObject
    {
		[SerializeField] private Color pathLineColour = new Color(0.1f, 1, 0.1f, 0.5f);

		private LineRenderer path;
        private SystemBody destinationBody;

        public SystemBody CurrentSystemBody { get; set; }

        private void Awake()
        {
			// Configure the selection outline and ship path line on awake
			this.ConfigureSelectionOutline(1f);
            this.ConfigureShipPathLine();
        }

        private void Update()
        {
			// Update the ship path line if it is enabled and a destination body is set
			if (this.path.enabled && this.destinationBody != null)
            {
                this.UpdatePathLine();
            }
        }

		/// <summary>
		/// Sets the destination system body for the ship and enables the path line renderer to visualize the path.
		/// </summary>
		/// <param name="target">The target system body to set as the destination.</param>
		public void SetDestination(SystemBody target)
		{
			this.destinationBody = target;
			this.path.enabled = true;
			this.path.forceRenderingOff = false;
		}

		/// <summary>
		/// Clears the destination system body for the ship and disables the path line renderer.
		/// </summary>
		public void ClearDestination()
		{
			this.destinationBody = null;
			this.path.enabled = false;
			this.path.forceRenderingOff = true;
		}

        private void ConfigureShipPathLine()
        {
			// Configure the LineRenderer component for the ship's path line visualization
			this.path = this.GetComponent<LineRenderer>();
            this.path.enabled = false;
            this.path.forceRenderingOff = true;
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

		/// <summary>
		/// Updates the positions of the path line renderer to visualize the path
        /// from the current system body to the destination system body.
		/// </summary>
		private void UpdatePathLine()
        {
            this.path.SetPosition(0, this.CurrentSystemBody.transform.position);
            this.path.SetPosition(1, this.destinationBody.transform.position);
        }
    }
}