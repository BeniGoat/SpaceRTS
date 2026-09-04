using System;
using SpaceRTS.Managers.Enums;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Authoritative simulation-time clock contract.
	/// </summary>
	public interface ISimulationClock
	{
		/// <summary>
		/// Gets the epoch in UTC used for the simulation calendar.
		/// </summary>
		DateTime EpochUtc { get; }

		/// <summary>
		/// Gets the current simulation calendar value.
		/// </summary>
		SimulationCalendar Calendar { get; }

		/// <summary>
		/// Gets the effective game speed.
		/// </summary>
		GameSpeed EffectiveSpeed { get; }

		/// <summary>
		/// Gets a value indicating whether the simulation is paused.
		/// </summary>
		bool IsPaused { get; }

		/// <summary>
		/// Gets the number of simulation minutes per tick.
		/// </summary>
		long TickMinutes { get; }

		/// <summary>
		/// Gets the base number of simulation minutes that elapse per real second.
		/// </summary>
		double BaseSimMinutesPerRealSecond { get; }

		/// <summary>
		/// Gets the maximum number of ticks that can be processed in a single advance.
		/// </summary>
		int MaxTicksPerAdvance { get; }

		/// <summary>
		/// Gets the current tick sequence number.
		/// </summary>
		ulong TickSequence { get; }

		/// <summary>
		/// Applies a requested game speed and returns the effective speed.
		/// </summary>
		/// <param name="requestedSpeed">The requested game speed.</param>
		/// <returns>The effective game speed after applying the request.</returns>
		GameSpeed ApplySpeedRequest(GameSpeed requestedSpeed);

		/// <summary>
		/// Advances the simulation by the specified number of real seconds.
		/// </summary>
		/// <param name="realElapsedSeconds">The number of real seconds to advance.</param>
		/// <param name="onTickProcessed">An action to invoke for each processed tick.</param>
		/// <returns>The result of the simulation advance.</returns>
		SimulationAdvanceResult AdvanceRealSeconds(double realElapsedSeconds, Action<SimulationTick> onTickProcessed);
	}
}
