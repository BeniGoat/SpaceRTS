using System;
using System.Collections.Generic;

namespace SpaceRTS.Services
{
	/// <summary>
	/// A simple service locator for resolving dependencies without requiring
	/// all components to live on the same GameObject.
	/// </summary>
	public static class ServiceLocator
	{
		private static readonly Dictionary<Type, object> services = new();

		/// <summary>
		/// Registers a service instance for the specified type.
		/// </summary>
		public static void Register<T>(T service) where T : class
		{
			services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
		}

		/// <summary>
		/// Resolves a registered service by type.
		/// </summary>
		public static T Get<T>() where T : class
		{
			if (services.TryGetValue(typeof(T), out var service))
			{
				return (T)service;
			}

			throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
		}

		/// <summary>
		/// Tries to resolve a registered service. Returns false if not found.
		/// </summary>
		public static bool TryGet<T>(out T service) where T : class
		{
			if (services.TryGetValue(typeof(T), out var obj))
			{
				service = (T)obj;
				return true;
			}

			service = null;
			return false;
		}

		/// <summary>
		/// Clears all registered services. Call on scene unload if needed.
		/// </summary>
		public static void Clear()
		{
			services.Clear();
		}
	}
}