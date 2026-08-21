using UnityEngine;

namespace SpaceRTS.Models.Interfaces
{
    /// <summary>
    /// Defines the interface for selectable objects in the game world.
    /// It provides properties and methods to manage the selection state and configure the visual outline of the object.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this object is currently selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Configures the outline effect for the selectable object. It retrieves the Outline component,
        /// disables it initially, and sets the specified outline width.
        /// </summary>
        /// <param name="outlineWidth">The width of the outline effect.</param>
        void ConfigureSelectionOutline(float outlineWidth);
        
        /// <summary>
        /// Gets the name of the selectable object.
        /// </summary>
        /// <returns>The name as a string.</returns>
        string GetName();

        /// <summary>
        /// Gets the Transform that defines position, rotation, and scale.
        /// </summary>
        /// <returns>The Transform used for positioning, rotation, and scaling.</returns>
        Transform GetTransform();
    }
}