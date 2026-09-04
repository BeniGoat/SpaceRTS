namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Simulation-domain system executed deterministically after each processed simulation tick event.
	/// </summary>
	public interface ISimulationSystem
	{
		/// <summary>
		/// Gets the priority.
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// Performs the operation for the specified simulation tick.
		/// </summary>
		/// <param name="tick">Simulation tick to process.</param>
		void Execute(in SimulationTick tick);
	}
}