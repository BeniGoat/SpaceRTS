using System;
using System.Globalization;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Authoritative simulation timestamp represented as elapsed whole minutes from a configured epoch.
	/// </summary>
	/// <remarks>
	/// This struct is immutable and can be used as a key in dictionaries or stored in sets.
	/// </remarks>
	public readonly struct SimulationCalendar : IEquatable<SimulationCalendar>, IComparable<SimulationCalendar>
	{
		private const string CalendarFormat = "yyyy-MM-dd HH:mm";

		/// <summary>
		/// Initializes a new instance of the <see cref="SimulationCalendar"/> struct with the specified elapsed minutes.
		/// </summary>
		/// <param name="elapsedMinutes">The number of elapsed whole minutes since the configured epoch.</param>
		public SimulationCalendar(long elapsedMinutes)
		{
			this.ElapsedMinutes = elapsedMinutes;
		}

		/// <summary>
		/// Gets the authoritative elapsed simulation minutes since the configured epoch.
		/// </summary>
		public long ElapsedMinutes { get; }

		/// <summary>
		/// Returns a new <see cref="SimulationCalendar"/> instance
		/// with the specified number of minutes added to the current value.
		/// </summary>
		/// <param name="minutes">The number of minutes to add.</param>
		/// <returns>A new <see cref="SimulationCalendar"/> instance with the added minutes.</returns>
		public SimulationCalendar AddMinutes(long minutes)
		{
			return new SimulationCalendar(checked(this.ElapsedMinutes + minutes));
		}

		/// <summary>
		/// Converts this value to a Gregorian date using the provided epoch.
		/// </summary>
		/// <param name="epochUtc">The epoch to use for conversion, which must be in UTC.</param>
		/// <returns>A <see cref="DateTime"/> representing the corresponding Gregorian date and time in UTC.</returns>
		public DateTime ToDateTime(DateTime epochUtc)
		{
			if (epochUtc.Kind != DateTimeKind.Utc)
			{
				epochUtc = DateTime.SpecifyKind(epochUtc, DateTimeKind.Utc);
			}

			return epochUtc.AddMinutes(this.ElapsedMinutes);
		}

		/// <summary>
		/// Formats this value as yyyy-MM-dd HH:mm using the provided epoch.
		/// </summary>
		/// <param name="epochUtc">The epoch to use for conversion, which must be in UTC.</param>
		/// <returns>A string representing the formatted date and time.</returns>
		public string Format(DateTime epochUtc)
		{
			return this.ToDateTime(epochUtc).ToString(CalendarFormat, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Compares the current calendar with another calendar by elapsed minutes.
		/// </summary>
		/// <param name="other">The calendar to compare with the current instance.</param>
		/// <returns>A value less than zero if the current instance occurs earlier than <paramref name="other"/>; zero if they are
		/// equal; a value greater than zero if the current instance occurs later.</returns>
		public int CompareTo(SimulationCalendar other) => this.ElapsedMinutes.CompareTo(other.ElapsedMinutes);

		/// <summary>
		/// Determines whether the specified simulation calendar is equal to the current instance.
		/// </summary>
		/// <param name="other">The simulation calendar to compare with the current instance.</param>
		/// <returns><see langword="true"/> if <paramref name="other"/> has the same elapsed minutes as the current instance;
		/// otherwise, <see langword="false"/>.</returns>
		public bool Equals(SimulationCalendar other) => this.ElapsedMinutes == other.ElapsedMinutes;

		public override bool Equals(object obj) => obj is SimulationCalendar other && this.Equals(other);

		public override int GetHashCode() => this.ElapsedMinutes.GetHashCode();

		public static bool operator ==(SimulationCalendar left, SimulationCalendar right) => left.Equals(right);

		public static bool operator !=(SimulationCalendar left, SimulationCalendar right) => !left.Equals(right);

		public static bool operator <(SimulationCalendar left, SimulationCalendar right) => left.ElapsedMinutes < right.ElapsedMinutes;

		public static bool operator >(SimulationCalendar left, SimulationCalendar right) => left.ElapsedMinutes > right.ElapsedMinutes;

		public static bool operator <=(SimulationCalendar left, SimulationCalendar right) => left.ElapsedMinutes <= right.ElapsedMinutes;

		public static bool operator >=(SimulationCalendar left, SimulationCalendar right) => left.ElapsedMinutes >= right.ElapsedMinutes;
	}
}
