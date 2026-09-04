using System;
using System.Collections.Generic;
using NUnit.Framework;
using SpaceRTS.Managers.Enums;

namespace SpaceRTS.Simulation.Tests
{
	/// <summary>
	/// Provides unit tests for simulation clock progression, speed handling, tick processing limits,
	/// scheduling order, and deterministic tick pipeline execution.
	/// </summary>
	/// <remarks>
	/// Covers calendar formatting, pause toggling, backlog fail-safe behavior,
	/// and dispatcher ordering to validate deterministic simulation timing and execution flow.
	/// </remarks>
	public class SimulationClockTests
	{
		private static readonly DateTime EpochUtc = new(2178, 4, 23, 14, 32, 0, DateTimeKind.Utc);

		/// <summary>
		/// Verifies that the <see cref="SimulationCalendar.Format(DateTime)"/> method
		/// correctly formats the calendar using the provided epoch and elapsed minutes.
		/// </summary>
		[Test]
		public void Calendar_FormatsUsingEpochAndElapsedMinutes()
		{
			SimulationCalendar calendar = new(90);

			string formatted = calendar.Format(EpochUtc);

			Assert.AreEqual("2178-04-23 16:02", formatted);
		}

		/// <summary>
		/// Verifies that advancing real time while paused does not process simulation ticks or advance calendar time.
		/// </summary>
		/// <remarks>Asserts paused effective speed, zero processed ticks, zero pending ticks, and unchanged elapsed
		/// minutes.</remarks>
		[Test]
		public void Advance_WhenPaused_DoesNotProcessTicksOrAdvanceCalendar()
		{
			SimulationClock clock = CreateClock();

			SimulationAdvanceResult result = clock.AdvanceRealSeconds(10d, null);

			Assert.AreEqual(GameSpeed.Paused, clock.EffectiveSpeed);
			Assert.AreEqual(0, result.ProcessedTickCount);
			Assert.AreEqual(0, clock.Calendar.ElapsedMinutes);
			Assert.AreEqual(0, result.PendingTickCount);
		}

		/// <summary>
		/// Verifies that advancing the simulation by real time applies the selected game speed multiplier when computing
		/// elapsed in-game minutes.
		/// </summary>
		/// <param name="speed">Game speed multiplier applied before advancing time.</param>
		/// <param name="realSeconds">Amount of real-world seconds to advance the simulation clock.</param>
		/// <param name="expectedMinutes">Expected elapsed in-game minutes after advancement.</param>
		[TestCase(GameSpeed.x1, 1d, 60)]
		[TestCase(GameSpeed.x2, 1d, 120)]
		[TestCase(GameSpeed.x5, 1d, 300)]
		[TestCase(GameSpeed.x10, 1d, 600)]
		public void Advance_RespectsSpeedMultipliers(GameSpeed speed, double realSeconds, long expectedMinutes)
		{
			SimulationClock clock = CreateClock();
			clock.ApplySpeedRequest(speed);

			clock.AdvanceRealSeconds(realSeconds, null);

			Assert.AreEqual(expectedMinutes, clock.Calendar.ElapsedMinutes);
		}

		/// <summary>
		/// Verifies that advancing the simulation by one full second or by equivalent fractional real-time increments
		/// produces identical elapsed minutes, pending ticks, and pending simulation minutes.
		/// </summary>
		/// <remarks>Confirms deterministic accumulation behavior for fractional real-time advancement at the same
		/// clock configuration and speed.</remarks>
		[Test]
		public void Advance_AccumulatesFractionalRealTimeDeterministically()
		{
			SimulationClock clockA = CreateClock(baseRate: 1d, tickMinutes: 1);
			clockA.ApplySpeedRequest(GameSpeed.x1);

			SimulationClock clockB = CreateClock(baseRate: 1d, tickMinutes: 1);
			clockB.ApplySpeedRequest(GameSpeed.x1);

			clockA.AdvanceRealSeconds(1d, null);
			clockB.AdvanceRealSeconds(0.25d, null);
			clockB.AdvanceRealSeconds(0.25d, null);
			clockB.AdvanceRealSeconds(0.25d, null);
			clockB.AdvanceRealSeconds(0.25d, null);

			Assert.AreEqual(clockA.Calendar.ElapsedMinutes, clockB.Calendar.ElapsedMinutes);
			Assert.AreEqual(clockA.PendingTickCount, clockB.PendingTickCount);
			Assert.AreEqual(clockA.PendingSimulationMinutes, clockB.PendingSimulationMinutes, 0.00001d);
		}

		/// <summary>
		/// Verifies that catch-up limiting processes no more than the configured maximum ticks per advance and retains
		/// unprocessed ticks as backlog.
		/// </summary>
		/// <remarks>Also verifies that a subsequent advance with no additional real-time input continues draining the
		/// retained backlog without discarding pending ticks.</remarks>
		[Test]
		public void Advance_AppliesCatchUpLimitWithoutDiscardingBacklog()
		{
			SimulationClock clock = CreateClock(baseRate: 10d, tickMinutes: 1, maxTicksPerAdvance: 2);
			clock.ApplySpeedRequest(GameSpeed.x1);

			SimulationAdvanceResult first = clock.AdvanceRealSeconds(1d, null);

			Assert.AreEqual(2, first.ProcessedTickCount);
			Assert.AreEqual(8, first.PendingTickCount);
			Assert.AreEqual(2, clock.Calendar.ElapsedMinutes);

			SimulationAdvanceResult second = clock.AdvanceRealSeconds(0d, null);

			Assert.AreEqual(2, second.ProcessedTickCount);
			Assert.AreEqual(6, second.PendingTickCount);
			Assert.AreEqual(4, clock.Calendar.ElapsedMinutes);
		}

		/// <summary>
		/// Verifies that advancing the simulation by one real second raises three ticks in ascending sequence order and sets
		/// the clock tick sequence to 3.
		/// </summary>
		/// <remarks>
		/// Uses a clock configured for a base rate of 3, one-minute ticks, and a maximum of 8 ticks per
		/// advance, then confirms callback sequence values are 1, 2, and 3.
		/// </remarks>
		[Test]
		public void Advance_RaisesTickSequenceInOrder()
		{
			SimulationClock clock = CreateClock(baseRate: 3d, tickMinutes: 1, maxTicksPerAdvance: 8);
			clock.ApplySpeedRequest(GameSpeed.x1);
			List<ulong> sequence = new();

			clock.AdvanceRealSeconds(1d, tick => sequence.Add(tick.Sequence));

			CollectionAssert.AreEqual(new ulong[] { 1, 2, 3 }, sequence);
			Assert.AreEqual(3UL, clock.TickSequence);
		}

		/// <summary>
		/// Verifies that advancing the simulation throws an InvalidOperationException when the pending backlog exceeds the
		/// configured fail-safe bound.
		/// </summary>
		/// <remarks>
		/// Configures a clock with a strict tick processing limit and a maximum pending window, then
		/// advances enough real time to exceed that window.
		/// </remarks>
		[Test]
		public void Advance_ThrowsWhenPendingBacklogExceedsFailSafeBound()
		{
			SimulationClock clock = CreateClock(baseRate: 10d, tickMinutes: 1, maxTicksPerAdvance: 1, maxPendingMinutes: 5d);
			clock.ApplySpeedRequest(GameSpeed.x1);

			Assert.Throws<InvalidOperationException>(() => clock.AdvanceRealSeconds(1d, null));
		}

		/// <summary>
		/// Verifies that pause toggling is determined solely by the simulation clock state when the paused speed is requested
		/// repeatedly.
		/// </summary>
		/// <remarks>
		/// Confirms that consecutive paused requests return resumed speed first, then paused speed, based on
		/// the clock’s current state.
		/// </remarks>
		[Test]
		public void PauseToggle_UsesClockAsSingleStateAuthority()
		{
			SimulationClock clock = CreateClock();

			GameSpeed resumed = clock.ApplySpeedRequest(GameSpeed.Paused);
			GameSpeed paused = clock.ApplySpeedRequest(GameSpeed.Paused);

			Assert.AreEqual(GameSpeed.x1, resumed);
			Assert.AreEqual(GameSpeed.Paused, paused);
		}

		/// <summary>
		/// Verifies that scheduled events execute in ascending due-minute order, and for events with the same due minute, in
		/// the order they were scheduled.
		/// </summary>
		/// <remarks>
		/// Schedules callbacks across multiple due minutes, executes due items in two passes, and asserts
		/// the resulting call sequence matches due-time priority followed by insertion order.
		/// </remarks>
		[Test]
		public void ScheduledQueue_ExecutesByDueMinuteThenInsertionOrder()
		{
			ScheduledSimulationEventQueue queue = new();
			List<string> calls = new();

			queue.Schedule(5, () => calls.Add("due-5-first"));
			queue.Schedule(3, () => calls.Add("due-3"));
			queue.Schedule(5, () => calls.Add("due-5-second"));

			queue.ExecuteDue(4);
			queue.ExecuteDue(5);

			CollectionAssert.AreEqual(new[] { "due-3", "due-5-first", "due-5-second" }, calls);
		}

		/// <summary>
		/// Verifies that the dispatcher executes registered systems in ascending priority order and, for equal priorities, in
		/// registration order.
		/// </summary>
		[Test]
		public void Dispatcher_ExecutesByPriorityThenRegistrationOrder()
		{
			SimulationSystemDispatcher dispatcher = new();
			List<string> calls = new();

			dispatcher.Register(new RecordingSystem(priority: 10, "late", calls));
			dispatcher.Register(new RecordingSystem(priority: 1, "early-a", calls));
			dispatcher.Register(new RecordingSystem(priority: 1, "early-b", calls));

			dispatcher.Execute(new SimulationTick(1, 1, new SimulationCalendar(1), GameSpeed.x1));

			CollectionAssert.AreEqual(new[] { "early-a", "early-b", "late" }, calls);
		}

		/// <summary>
		/// Verifies deterministic tick execution order by advancing the clock and asserting that scheduled work runs before
		/// event-stage logic, followed by system dispatch.
		/// </summary>
		/// <remarks>
		/// Confirms that at one elapsed minute, the recorded call sequence is `scheduled`, then `event`, then `system`.
		/// </remarks>
		[Test]
		public void TickPipeline_IsDeterministic_ClockThenScheduledThenEventThenSystems()
		{
			SimulationClock clock = CreateClock(baseRate: 1d, tickMinutes: 1, maxTicksPerAdvance: 4);
			clock.ApplySpeedRequest(GameSpeed.x1);
			ScheduledSimulationEventQueue queue = new();
			SimulationSystemDispatcher dispatcher = new();
			List<string> calls = new();

			queue.Schedule(1, () => calls.Add("scheduled"));
			dispatcher.Register(new RecordingSystem(priority: 0, "system", calls));

			clock.AdvanceRealSeconds(1d, tick =>
			{
				Assert.AreEqual(1, tick.Calendar.ElapsedMinutes);
				queue.ExecuteDue(tick.Calendar.ElapsedMinutes);
				calls.Add("event");
				dispatcher.Execute(tick);
			});

			CollectionAssert.AreEqual(new[] { "scheduled", "event", "system" }, calls);
		}

		/// <summary>
		/// Creates a <see cref="SimulationClock"/> initialized at the simulation epoch with zero elapsed minutes.
		/// </summary>
		/// <param name="baseRate">Base simulation rate used by the clock.</param>
		/// <param name="tickMinutes">Number of simulated minutes represented by each tick.</param>
		/// <param name="maxTicksPerAdvance">Maximum number of ticks to process in a single advance operation.</param>
		/// <param name="maxPendingMinutes">Maximum number of pending simulated minutes the clock can accumulate.</param>
		/// <returns>A new <see cref="SimulationClock"/> configured with the specified rate, tick size, and advance limits.</returns>
		private static SimulationClock CreateClock(
			double baseRate = 60d,
			long tickMinutes = 1,
			int maxTicksPerAdvance = 10_000,
			double maxPendingMinutes = 1_000_000d)
		{
			return new SimulationClock(
				EpochUtc,
				startingElapsedMinutes: 0,
				tickMinutes,
				baseRate,
				maxTicksPerAdvance,
				maxPendingMinutes);
		}

		/// <summary>
		/// Represents a simulation system that records its execution by appending its configured name to a shared call log.
		/// </summary>
		/// <remarks>
		/// Useful for verifying simulation scheduling and execution order based on system priority.
		/// </remarks>
		private sealed class RecordingSystem : ISimulationSystem
		{
			private readonly string name;
			private readonly List<string> calls;

			/// <summary>
			/// Initializes a new instance of the RecordingSystem class with the specified priority, name, and calls.
			/// </summary>
			/// <param name="priority">The priority level assigned to the recording system.</param>
			/// <param name="name">The name of the recording system.</param>
			/// <param name="calls">The list of calls associated with the recording system.</param>
			public RecordingSystem(int priority, string name, List<string> calls)
			{
				this.Priority = priority;
				this.name = name;
				this.calls = calls;
			}

			/// <inheritdoc/>
			public int Priority { get; }

			/// <inheritdoc/>
			public void Execute(in SimulationTick tick)
			{
				this.calls.Add(this.name);
			}
		}
	}
}
