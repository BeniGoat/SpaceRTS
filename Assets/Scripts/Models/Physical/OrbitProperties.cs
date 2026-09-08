using System;

namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Immutable circular-orbit configuration in SI units.
	/// </summary>
	public sealed class OrbitProperties
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrbitProperties"/> class with the specified orbital properties.
		/// </summary>
		/// <param name="parentBodyId">The identifier of the parent system body.</param>
		/// <param name="orbitalRadiusMeters">The orbital radius in meters.</param>
		/// <param name="orbitalPeriodSeconds">The orbital period in seconds.</param>
		/// <param name="initialPhaseRadians">The initial phase angle in radians.</param>
		/// <exception cref="ArgumentException">Thrown when the parent body identifier is not defined.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when any of the specified values are not positive.</exception>
		public OrbitProperties(
			SystemBodyId parentBodyId,
			double orbitalRadiusMeters,
			double orbitalPeriodSeconds,
			double initialPhaseRadians)
		{
			if (!parentBodyId.IsDefined)
				throw new ArgumentException("Parent body identifier must be defined.", nameof(parentBodyId));

			this.ParentBodyId = parentBodyId;
			this.OrbitalRadiusMeters = PhysicalValueValidation.RequirePositive(orbitalRadiusMeters, nameof(orbitalRadiusMeters));
			this.OrbitalPeriodSeconds = PhysicalValueValidation.RequirePositive(orbitalPeriodSeconds, nameof(orbitalPeriodSeconds));
			this.InitialPhaseRadians = PhysicalValueValidation.NormalizeRadians(initialPhaseRadians, nameof(initialPhaseRadians));
		}
		
		/// <summary>
		/// Gets the identifier of the parent system body.
		/// </summary>
		public SystemBodyId ParentBodyId { get; }

		/// <summary>
		/// Gets the orbital radius in meters.
		/// </summary>
		public double OrbitalRadiusMeters { get; }

		/// <summary>
		/// Gets the orbital period in seconds.
		/// </summary>
		public double OrbitalPeriodSeconds { get; }

		/// <summary>
		/// Gets the initial phase angle in radians.
		/// </summary>
		public double InitialPhaseRadians { get; }
	}
}
