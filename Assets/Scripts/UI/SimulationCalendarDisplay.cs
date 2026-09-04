using SpaceRTS.Services;
using SpaceRTS.Simulation;
using TMPro;
using UnityEngine;

namespace SpaceRTS.UI
{
	/// <summary>
	/// Displays the authoritative simulation calendar timestamp.
	/// </summary>
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SimulationCalendarDisplay : MonoBehaviour
	{
		private TextMeshProUGUI calendarText;
		private ISimulationClock simulationClock;
		private long? lastRenderedElapsedMinutes;

		private void Awake()
		{
			this.calendarText = this.GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			// Attempt to resolve the simulation clock from the service locator if not already resolved.
			if (!this.TryResolveClock())
				return;

			// Update the displayed calendar text only if the elapsed minutes have changed since the last render.
			long elapsedMinutes = this.simulationClock.Calendar.ElapsedMinutes;
			if (this.lastRenderedElapsedMinutes.HasValue && this.lastRenderedElapsedMinutes.Value == elapsedMinutes)
				return;

			// Update the text to reflect the current simulation calendar.
			this.lastRenderedElapsedMinutes = elapsedMinutes;
			this.calendarText.text = this.simulationClock.Calendar.Format(this.simulationClock.EpochUtc);
		}

		/// <summary>
		/// Attempts to resolve and cache the simulation clock instance.
		/// </summary>
		/// <remarks>Caches the resolved instance in `simulationClock` for subsequent calls.</remarks>
		/// <returns>true if a simulation clock instance is already cached or is successfully resolved from the service locator;
		/// otherwise, false.</returns>
		private bool TryResolveClock()
		{
			if (this.simulationClock != null)
				return true;

			return ServiceLocator.TryGet(out this.simulationClock);
		}
	}
}
