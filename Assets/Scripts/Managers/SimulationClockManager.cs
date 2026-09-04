using System;
using SpaceRTS.Events;
using SpaceRTS.Simulation;
using SpaceRTS.Services;
using UnityEngine;

namespace SpaceRTS.Managers
{
	/// <summary>
	/// Unity bridge that supplies real elapsed time to the authoritative pure simulation clock.
	/// Deterministic tick-order pipeline per processed tick:
	/// 1) clock advances time,
	/// 2) scheduled events due at the new time execute,
	/// 3) SimulationTickEvent is published,
	/// 4) simulation systems execute by priority then registration order.
	/// </summary>
	[DefaultExecutionOrder(-500)]
	public class SimulationClockManager : MonoBehaviour
	{
		[Header("Epoch")]
		[SerializeField] private int epochYear = 2050;
		[SerializeField] private int epochMonth = 1;
		[SerializeField] private int epochDay = 1;
		[SerializeField] private int epochHour = 0;
		[SerializeField] private int epochMinute = 0;

		[Header("Simulation Time")]
		[SerializeField] private long startingElapsedMinutes = 0;
		[SerializeField, Min(1)] private long tickMinutes = 1;
		[SerializeField, Min(0.0001f)] private float baseSimMinutesPerRealSecond = 1f;
		[SerializeField, Min(1)] private int maxTicksPerFrame = 256;
		[SerializeField, Min(1f)] private float maxPendingSimulationMinutes = 1_000_000f;

		private ISimulationClock clock;
		private IScheduledSimulationEventQueue scheduledEvents;
		private SimulationSystemDispatcher simulationSystems;

		private void Awake()
		{
			// Initialize the simulation clock and register it with the service locator.
			this.clock = new SimulationClock(
				this.CreateEpochUtc(),
				this.startingElapsedMinutes,
				this.tickMinutes,
				this.baseSimMinutesPerRealSecond,
				this.maxTicksPerFrame,
				this.maxPendingSimulationMinutes);
			this.scheduledEvents = new ScheduledSimulationEventQueue();
			this.simulationSystems = new SimulationSystemDispatcher();

			ServiceLocator.Register(this.clock);
			ServiceLocator.Register(this);
		}

		private void Update()
		{
			// Advance the simulation clock based on real elapsed time since the last frame.
			this.clock.AdvanceRealSeconds(Time.unscaledDeltaTime, this.ProcessTick);
		}

		/// <summary>
		/// Schedules an action to be executed at a specific simulation time in elapsed minutes.
		/// </summary>
		/// <param name="dueElapsedMinutes">The simulation time in elapsed minutes when the action should be executed.</param>
		/// <param name="action">The action to execute.</param>
		public void ScheduleAt(long dueElapsedMinutes, Action action)
		{
			this.scheduledEvents.Schedule(dueElapsedMinutes, action);
		}

		/// <summary>
		/// Registers a simulation system for deterministic post-tick execution.
		/// </summary>
		/// <param name="system">The simulation system to register.</param>
		public void RegisterSystem(ISimulationSystem system)
		{
			this.simulationSystems.Register(system);
		}

		/// <summary>
		/// Removes the specified simulation system from the registered systems collection.
		/// </summary>
		/// <param name="system">The simulation system to unregister.</param>
		public void UnregisterSystem(ISimulationSystem system)
		{
			this.simulationSystems.Unregister(system);
		}

		/// <summary>
		/// Processes a simulation tick by executing due scheduled events,
		/// publishing a tick event, and executing registered simulation systems.
		/// </summary>
		/// <param name="tick">The simulation tick to process.</param>
		private void ProcessTick(SimulationTick tick)
		{
			this.scheduledEvents.ExecuteDue(tick.Calendar.ElapsedMinutes);
			EventBus.Publish(new SimulationTickEvent { Tick = tick });
			this.simulationSystems.Execute(tick);
		}

		/// <summary>
		/// Creates a UTC <see cref="DateTime"/> using the configured epoch date and time components.
		/// </summary>
		/// <returns>A <see cref="DateTime"/> representing the configured epoch in Coordinated Universal Time (UTC), with seconds set
		/// to 0.</returns>
		private DateTime CreateEpochUtc()
		{
			return new DateTime(
				this.epochYear,
				this.epochMonth,
				this.epochDay,
				this.epochHour,
				this.epochMinute,
				0,
				DateTimeKind.Utc);
		}
	}
}
