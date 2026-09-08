using System;

namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Immutable SI physical properties for a system body configuration.
	/// </summary>
	public sealed class SystemBodyPhysicalProperties
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemBodyPhysicalProperties"/> class with the specified physical properties.
		/// </summary>
		/// <param name="radiusMeters">The radius of the system body in meters.</param>
		/// <param name="massKilograms">The mass of the system body in kilograms.</param>
		/// <param name="axialRotationPeriodSeconds">The axial rotation period of the system body in seconds.</param>
		/// <param name="isTidallyLocked">Indicates whether the system body is tidally locked.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when any of the specified values are not positive.</exception>
		public SystemBodyPhysicalProperties(
			double radiusMeters,
			double massKilograms,
			double axialRotationPeriodSeconds,
			bool isTidallyLocked = false)
		{
			this.RadiusMeters = PhysicalValueValidation.RequirePositive(radiusMeters, nameof(radiusMeters));
			this.MassKilograms = PhysicalValueValidation.RequirePositive(massKilograms, nameof(massKilograms));
			this.AxialRotationPeriodSeconds = PhysicalValueValidation.RequirePositive(axialRotationPeriodSeconds, nameof(axialRotationPeriodSeconds));
			this.IsTidallyLocked = isTidallyLocked;
		}
		
		/// <summary>
		/// Gets the radius of the system body in meters.
		/// </summary>
		public double RadiusMeters { get; }

		/// <summary>
		/// Gets the mass of the system body in kilograms.
		/// </summary>
		public double MassKilograms { get; }

		/// <summary>
		/// Gets the axial rotation period of the system body in seconds.
		/// </summary>
		public double AxialRotationPeriodSeconds { get; }

		/// <summary>
		/// Gets a value indicating whether the system body is tidally locked.
		/// </summary>
		public bool IsTidallyLocked { get; }

		/// <summary>
		/// Gets the diameter of the system body in meters.
		/// </summary>
		public double DiameterMeters => this.RadiusMeters * 2d;

		/// <summary>
		/// Gets the volume of the system body in cubic meters.
		/// </summary>
		public double VolumeCubicMeters => (4d / 3d) * Math.PI * this.RadiusMeters * this.RadiusMeters * this.RadiusMeters;

		/// <summary>
		/// Gets the density of the system body in kilograms per cubic meter.
		/// </summary>
		public double DensityKilogramsPerCubicMeter => this.MassKilograms / this.VolumeCubicMeters;
	}
}
