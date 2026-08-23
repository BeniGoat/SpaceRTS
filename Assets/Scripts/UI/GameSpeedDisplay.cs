using SpaceRTS.Events;
using SpaceRTS.Managers.Enums;
using SpaceRTS.Services;
using TMPro;
using UnityEngine;

namespace SpaceRTS.UI
{
    /// <summary>
    /// Displays the current game speed on the UI.
	/// It listens for SpeedChangedEvent events and updates the text accordingly.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
	public class GameSpeedDisplay : MonoBehaviour
	{
		private TextMeshProUGUI speedText;

        private void Awake()
		{
            ServiceLocator.Register(this);
            this.speedText = this.GetComponent<TextMeshProUGUI>();
		}

        private void Start()
        {
            this.UpdateDisplay(new SpeedChangedEvent { Speed = GameSpeed.Paused });
        }

        private void OnEnable()
		{
            EventBus.Subscribe<SpeedChangedEvent>(this.UpdateDisplay);
		}

		private void OnDisable()
		{
            EventBus.Unsubscribe<SpeedChangedEvent>(this.UpdateDisplay);
		}

		/// <summary>
		/// Updates the display text based on the current game speed. If the game is paused, it shows "PAUSED".
		/// If the speed is x1, it shows an empty string. For other speeds, it displays the speed as a string.
		/// </summary>
		/// <param name="evt">The speed changed event.</param>
		private void UpdateDisplay(SpeedChangedEvent evt)
		{
			this.speedText.text = evt.Speed switch
			{
				GameSpeed.Paused => "PAUSED",
				GameSpeed.x1 => string.Empty,
				_ => evt.Speed.ToString()
			};
		}
	}
}