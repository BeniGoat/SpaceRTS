using System;
using SpaceRTS.Managers.Enums;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Represents a deterministic simulation clock that tracks simulation time,
	/// manages game speed, and processes simulation ticks.
	/// </summary>
	/// <remarks>
	/// This clock is deterministic, meaning that given the same inputs, it will always produce the same sequence of simulation ticks.
	/// </remarks>
	public sealed class SimulationClock : ISimulationClock
	{
		private readonly double maxPendingSimulationMinutes;
		private double pendingSimulationMinutes;
		private GameSpeed previousRunningSpeed;

		/// <summary>
		/// Initializes a new instance of the <see cref="SimulationClock"/> class.
		/// </summary>
		/// <param name="epochUtc">The epoch to use for the simulation clock, which must be in UTC.</param>
		/// <param name="startingElapsedMinutes">The starting number of elapsed simulation minutes.</param>
		/// <param name="tickMinutes">The number of simulation minutes per tick.</param>
		/// <param name="baseSimMinutesPerRealSecond">The base number of simulation minutes that elapse per real second.</param>
		/// <param name="maxTicksPerAdvance">The maximum number of ticks that can be processed in a single advance.</param>
		/// <param name="maxPendingSimulationMinutes">The maximum number of simulation minutes that can be pending.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when any of the numeric parameters are out of their valid ranges.</exception>
		public SimulationClock(
			DateTime epochUtc,
			long startingElapsedMinutes,
			long tickMinutes,
			double baseSimMinutesPerRealSecond,
			int maxTicksPerAdvance,
			double maxPendingSimulationMinutes = 1_000_000d)
		{
			if (tickMinutes <= 0)
				throw new ArgumentOutOfRangeException(nameof(tickMinutes));
			if (baseSimMinutesPerRealSecond <= 0d || double.IsNaN(baseSimMinutesPerRealSecond) || double.IsInfinity(baseSimMinutesPerRealSecond))
				throw new ArgumentOutOfRangeException(nameof(baseSimMinutesPerRealSecond));
			if (maxTicksPerAdvance <= 0)
				throw new ArgumentOutOfRangeException(nameof(maxTicksPerAdvance));
			if (maxPendingSimulationMinutes <= 0d || double.IsNaN(maxPendingSimulationMinutes) || double.IsInfinity(maxPendingSimulationMinutes))
				throw new ArgumentOutOfRangeException(nameof(maxPendingSimulationMinutes));

			this.EpochUtc = epochUtc.Kind == DateTimeKind.Utc
				? epochUtc
				: DateTime.SpecifyKind(epochUtc, DateTimeKind.Utc);
			this.Calendar = new SimulationCalendar(startingElapsedMinutes);
			this.TickMinutes = tickMinutes;
			this.BaseSimMinutesPerRealSecond = baseSimMinutesPerRealSecond;
			this.MaxTicksPerAdvance = maxTicksPerAdvance;
			this.maxPendingSimulationMinutes = maxPendingSimulationMinutes;
			this.EffectiveSpeed = GameSpeed.Paused;
			this.previousRunningSpeed = GameSpeed.x1;
			this.pendingSimulationMinutes = 0d;
		}

		/// <inheritdoc/>
		public DateTime EpochUtc { get; }

		/// <inheritdoc/>
		public SimulationCalendar Calendar { get; private set; }

		/// <inheritdoc/>
		public GameSpeed EffectiveSpeed { get; private set; }

		/// <inheritdoc/>
		public bool IsPaused => this.EffectiveSpeed == GameSpeed.Paused;

		/// <inheritdoc/>
		public long TickMinutes { get; }

		/// <inheritdoc/>
		public double BaseSimMinutesPerRealSecond { get; }

		/// <inheritdoc/>
		public int MaxTicksPerAdvance { get; }

		/// <inheritdoc/>
		public ulong TickSequence { get; private set; }

		/// <inheritdoc/>
		public long PendingTickCount => (long)(this.pendingSimulationMinutes / this.TickMinutes);

		/// <inheritdoc/>
		public double PendingSimulationMinutes => this.pendingSimulationMinutes;

		/// <inheritdoc/>
		public GameSpeed ApplySpeedRequest(GameSpeed requestedSpeed)
		{
			// If the requested speed is Paused, toggle between Paused and the previous running speed.
			if (requestedSpeed == GameSpeed.Paused)
			{
				if (this.IsPaused)
				{
					this.EffectiveSpeed = this.previousRunningSpeed;
				}
				else
				{
					this.previousRunningSpeed = this.EffectiveSpeed;
					this.EffectiveSpeed = GameSpeed.Paused;
				}

				return this.EffectiveSpeed;
			}

			this.previousRunningSpeed = requestedSpeed;
			this.EffectiveSpeed = requestedSpeed;
			return this.EffectiveSpeed;
		}

		/// <inheritdoc/>
		public SimulationAdvanceResult AdvanceRealSeconds(double realElapsedSeconds, Action<SimulationTick> onTickProcessed)
		{
			if (realElapsedSeconds < 0d || double.IsNaN(realElapsedSeconds) || double.IsInfinity(realElapsedSeconds))
				throw new ArgumentOutOfRangeException(nameof(realElapsedSeconds));

			// Accumulate simulation work based on elapsed real time, unless the simulation is paused.
			if (!this.IsPaused)
			{
				this.AccumulateSimulationWork(realElapsedSeconds);
			}

			// Process simulation ticks up to the configured per-advance limit.
			int processed = 0;
			while (processed < this.MaxTicksPerAdvance && this.pendingSimulationMinutes >= this.TickMinutes)
			{
				// Process a single simulation tick.
				this.pendingSimulationMinutes -= this.TickMinutes;
				this.Calendar = this.Calendar.AddMinutes(this.TickMinutes);
				this.TickSequence = checked(this.TickSequence + 1UL);
				processed++;

				onTickProcessed?.Invoke(new SimulationTick(this.TickSequence, this.TickMinutes, this.Calendar, this.EffectiveSpeed));
			}

			return new SimulationAdvanceResult(
				processed,
				this.PendingTickCount,
				this.pendingSimulationMinutes,
				this.Calendar);
		}

		/// <summary>
		/// Accumulates pending simulation time by converting elapsed real-world seconds into simulation minutes using the
		/// current effective speed and base simulation rate.
		/// </summary>
		/// <remarks>Uses the integer value of the effective speed as a multiplier and updates the pending simulation
		/// total only when the fail-safe bound is not exceeded.</remarks>
		/// <param name="realElapsedSeconds">Elapsed real-world time, in seconds, to convert and add to pending simulation minutes. Non-positive values are
		/// ignored.</param>
		/// <exception cref="InvalidOperationException">Thrown when adding the computed simulation minutes would exceed the configured pending-simulation fail-safe limit.</exception>
		private void AccumulateSimulationWork(double realElapsedSeconds)
		{
			if (realElapsedSeconds <= 0d)
				return;

			// Convert elapsed real-world seconds into simulation minutes, factoring in the effective game speed.
			double speedMultiplier = (int)this.EffectiveSpeed;
			double simulationMinutes = realElapsedSeconds * this.BaseSimMinutesPerRealSecond * speedMultiplier;
			double nextPending = this.pendingSimulationMinutes + simulationMinutes;
			if (nextPending > this.maxPendingSimulationMinutes)
			{
				throw new InvalidOperationException(
					$"Pending simulation minutes exceeded fail-safe bound of {this.maxPendingSimulationMinutes}." +
					" This protects performance by failing explicitly instead of silently discarding simulation time.");
			}

			this.pendingSimulationMinutes = nextPending;
		}
	}
}
