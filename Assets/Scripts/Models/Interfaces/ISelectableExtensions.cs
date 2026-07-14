using UnityEngine;
using SpaceRTS.Models.Interfaces;

namespace SpaceRTS.Models
{
    /// <summary>
    /// Provides extension methods for the ISelectable interface, allowing for convenient 
    /// access to Unity components and transforms associated with selectable objects.
    /// </summary>
    public static class ISelectableExtensions
    {
        /// <summary>
        /// Attempts to retrieve a component of type T from the selectable object. If the selectable is a Component,
        ///  it uses Unity's TryGetComponent method to find the component.
        /// If the selectable is not a Component, it returns false and sets the output component to null.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="selectable">The selectable object.</param>
        /// <param name="component">The retrieved component, or null if not found.</param>
        /// <returns>True if the component was found; otherwise, false.</returns>
        public static bool TryGetComponent<T>(this ISelectable selectable, out T component)
            where T : Component
        {
            if (selectable is Component componentOwner)
            {
                return componentOwner.TryGetComponent(out component);
            }

            component = null;
            return false;
        }

        /// <summary>
        /// Retrieves the Transform of the selectable object if it is a Component.
        /// If the selectable is not a Component, it returns null.
        /// </summary>
        /// <param name="selectable">The selectable object.</param>
        /// <returns>The Transform of the selectable object, or null if it is not a Component.</returns>
        public static Transform GetTransform(this ISelectable selectable)
        {
            return (selectable as Component)?.transform;
        }
    }
}