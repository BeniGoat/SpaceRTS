using System;
using System.Collections.Generic;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Dispatches simulation systems in deterministic order based on priority and registration order.
	/// </summary>
	public sealed class SimulationSystemDispatcher
	{
		private readonly List<Registration> systems = new();
		private ulong nextRegistrationOrder;

		/// <summary>
		/// Adds a simulation system to the registry, assigns it the next registration order,
		/// and keeps the registry sorted by priority then registration order.
		/// </summary>
		/// <param name="system">The simulation system to register.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="system"/> is <see langword="null"/>.</exception>
		public void Register(ISimulationSystem system)
		{
			if (system == null)
				throw new ArgumentNullException(nameof(system));

			Registration registration = new(system, checked(this.nextRegistrationOrder + 1UL));
			this.nextRegistrationOrder = registration.RegistrationOrder;
			this.systems.Add(registration);
			this.systems.Sort(RegistrationComparer.Instance);
		}

		/// <summary>
		/// Removes all registered entries that reference the specified simulation system instance.
		/// </summary>
		/// <remarks>Comparison uses reference equality, and every matching registration is removed.</remarks>
		/// <param name="system">The simulation system instance to remove from registration.</param>
		public void Unregister(ISimulationSystem system)
		{
			for (int i = this.systems.Count - 1; i >= 0; i--)
			{
				if (ReferenceEquals(this.systems[i].System, system))
				{
					this.systems.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Executes each registered system for the provided simulation tick.
		/// </summary>
		/// <remarks>Systems are executed sequentially in the order they appear in the collection.</remarks>
		/// <param name="tick">Simulation tick forwarded to each system execution.</param>
		public void Execute(in SimulationTick tick)
		{
			for (int i = 0; i < this.systems.Count; i++)
			{
				this.systems[i].System.Execute(tick);
			}
		}

		/// <summary>
		/// Represents a registration entry that pairs a simulation system with the order in which it was registered.
		/// </summary>
		private readonly struct Registration
		{
			/// <summary>
			/// Initializes a new instance of the <c>Registration</c> struct with the specified simulation system and
			/// registration order.
			/// </summary>
			/// <param name="system">Simulation system associated with the registration.</param>
			/// <param name="registrationOrder">Order used to determine registration sequence.</param>
			public Registration(ISimulationSystem system, ulong registrationOrder)
			{
				this.System = system;
				this.RegistrationOrder = registrationOrder;
			}

			/// <summary>
			/// Gets the simulation system.
			/// </summary>
			public ISimulationSystem System { get; }

			/// <summary>
			/// Gets the order in which registration occurred.
			/// </summary>
			public ulong RegistrationOrder { get; }
		}

		/// <summary>
		/// Compares Registration instances by system priority, then by registration order.
		/// </summary>
		/// <remarks>Provides deterministic ordering when priorities are equal.</remarks>
		private sealed class RegistrationComparer : IComparer<Registration>
		{
			public static readonly RegistrationComparer Instance = new();

			/// <summary>
			/// Compares two registrations by system priority, then by registration order when priorities are equal.
			/// </summary>
			/// <param name="x">The first registration to compare.</param>
			/// <param name="y">The second registration to compare.</param>
			/// <returns>A value less than zero if x precedes y, zero if they are equal in sort order, or greater than zero if x follows
			/// y.</returns>
			public int Compare(Registration x, Registration y)
			{
				int priorityCompare = x.System.Priority.CompareTo(y.System.Priority);
				if (priorityCompare != 0)
				{
					return priorityCompare;
				}

				return x.RegistrationOrder.CompareTo(y.RegistrationOrder);
			}
		}
	}
}