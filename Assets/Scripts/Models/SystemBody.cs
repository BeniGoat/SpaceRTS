using SpaceRTS.Factories;
using UnityEngine;

namespace SpaceRTS.Models
{
    /// <summary>
    /// Represents a celestial body in the star system, such as a planet or moon.
    /// It holds properties for orbital distance and size, and provides methods
    /// to configure its visual representation in the game world.
    /// Ship spawning is handled by the <see cref="ShipFactory"/>
    /// </summary>
    [RequireComponent(typeof(SelectableComponent))]
    [RequireComponent(typeof(ShipFactory))]
    public class SystemBody : MonoBehaviour
    {
        private SelectableComponent selectableComponent;
        private ShipFactory shipFactory;

        /// <summary>
        /// Gets or sets the orbital distance of the celestial body from its parent object.
        /// </summary>
        public float OrbitalDistance { get; set; }

        /// <summary>
        /// Gets the maximum world-space radius of the body.
        /// </summary>
        public float MaxRadius
        {
            get
            {
                // Calculate the maximum radius based on the lossy scale of the transform.
                Vector3 scale = this.transform.lossyScale;
                return Mathf.Max(scale.x, scale.y, scale.z) * 0.5f;
            }
        }

        /// <summary>
        /// Gets the ShipFactory component attached to this celestial body, if it is active.
        /// </summary>
        public ShipFactory ShipFactory => this.shipFactory.isActive ? this.shipFactory : null;

        private void Awake()
        {
            this.selectableComponent = this.GetComponent<SelectableComponent>();
            this.shipFactory = this.GetComponent<ShipFactory>();
        }

        /// <summary>
        /// Sets the size of the celestial body uniformly in all dimensions based on the specified diameter.
        /// </summary>
        /// <param name="diameter">The diameter of the celestial body.</param>
        public void SetBodySize(float diameter)
        {
            this.SetBodySize(diameter, diameter, diameter);
        }

        /// <summary>
        /// Sets the size of the celestial body in each dimension based on the specified values.
        /// </summary>
        /// <param name="x">The scale along the x-axis.</param>
        /// <param name="y">The scale along the y-axis.</param>
        /// <param name="z">The scale along the z-axis.</param>
        public void SetBodySize(float x, float y, float z)
        {
            this.SetBodySize(new Vector3(x, y, z));
        }

        /// <summary>
        /// Sets the size of the celestial body and updates its size-dependent selection outline.
        /// </summary>
        /// <param name="scale">The scale vector for each dimension.</param>
        public void SetBodySize(Vector3 scale)
        {
            this.transform.localScale = scale;
            this.selectableComponent.ConfigureSelectionOutline(this.MaxRadius * 2f);
        }
    }
}