using System;
using System.Collections.Generic;

namespace SpaceRTS.Services
{
	/// <summary>
	/// A lightweight publish/subscribe event bus that decouples event publishers from subscribers.
	/// Publishers and subscribers only need to know about the event type, not each other.
	/// </summary>
	public static class EventBus
	{
		/// <summary>
		/// A dictionary mapping event types to their corresponding delegates (handlers).
		/// </summary>
		private static readonly Dictionary<Type, Delegate> handlers = new();

		/// <summary>
		/// Subscribes a handler to events of type T.
		/// </summary>
		public static void Subscribe<T>(Action<T> handler) where T : struct
		{
			var type = typeof(T);
			if (handlers.TryGetValue(type, out var existing))
			{
				handlers[type] = Delegate.Combine(existing, handler);
			}
			else
			{
				handlers[type] = handler;
			}
		}

		/// <summary>
		/// Unsubscribes a handler from events of type T.
		/// </summary>
		public static void Unsubscribe<T>(Action<T> handler) where T : struct
		{
			var type = typeof(T);
			if (handlers.TryGetValue(type, out var existing))
			{
				var result = Delegate.Remove(existing, handler);
				if (result == null)
					handlers.Remove(type);
				else
					handlers[type] = result;
			}
		}

		/// <summary>
		/// Publishes an event of type T to all subscribers.
		/// </summary>
		public static void Publish<T>(T evt) where T : struct
		{
			if (handlers.TryGetValue(typeof(T), out var existing))
			{
				((Action<T>)existing).Invoke(evt);
			}
		}

		/// <summary>
		/// Clears all subscriptions. Call on scene unload if needed.
		/// </summary>
		public static void Clear()
		{
			handlers.Clear();
		}
	}
}