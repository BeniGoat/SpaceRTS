using SpaceRTS.Models.Components;
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
        [Header("Axial Spin")]
        [Min(0f)]
        [SerializeField] private float axialAngularMomentum = 0.01f;

        private SelectableComponent selectableComponent;
        private Rotator rotator;
        private bool isTidallyLocked;

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

		/// <summary>
		/// Radius at which objects orbit around the system body, based on the world radius and a minimum offset.
		/// </summary>
		public float OrbitalRadius => this.WorldRadius + Mathf.Max(this.WorldRadius * 0.25f, 0.05f);

		/// <summary>
		/// Gets the body's mass calculated from its local radius.
		/// Assumes constant density of 1 and uses the sphere volume formula.
		/// </summary>
		public float Mass
		{
			get
			{
				float radius = this.LocalRadius;
				return (4f / 3f) * Mathf.PI * radius * radius * radius;
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
			this.ConfigureAxialSpinFromMass();
		}

		/// <summary>
		/// Sets the angular velocity for rotation around the up axis.
		/// </summary>
		/// <param name="degreesPerSecond">The angular velocity value in degrees per second.</param>
		public void SetAngularVelocity(float degreesPerSecond)
		{
			if (this.rotator == null)
				this.rotator = this.GetComponent<Rotator>();

			this.rotator.SetRotationSpeed(degreesPerSecond, Vector3.up);
		}

		/// <summary>
		/// Configures the body as tidally locked by cancelling independent axial self-spin.
		/// </summary>
		public void ConfigureTidalLock()
		{
			this.isTidallyLocked = true;
			this.SetAngularVelocity(0f);
		}

		/// <summary>
		/// Configures axial self-spin using angular momentum and uniform-sphere moment of inertia.
		/// </summary>
		private void ConfigureAxialSpinFromMass()
		{
			// If the body is tidally locked, cancel any independent axial self-spin.
			if (this.isTidallyLocked)
			{
				this.SetAngularVelocity(0f);
				return;
			}

			// Calculate the moment of inertia for a uniform sphere
			float radius = this.LocalRadius;
			if (radius <= 0f)
			{
				// If the radius is zero or negative, cancel any independent axial self-spin.
				this.SetAngularVelocity(0f);
				return;
			}

			// Calculate the moment of inertia for a uniform sphere
			float inertia = 0.4f * this.Mass * radius * radius;
			if (inertia <= Mathf.Epsilon)
			{
				// If the moment of inertia is zero or negative, cancel any independent axial self-spin.
				this.SetAngularVelocity(0f);
				return;
			}

			// Calculate the angular velocity in radians per second and convert to degrees per second
			float angularVelocityRadiansPerSecond = this.axialAngularMomentum / inertia;
			float angularVelocityDegreesPerSecond = angularVelocityRadiansPerSecond * Mathf.Rad2Deg;

			// Bound the angular velocity to a reasonable range to avoid extreme rotation speeds
			this.SetAngularVelocity(Mathf.Clamp(angularVelocityDegreesPerSecond, -360f, 360f));
		}
	}
}