using NUnit.Framework;

namespace SpaceRTS.Simulation.Tests
{
	/// <summary>
	/// Scaffold tests for deterministic, tick-driven ship travel progression.
	/// These tests validate the core stepping model independent of Unity transforms.
	/// </summary>
	public class TickDrivenShipMovementScaffoldTests
	{
		[Test]
		public void Travel_EquivalentTickBatches_ProduceSameRemainingDistance()
		{
			// Arrange
			const float startingDistance = 100f;
			const float travelSpeedUnitsPerRealSecond = 5f;
			const long tickMinutes = 1;
			const double baseSimMinutesPerRealSecond = 60d;

			// A: 60 ticks of 1 minute each (represents 1 real second total at x1)
			float remainingA = startingDistance;
			for (int i = 0; i < 60; i++)
			{
				remainingA = TickDrivenTravelStepper.Advance(
					remainingA,
					travelSpeedUnitsPerRealSecond,
					tickMinutes,
					baseSimMinutesPerRealSecond);
			}

			// B: 30 ticks, then 30 ticks (same total simulated minutes)
			float remainingB = startingDistance;
			for (int i = 0; i < 30; i++)
			{
				remainingB = TickDrivenTravelStepper.Advance(
					remainingB,
					travelSpeedUnitsPerRealSecond,
					tickMinutes,
					baseSimMinutesPerRealSecond);
			}
			for (int i = 0; i < 30; i++)
			{
				remainingB = TickDrivenTravelStepper.Advance(
					remainingB,
					travelSpeedUnitsPerRealSecond,
					tickMinutes,
					baseSimMinutesPerRealSecond);
			}

			// Assert
			Assert.AreEqual(remainingA, remainingB, 0.00001f);
		}

		[Test]
		public void Travel_DoesNotOvershootDestination()
		{
			// Arrange
			const float startingDistance = 0.1f;
			const float travelSpeedUnitsPerRealSecond = 5f;
			const long tickMinutes = 1;
			const double baseSimMinutesPerRealSecond = 60d;

			// Act
			float remaining = TickDrivenTravelStepper.Advance(
				startingDistance,
				travelSpeedUnitsPerRealSecond,
				tickMinutes,
				baseSimMinutesPerRealSecond);

			// Assert
			Assert.AreEqual(0f, remaining, 0.00001f);
		}

		/// <summary>
		/// Minimal deterministic stepping helper used only by scaffold tests.
		/// </summary>
		private static class TickDrivenTravelStepper
		{
			public static float Advance(
				float remainingDistance,
				float speedUnitsPerRealSecond,
				long tickMinutes,
				double baseSimMinutesPerRealSecond)
			{
				float realSecondsForTick =
					(float)(tickMinutes / baseSimMinutesPerRealSecond);

				float deltaDistance = speedUnitsPerRealSecond * realSecondsForTick;
				float nextRemaining = remainingDistance - deltaDistance;

				return nextRemaining <= 0f ? 0f : nextRemaining;
			}
		}
	}
}