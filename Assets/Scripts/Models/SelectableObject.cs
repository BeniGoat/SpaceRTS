using UnityEngine;

namespace SpaceRTS.Models
{
	/// <summary>
	/// Represents an object in the game that can be selected by the player.
	/// It provides functionality for visual feedback when selected, such as an outline effect.
	/// </summary>
	public class SelectableObject : MonoBehaviour
    {
        protected Outline outline;
        protected Color maxColor = new Color32(170, 255, 255, 100);
        protected Color minColor = new Color32(150, 255, 255, 0);

		private bool isSelected;

		/// <summary>
		/// Gets or sets a value indicating whether this object is currently selected.
		/// </summary>
		public bool IsSelected
		{
			get => this.isSelected;
			set
			{
				if (this.isSelected == value) return;
				this.isSelected = value;
				this.OnSelectionChanged(value);
			}
		}

		/// <summary>
		/// Called when the selection state of the object changes.
        /// It enables or disables the outline effect based on the selection state.
		/// </summary>
		/// <param name="selected">Indicates whether the object is now selected.</param>
		protected virtual void OnSelectionChanged(bool selected)
		{
			if (this.outline == null) return;
			this.outline.enabled = selected;
		}

		private void Update()
		{
			// If the object is selected, oscillate the outline color to create a visual effect
			if (this.isSelected)
			{
				this.OscillateOutline();
			}
		}

		/// <summary>
		/// Configures the outline effect for the selectable object. It retrieves the Outline component,
		/// disables it initially, and sets the specified outline width.
		/// </summary>
		/// <param name="outlineWidth">The width of the outline effect.</param>
		protected void ConfigureSelectionOutline(float outlineWidth)
		{
			this.outline = this.GetComponent<Outline>();
			this.outline.enabled = false;
			this.outline.OutlineWidth = outlineWidth;
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
