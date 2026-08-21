using UnityEngine;
using SpaceRTS.Models.Interfaces;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents a selectable component in the game world. It implements the ISelectable interface and
	/// provides functionality for managing selection state and configuring the visual outline of the object.
	/// The component requires an Outline component to be attached to the same GameObject for visualizing selection.
	/// </summary>
	[DisallowMultipleComponent]
    [RequireComponent(typeof(Outline))]
	public class SelectableComponent : MonoBehaviour, ISelectable
    {
		[SerializeField] private bool isSelected;
        [SerializeField] private Color maxColor = new Color32(170, 255, 255, 100);
        [SerializeField] private Color minColor = new Color32(150, 255, 255, 0);

        private Outline outline;

        /// <inheritdoc cref="ISelectable.GetName"/>
		public string GetName()
		{
            // Return the name of the parent GameObject of this component's SystemBody
            return this.gameObject.transform.parent.name;
        }

        /// <inheritdoc cref="ISelectable.GetTransform"/>
        public Transform GetTransform() => this.transform;

        /// <inheritdoc cref="ISelectable.IsSelected"/>
        public bool IsSelected
		{
			get => this.isSelected;
			set
			{
				if (this.isSelected == value) return;
                this.isSelected = value;
                if (this.outline != null)
                {
                    this.outline.enabled = value;
                }
			}
		}

		private void Awake()
        {
			// Configure the selection outline on awake
            this.outline = this.GetComponent<Outline>();
            if (this.outline != null)
            {
                this.outline.enabled = false;
            }
        }

		private void Update()
		{
			// If the object is selected, oscillate the outline color to create a visual effect
			if (this.isSelected)
			{
				this.OscillateOutline();
			}
		}

		/// <inheritdoc cref="ISelectable.ConfigureSelectionOutline"/>
		public void ConfigureSelectionOutline(float outlineWidth)
        {
            this.outline = this.outline != null 
				? this.outline 
				: this.GetComponent<Outline>();

            if (this.outline == null) return;

            this.outline.OutlineWidth = outlineWidth;
            this.outline.enabled = false;
        }

		/// <summary>
		/// Oscillates the outline color between the minimum and maximum colors over time to create a visual effect.
		/// </summary>
		private void OscillateOutline()
		{
			if (this.outline == null) return;

			this.outline.OutlineColor = Color.Lerp(
				this.minColor,
				this.maxColor,
				Mathf.PingPong(Time.realtimeSinceStartup, 1f));
		}
	}
}
