using System;
using System.Collections.Generic;

namespace SpaceRTS.Simulation
{
	/// <summary>
	/// Represents a queue of scheduled simulation events that can be executed at specific elapsed minutes.
	/// </summary>
	public sealed class ScheduledSimulationEventQueue : IScheduledSimulationEventQueue
	{
		private readonly List<Item> items = new();
		private ulong nextSequence;

		/// <inheritdoc/>
		public void Schedule(long dueElapsedMinutes, Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			// Create a new item with the specified due time, the next sequence number, and the provided action.
			Item item = new(dueElapsedMinutes, checked(this.nextSequence + 1UL), action);
			this.nextSequence = item.Sequence;

			// Use binary search to find the index where the new item should be inserted to maintain sorted order.
			int index = this.items.BinarySearch(item, ItemComparer.Instance);
			if (index < 0)
			{
				// If the item is not found, BinarySearch returns the bitwise complement
				// of the index of the next element that is larger than the item.
				index = ~index;
			}

			// Insert the item at the calculated index to maintain the sorted order.
			this.items.Insert(index, item);
		}

		/// <inheritdoc/>
		public void ExecuteDue(long currentElapsedMinutes)
		{
			while (this.items.Count > 0)
			{
				// Peek at the first item in the list to check if it is due for execution.
				Item item = this.items[0];
				if (item.DueElapsedMinutes > currentElapsedMinutes)
				{
					// The first item is not due yet, so we can exit the loop.
					break;
				}

				// The first item is due for execution, so we remove it from the list and execute its action.
				this.items.RemoveAt(0);
				item.Action.Invoke();
			}
		}

		/// <summary>
		/// Represents an immutable scheduled work item with a due time, a sequence number, and an action to execute.
		/// </summary>
		/// <remarks>Intended for use as a lightweight value that can be ordered by due elapsed minutes and sequence
		/// to support deterministic scheduling.</remarks>
		private readonly struct Item
		{
			/// <summary>
			/// Initializes a new instance of the item with its due time, sequence number, and callback action.
			/// </summary>
			/// <param name="dueElapsedMinutes">The elapsed minutes value indicating when the item is due.</param>
			/// <param name="sequence">The sequence number used to order items with the same due time.</param>
			/// <param name="action">The action to execute when the item is processed.</param>
			public Item(long dueElapsedMinutes, ulong sequence, Action action)
			{
				this.DueElapsedMinutes = dueElapsedMinutes;
				this.Sequence = sequence;
				this.Action = action;
			}

			/// <summary>
			/// Gets the number of elapsed minutes relative to the due time.
			/// </summary>
			public long DueElapsedMinutes { get; }

			/// <summary>
			/// Gets the sequence number.
			/// </summary>
			public ulong Sequence { get; }

			/// <summary>
			/// Gets the action to execute.
			/// </summary>
			public Action Action { get; }
		}

		/// <summary>
		/// Provides an item comparer that orders items by due elapsed minutes in ascending order
		/// and then by sequence in ascending order.
		/// </summary>
		private sealed class ItemComparer : IComparer<Item>
		{
			public static readonly ItemComparer Instance = new();

			/// <summary>
			/// Compares two items by due elapsed minutes, then by sequence when due elapsed minutes are equal.
			/// </summary>
			/// <param name="x">The first item to compare.</param>
			/// <param name="y">The second item to compare.</param>
			/// <returns>A value less than zero when x precedes y, zero when they are equal in sort order, or greater than zero when x
			/// follows y.</returns>
			public int Compare(Item x, Item y)
			{
				int dueCompare = x.DueElapsedMinutes.CompareTo(y.DueElapsedMinutes);
				if (dueCompare != 0)
				{
					return dueCompare;
				}

				return x.Sequence.CompareTo(y.Sequence);
			}
		}
	}
}