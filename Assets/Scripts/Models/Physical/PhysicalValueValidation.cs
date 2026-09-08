using System;

namespace SpaceRTS.Models.Physical
{
	/// <summary>
	/// Provides helper methods for validating numeric physical values and normalizing angles in radians.
	/// </summary>
	/// <remarks>Includes guards for finite, positive, non-negative, and inclusive-range constraints, and throws
	/// argument exceptions when validation fails.</remarks>
	internal static class PhysicalValueValidation
	{
		/// <summary>
		/// Ensures that the specified value is a finite number.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <returns>The validated value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not finite.</exception>
		public static double RequireFinite(double value, string paramName)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
				throw new ArgumentOutOfRangeException(paramName, value, "Value must be finite.");

			return value;
		}

		/// <summary>
		/// Ensures that the specified value is a finite number greater than zero.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <returns>The validated value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not finite or not greater than zero.</exception>
		public static double RequirePositive(double value, string paramName)
		{
			RequireFinite(value, paramName);
			if (value <= 0d)
				throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero.");

			return value;
		}
		
		/// <summary>
		/// Ensures that the specified value is a finite number greater than or equal to zero.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <returns>The validated value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not finite or negative.</exception>
		public static double RequireNonNegative(double value, string paramName)
		{
			RequireFinite(value, paramName);
			if (value < 0d)
				throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than or equal to zero.");

			return value;
		}
		
		/// <summary>
		/// Ensures that the specified value is within the inclusive range defined by <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="minInclusive">The minimum inclusive bound.</param>
		/// <param name="maxInclusive">The maximum inclusive bound.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <returns>The validated value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not finite or outside the inclusive range.</exception>
		/// <exception cref="ArgumentException">Thrown when the minimum bound is greater than the maximum bound.</exception>
		public static double RequireRangeInclusive(double value, double minInclusive, double maxInclusive, string paramName)
		{
			RequireFinite(value, paramName);
			RequireFinite(minInclusive, nameof(minInclusive));
			RequireFinite(maxInclusive, nameof(maxInclusive));

			if (minInclusive > maxInclusive)
				throw new ArgumentException("Minimum bound cannot be greater than maximum bound.", nameof(minInclusive));

			if (value < minInclusive || value > maxInclusive)
				throw new ArgumentOutOfRangeException(paramName, value, "Value is outside the allowed inclusive range.");

			return value;
		}
		
		/// <summary>
		/// Normalizes the specified angle in radians to the range [0, 2π).
		/// </summary>
		/// <param name="radians">The angle in radians to normalize.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <returns>The normalized angle in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the angle is not finite.</exception>
		public static double NormalizeRadians(double radians, string paramName)
		{
			RequireFinite(radians, paramName);

			double twoPi = Math.PI * 2d;
			double normalized = radians % twoPi;
			if (normalized < 0d)
				normalized += twoPi;

			return normalized;
		}
	}
}
