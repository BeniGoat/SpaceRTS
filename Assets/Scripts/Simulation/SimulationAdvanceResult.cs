namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Represents the result of advancing the simulation by a certain number of ticks.
	/// </summary>
	public readonly struct SimulationAdvanceResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SimulationAdvanceResult"/> struct.
		/// </summary>
		/// <param name="processedTickCount">The number of ticks that were processed.</param>
		/// <param name="pendingTickCount">The number of ticks that are still pending.</param>
		/// <param name="pendingSimulationMinutes">The number of simulation minutes that are still pending.</param>
		/// <param name="calendar">The current simulation calendar value.</param>
		public SimulationAdvanceResult(int processedTickCount, long pendingTickCount, double pendingSimulationMinutes, SimulationCalendar calendar)
		{
			this.ProcessedTickCount = processedTickCount;
			this.PendingTickCount = pendingTickCount;
			this.PendingSimulationMinutes = pendingSimulationMinutes;
			this.Calendar = calendar;
		}

		/// <summary>
		/// Gets the number of ticks that were processed.
		/// </summary>
		public int ProcessedTickCount { get; }

		/// <summary>
		/// Gets the number of ticks that are still pending.
		/// </summary>
		public long PendingTickCount { get; }

		/// <summary>
		/// Gets the number of simulation minutes that are still pending.
		/// </summary>
		public double PendingSimulationMinutes { get; }

		/// <summary>
		/// Gets the current simulation calendar value.
		/// </summary>
		public SimulationCalendar Calendar { get; }
	}
}
