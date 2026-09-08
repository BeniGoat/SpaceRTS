using System;

namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Stable Unity-independent identifier for a system body.
	/// </summary>
	public readonly struct SystemBodyId : IEquatable<SystemBodyId>
	{
		/// <summary>
		/// Initializes a new instance of the <c>SystemBodyId</c> type with the specified identifier value.
		/// </summary>
		/// <param name="value">The system body identifier as a non-empty, non-whitespace string.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="value"/> is <see langword="null"/>, empty, or consists only of white-space characters.
		/// </exception>
		public SystemBodyId(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("System body identifier must be a non-empty, non-whitespace string.", nameof(value));

			this.Value = value;
		}
		
		/// <summary>
		/// Gets the system body identifier value.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Gets a value indicating whether the system body identifier is defined (non-empty, non-whitespace).
		/// </summary>
		public bool IsDefined => !string.IsNullOrWhiteSpace(this.Value);

		public bool Equals(SystemBodyId other) =>
			string.Equals(this.Value, other.Value, StringComparison.Ordinal);

		public override bool Equals(object obj) =>
			obj is SystemBodyId other && this.Equals(other);

		public override int GetHashCode() =>
			StringComparer.Ordinal.GetHashCode(this.Value ?? string.Empty);

		public override string ToString() => this.Value ?? string.Empty;

		public static bool operator ==(SystemBodyId left, SystemBodyId right) => left.Equals(right);

		public static bool operator !=(SystemBodyId left, SystemBodyId right) => !left.Equals(right);
	}
}
