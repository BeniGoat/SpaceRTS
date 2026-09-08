namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Immutable SI physical properties for ship configuration.
	/// </summary>
	public sealed class ShipPhysicalProperties
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShipPhysicalProperties"/> class with the specified dry mass and fuel capacity.
		/// </summary>
		/// <param name="dryMassKilograms">The dry mass of the ship in kilograms.</param>
		/// <param name="fuelCapacityKilograms">The fuel capacity of the ship in kilograms.</param>
		public ShipPhysicalProperties(double dryMassKilograms, double fuelCapacityKilograms)
		{
			this.DryMassKilograms = PhysicalValueValidation.RequirePositive(dryMassKilograms, nameof(dryMassKilograms));
			this.FuelCapacityKilograms = PhysicalValueValidation.RequireNonNegative(fuelCapacityKilograms, nameof(fuelCapacityKilograms));
		}

		/// <summary>
		/// Gets the dry mass of the ship in kilograms.
		/// </summary>
		public double DryMassKilograms { get; }

		/// <summary>
		/// Gets the fuel capacity of the ship in kilograms.
		/// </summary>
		public double FuelCapacityKilograms { get; }
	}
}
