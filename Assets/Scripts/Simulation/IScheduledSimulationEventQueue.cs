using System;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Represents a queue of scheduled simulation events that can be executed at specific elapsed minutes.
	/// </summary>
	public interface IScheduledSimulationEventQueue
	{
		/// <summary>
		/// Schedules an action to run after the specified number of elapsed minutes.
		/// </summary>
		/// <param name="dueElapsedMinutes">Number of elapsed minutes to wait before execution.</param>
		/// <param name="action">Delegate to invoke when the scheduled time is reached.</param>
		void Schedule(long dueElapsedMinutes, Action action);
		
		/// <summary>
		/// Executes all actions that are due at the specified number of elapsed minutes.
		/// </summary>
		/// <param name="currentElapsedMinutes">The current number of elapsed minutes.</param>
		void ExecuteDue(long currentElapsedMinutes);
	}
}