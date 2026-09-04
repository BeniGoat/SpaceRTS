using SpaceRTS.Managers.Enums;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Represents one processed deterministic simulation tick.
	/// </summary>
	public readonly struct SimulationTick
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SimulationTick"/> type.
		/// </summary>
		/// <param name="sequence">The monotonically increasing sequence number for the tick.</param>
		/// <param name="tickMinutes">The simulation time, in minutes, represented by the tick.</param>
		/// <param name="calendar">The simulation calendar state at the tick.</param>
		/// <param name="speed">The game speed in effect for the tick.</param>
		public SimulationTick(ulong sequence, long tickMinutes, SimulationCalendar calendar, GameSpeed speed)
		{
			this.Sequence = sequence;
			this.TickMinutes = tickMinutes;
			this.Calendar = calendar;
			this.Speed = speed;
		}

		/// <summary>
		/// Gets the sequence number.
		/// </summary>
		public ulong Sequence { get; }

		/// <summary>
		/// Gets the number of elapsed minutes represented by the tick value.
		/// </summary>
		public long TickMinutes { get; }

		/// <summary>
		/// Gets the simulation calendar.
		/// </summary>
		public SimulationCalendar Calendar { get; }

		/// <summary>
		/// Gets the current game speed.
		/// </summary>
		public GameSpeed Speed { get; }
	}
}
