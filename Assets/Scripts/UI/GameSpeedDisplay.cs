using SpaceRTS.Managers;
using SpaceRTS.Managers.Enums;
using TMPro;
using UnityEngine;

namespace SpaceRTS.UI
{
	/// <summary>
	/// Displays the current game speed on the UI. It listens for changes in
	/// game speed from the TimeScaleManager and updates the text accordingly.
	/// </summary>
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class GameSpeedDisplay : MonoBehaviour
	{
		private TextMeshProUGUI speedText;

		private void Awake()
		{
			this.speedText = this.GetComponent<TextMeshProUGUI>();
		}

		private void OnEnable()
		{
			TimeScaleManager.OnSpeedChanged += this.UpdateDisplay;
		}

		private void OnDisable()
		{
			TimeScaleManager.OnSpeedChanged -= this.UpdateDisplay;
		}

		/// <summary>
		/// Updates the display text based on the current game speed. If the game is paused, it shows "PAUSED".
		/// If the speed is x1, it shows an empty string. For other speeds, it displays the speed as a string.
		/// </summary>
		/// <param name="speed">The current game speed.</param>
		private void UpdateDisplay(GameSpeed speed)
		{
			this.speedText.text = speed switch
			{
				GameSpeed.Paused => "PAUSED",
				GameSpeed.x1 => string.Empty,
				_ => speed.ToString()
			};
		}
	}
}