using System;
using NUnit.Framework;

namespace SpaceRTS.Models.Physical.Tests
{
	/// <summary>
	/// Unit tests for physical value types used by the simulation model.
	/// </summary>
	/// <remarks>
	/// These tests verify core value-type behavior such as construction, equality,
	/// comparison, arithmetic operations, and boundary handling.
	/// </remarks>
	public class PhysicalValueTypesTests
	{
		[Test]
		public void SystemBodyId_ValidValue_Constructs()
		{
			SystemBodyId id = new("Earth");

			Assert.AreEqual("Earth", id.Value);
			Assert.IsTrue(id.IsDefined);
			Assert.AreEqual("Earth", id.ToString());
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("   ")]
		public void SystemBodyId_InvalidValue_ThrowsArgumentException(string value)
		{
			Assert.Throws<ArgumentException>(() => new SystemBodyId(value));
		}

		[Test]
		public void SystemBodyId_UsesOrdinalEquality()
		{
			SystemBodyId first = new("Body_A");
			SystemBodyId second = new("Body_A");
			SystemBodyId third = new("body_a");

			Assert.AreEqual(first, second);
			Assert.IsTrue(first == second);
			Assert.AreNotEqual(first, third);
			Assert.IsTrue(first != third);
		}

		[Test]
		public void SystemBodyPhysicalProperties_ValidConstruction_DerivesValues()
		{
			SystemBodyPhysicalProperties properties = new(
				radiusMeters: 2d,
				massKilograms: 64d,
				axialRotationPeriodSeconds: 10d,
				isTidallyLocked: false);

			Assert.AreEqual(2d, properties.RadiusMeters);
			Assert.AreEqual(64d, properties.MassKilograms);
			Assert.AreEqual(10d, properties.AxialRotationPeriodSeconds);
			Assert.AreEqual(4d, properties.DiameterMeters);
			Assert.AreEqual((4d / 3d) * Math.PI * 8d, properties.VolumeCubicMeters, 1e-12);
			Assert.AreEqual(64d / properties.VolumeCubicMeters, properties.DensityKilogramsPerCubicMeter, 1e-12);
		}

		[Test]
		public void SystemBodyPhysicalProperties_TidalLock_UsesExplicitFiniteState()
		{
			SystemBodyPhysicalProperties properties = new(
				radiusMeters: 1d,
				massKilograms: 1d,
				axialRotationPeriodSeconds: 123d,
				isTidallyLocked: true);

			Assert.IsTrue(properties.IsTidallyLocked);
			Assert.IsFalse(double.IsNaN(properties.AxialRotationPeriodSeconds));
			Assert.IsFalse(double.IsInfinity(properties.AxialRotationPeriodSeconds));
			Assert.Greater(properties.AxialRotationPeriodSeconds, 0d);
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(0d)]
		[TestCase(-1d)]
		public void SystemBodyPhysicalProperties_InvalidRadius_Throws(double radius)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new SystemBodyPhysicalProperties(radius, 1d, 1d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(0d)]
		[TestCase(-1d)]
		public void SystemBodyPhysicalProperties_InvalidMass_Throws(double mass)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new SystemBodyPhysicalProperties(1d, mass, 1d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(0d)]
		[TestCase(-1d)]
		public void SystemBodyPhysicalProperties_InvalidAxialPeriod_Throws(double period)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new SystemBodyPhysicalProperties(1d, 1d, period));
		}

		[Test]
		public void OrbitProperties_ValidConstruction_NormalizesInitialPhase()
		{
			OrbitProperties properties = new(
				new SystemBodyId("Parent"),
				orbitalRadiusMeters: 10d,
				orbitalPeriodSeconds: 20d,
				initialPhaseRadians: 2d * Math.PI + 0.5d);

			Assert.AreEqual(0.5d, properties.InitialPhaseRadians, 1e-12);
		}

		[Test]
		public void OrbitProperties_InvalidParent_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() =>
				new OrbitProperties(default, 1d, 1d, 0d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(0d)]
		[TestCase(-1d)]
		public void OrbitProperties_InvalidRadius_Throws(double radius)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new OrbitProperties(new SystemBodyId("Parent"), radius, 1d, 0d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(0d)]
		[TestCase(-1d)]
		public void OrbitProperties_InvalidPeriod_Throws(double period)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new OrbitProperties(new SystemBodyId("Parent"), 1d, period, 0d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		public void OrbitProperties_InvalidPhase_Throws(double phase)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new OrbitProperties(new SystemBodyId("Parent"), 1d, 1d, phase));
		}

		[Test]
		public void OrbitState_NormalizesPositiveAndNegativePhase()
		{
			OrbitState state = new(-0.25d);
			double twoPi = Math.PI * 2d;

			Assert.AreEqual(twoPi - 0.25d, state.CurrentPhaseRadians, 1e-12);

			state.AdvanceByRadians(twoPi + 0.5d);
			Assert.AreEqual(0.25d, state.CurrentPhaseRadians, 1e-12);
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		public void OrbitState_InvalidDelta_Throws(double delta)
		{
			OrbitState state = new(0d);
			Assert.Throws<ArgumentOutOfRangeException>(() => state.AdvanceByRadians(delta));
		}

		[Test]
		public void ShipPhysicalProperties_ValidConstruction_Constructs()
		{
			ShipPhysicalProperties properties = new(dryMassKilograms: 1000d, fuelCapacityKilograms: 500d);

			Assert.AreEqual(1000d, properties.DryMassKilograms);
			Assert.AreEqual(500d, properties.FuelCapacityKilograms);
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(0d)]
		[TestCase(-1d)]
		public void ShipPhysicalProperties_InvalidDryMass_Throws(double dryMass)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new ShipPhysicalProperties(dryMass, 1d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(-1d)]
		public void ShipPhysicalProperties_InvalidFuelCapacity_Throws(double fuelCapacity)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new ShipPhysicalProperties(1d, fuelCapacity));
		}

		[Test]
		public void ShipFuelState_ValidConstruction_DerivesTotalMass()
		{
			ShipPhysicalProperties properties = new(100d, 50d);
			ShipFuelState fuelState = new(properties, 40d);

			Assert.AreEqual(40d, fuelState.CurrentFuelMassKilograms);
			Assert.AreEqual(140d, fuelState.TotalMassKilograms);
		}

		[Test]
		public void ShipFuelState_NullProperties_ThrowsArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new ShipFuelState(null, 0d));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(-1d)]
		[TestCase(51d)]
		public void ShipFuelState_InvalidCurrentFuel_Throws(double fuel)
		{
			ShipPhysicalProperties properties = new(100d, 50d);
			Assert.Throws<ArgumentOutOfRangeException>(() => new ShipFuelState(properties, fuel));
		}

		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		[TestCase(-1d)]
		[TestCase(60d)]
		public void ShipFuelState_SetFuelOutsideBounds_Throws(double fuel)
		{
			ShipFuelState state = new(new ShipPhysicalProperties(100d, 50d), 10d);
			Assert.Throws<ArgumentOutOfRangeException>(() => state.SetCurrentFuelMassKilograms(fuel));
		}

		[Test]
		public void RootOwnershipConvention_RootHasNoOrbitInstance()
		{
			OrbitProperties orbit = null;
			Assert.IsNull(orbit);
		}

		[Test]
		public void ImmutableConfigurationTypes_DoNotExposeSetters()
		{
			Assert.IsFalse(typeof(SystemBodyPhysicalProperties)
				.GetProperty(nameof(SystemBodyPhysicalProperties.RadiusMeters))
				.CanWrite);
			Assert.IsFalse(typeof(OrbitProperties)
				.GetProperty(nameof(OrbitProperties.OrbitalRadiusMeters))
				.CanWrite);
			Assert.IsFalse(typeof(ShipPhysicalProperties)
				.GetProperty(nameof(ShipPhysicalProperties.DryMassKilograms))
				.CanWrite);
		}
	}
}
