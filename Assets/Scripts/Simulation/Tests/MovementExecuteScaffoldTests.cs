using NUnit.Framework;
using SpaceRTS.Models;
using System.Collections.Generic;

namespace SpaceRTS.Simulation.Tests
{
	/// <summary>
	/// Scaffold tests for tick-execute movement loop behavior.
	/// Tests now target the shared production helper.
	/// </summary>
	public class MovementExecuteScaffoldTests
	{
		[Test]
		public void Execute_RemovesNullOrMissingEntries()
		{
			List<FakeTravelShip> ships = new()
			{
				new FakeTravelShip(),
				null,
				new FakeTravelShip() { CurrentSystemBody = null }, // This ship is considered "missing" because its CurrentSystemBody is null
				new FakeTravelShip()
			};

			MovementExecuteCore.Execute(
				ships,
				1f,
				s => s == null || s.CurrentSystemBody == null,
				s => s.HasArrived,
				s => s.CompleteTravel(),
				(s, dt) => s.ProcessTravelTick(dt));

			Assert.AreEqual(2, ships.Count);
			Assert.IsNotNull(ships[0]);
			Assert.IsNotNull(ships[1]);
		}

		[Test]
		public void Execute_ArrivedAndCompleted_IsRemoved()
		{
			FakeTravelShip arrived = new() { HasArrived = true, CompleteTravelResult = true };
			List<FakeTravelShip> ships = new() { arrived };

			MovementExecuteCore.Execute(
				ships,
				1f,
				s => s == null || s.CurrentSystemBody == null,
				s => s.HasArrived,
				s => s.CompleteTravel(),
				(s, dt) => s.ProcessTravelTick(dt));

			Assert.AreEqual(0, ships.Count);
			Assert.AreEqual(1, arrived.CompleteTravelCalls);
			Assert.AreEqual(0, arrived.ProcessTickCalls);
		}

		[Test]
		public void Execute_ArrivedButNotCompleted_IsRetained()
		{
			FakeTravelShip arrived = new() { HasArrived = true, CompleteTravelResult = false };
			List<FakeTravelShip> ships = new() { arrived };

			MovementExecuteCore.Execute(
				ships,
				1f,
				s => s == null || s.CurrentSystemBody == null,
				s => s.HasArrived,
				s => s.CompleteTravel(),
				(s, dt) => s.ProcessTravelTick(dt));

			Assert.AreEqual(1, ships.Count);
			Assert.AreEqual(1, arrived.CompleteTravelCalls);
			Assert.AreEqual(0, arrived.ProcessTickCalls);
		}

		[Test]
		public void Execute_InTransit_ProcessesTickWithProvidedStep()
		{
			FakeTravelShip moving = new() { HasArrived = false };
			List<FakeTravelShip> ships = new() { moving };

			MovementExecuteCore.Execute(
				ships,
				0.5f,
				s => s == null || s.CurrentSystemBody == null,
				s => s.HasArrived,
				s => s.CompleteTravel(),
				(s, dt) => s.ProcessTravelTick(dt));

			Assert.AreEqual(1, ships.Count);
			Assert.AreEqual(0, moving.CompleteTravelCalls);
			Assert.AreEqual(1, moving.ProcessTickCalls);
			Assert.AreEqual(0.5f, moving.LastTickStepSeconds, 0.00001f);
		}

		private sealed class FakeTravelShip
		{
			public bool HasArrived { get; set; }
			public bool CompleteTravelResult { get; set; }

			public int CompleteTravelCalls { get; private set; }
			public int ProcessTickCalls { get; private set; }
			public float LastTickStepSeconds { get; private set; }
			public SystemBody CurrentSystemBody { get; set; }

			public bool CompleteTravel()
			{
				this.CompleteTravelCalls++;
				return this.CompleteTravelResult;
			}

			public void ProcessTravelTick(float tickStepSeconds)
			{
				this.ProcessTickCalls++;
				this.LastTickStepSeconds = tickStepSeconds;
			}
		}
	}
}