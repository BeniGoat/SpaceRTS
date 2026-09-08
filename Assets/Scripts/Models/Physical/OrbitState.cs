namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Mutable runtime state for orbital phase in radians.
	/// </summary>
	public sealed class OrbitState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrbitState"/> class with the specified current phase angle in radians.
		/// </summary>
		/// <param name="currentPhaseRadians">The current phase angle in radians.</param>
		public OrbitState(double currentPhaseRadians)
		{
			this.CurrentPhaseRadians = PhysicalValueValidation.NormalizeRadians(currentPhaseRadians, nameof(currentPhaseRadians));
		}

		/// <summary>
		/// Gets the current phase angle in radians.
		/// </summary>
		public double CurrentPhaseRadians { get; private set; }

		/// <summary>
		/// Sets the current phase angle in radians.
		/// </summary>
		/// <param name="phaseRadians">The phase angle in radians.</param>
		public void SetPhase(double phaseRadians)
		{
			this.CurrentPhaseRadians = PhysicalValueValidation.NormalizeRadians(phaseRadians, nameof(phaseRadians));
		}

		/// <summary>
		/// Advances the current phase angle by the specified delta in radians.
		/// </summary>
		/// <param name="deltaRadians">The delta angle in radians.</param>
		public void AdvanceByRadians(double deltaRadians)
		{
			double updated = this.CurrentPhaseRadians + PhysicalValueValidation.RequireFinite(deltaRadians, nameof(deltaRadians));
			this.CurrentPhaseRadians = PhysicalValueValidation.NormalizeRadians(updated, nameof(deltaRadians));
		}
	}
}
