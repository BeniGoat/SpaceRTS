using SpaceRTS.Models.Components;
using System;
using UnityEngine;

namespace SpaceRTS.Models
{
    /// <summary>
    /// Represents a celestial body in the star system, such as a planet or moon.
    /// It holds properties for orbital distance and size, and provides methods
    /// to configure its visual representation in the game world.
    /// </summary>
    [RequireComponent(typeof(SelectableComponent))]
    [RequireComponent(typeof(Rotator))] 
    public class SystemBody : MonoBehaviour
    {
        private SelectableComponent selectableComponent;
        private Rotator rotator;

		/// <summary>
		/// Gets the body's maximum world-space radius.
		/// </summary>
		public float WorldRadius
        {
            get
            {
				// Calculate the maximum radius based on the lossy scale of the transform.
				Vector3 scale = this.transform.lossyScale;
				return Mathf.Max(scale.x, scale.y, scale.z) * 0.5f;
			}
        }

		/// <summary>
		/// Gets the body's maximum local-space radius.
		/// </summary>
		public float LocalRadius
        {
            get
            {
                // Calculate the local radius based on the local scale of the transform.
                Vector3 scale = this.transform.localScale;
                return Mathf.Max(scale.x, scale.y, scale.z) * 0.5f;
            }
        }

		private void Awake()
        {
            this.selectableComponent = this.GetComponent<SelectableComponent>();
			this.rotator = this.GetComponent<Rotator>();
		}

		/// <summary>
		/// Sets the size of the celestial body uniformly in all dimensions based on the specified diameter.
		/// </summary>
		/// <param name="diameter">The diameter of the celestial body.</param>
		public void SetBodySize(float diameter) => this.SetBodySize(diameter, diameter, diameter);

		/// <summary>
		/// Sets the size of the celestial body in each dimension based on the specified values.
		/// </summary>
		/// <param name="x">The scale along the x-axis.</param>
		/// <param name="y">The scale along the y-axis.</param>
		/// <param name="z">The scale along the z-axis.</param>
		public void SetBodySize(float x, float y, float z) => this.SetBodySize(new Vector3(x, y, z));

		/// <summary>
		/// Sets the size of the celestial body and updates its size-dependent selection outline.
		/// </summary>
		/// <param name="scale">The scale vector for each dimension.</param>
		public void SetBodySize(Vector3 scale)
        {
            this.transform.localScale = scale;
			this.selectableComponent.ConfigureSelectionOutline(this.WorldRadius * 2f);
		}

		/// <summary>
		/// Sets the angular velocity for rotation around the up axis.
		/// </summary>
		/// <param name="degreesPerSecond">The angular velocity value in degrees per second.</param>
		public void SetAngularVelocity(float degreesPerSecond) => this.rotator.SetRotationSpeed(degreesPerSecond, Vector3.up);
	}
}