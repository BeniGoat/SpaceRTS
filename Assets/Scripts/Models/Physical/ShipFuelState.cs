using System;

namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Mutable runtime fuel state for a ship.
	/// </summary>
	public sealed class ShipFuelState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShipFuelState"/> class with the specified ship physical properties and current fuel mass.
		/// </summary>
		/// <param name="shipPhysicalProperties">The physical properties of the ship.</param>
		/// <param name="currentFuelMassKilograms">The current fuel mass in kilograms.</param>
		public ShipFuelState(ShipPhysicalProperties shipPhysicalProperties, double currentFuelMassKilograms)
		{
			if (shipPhysicalProperties == null)
				throw new ArgumentException("Ship physical properties are required.", nameof(shipPhysicalProperties));

			this.ShipPhysicalProperties = shipPhysicalProperties;
			this.CurrentFuelMassKilograms = PhysicalValueValidation.RequireRangeInclusive(
				currentFuelMassKilograms,
				0d,
				shipPhysicalProperties.FuelCapacityKilograms,
				nameof(currentFuelMassKilograms));
		}
		
		/// <summary>
		/// Gets the physical properties of the ship.
		/// </summary>
		public ShipPhysicalProperties ShipPhysicalProperties { get; }

		/// <summary>
		/// Gets the current fuel mass in kilograms.
		/// </summary>
		public double CurrentFuelMassKilograms { get; private set; }

		/// <summary>
		/// Gets the total mass of the ship in kilograms, including dry mass and current fuel mass.
		/// </summary>
		public double TotalMassKilograms => this.ShipPhysicalProperties.DryMassKilograms + this.CurrentFuelMassKilograms;

		/// <summary>
		/// Sets the current fuel mass in kilograms.
		/// </summary>
		/// <param name="currentFuelMassKilograms">The current fuel mass in kilograms.</param>
		public void SetCurrentFuelMassKilograms(double currentFuelMassKilograms)
		{
			this.CurrentFuelMassKilograms = PhysicalValueValidation.RequireRangeInclusive(
				currentFuelMassKilograms,
				0d,
				this.ShipPhysicalProperties.FuelCapacityKilograms,
				nameof(currentFuelMassKilograms));
		}
	}
}
