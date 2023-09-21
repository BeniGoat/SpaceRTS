using SpaceRTS.Camera;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class GameManager : MonoBehaviour
    {
        public bool isPaused = false;

        private GameSpeed gameSpeed;
        private readonly Dictionary<KeyCode, GameSpeed> gameSpeeds = new Dictionary<KeyCode, GameSpeed>()
        {
            { KeyCode.Alpha1, GameSpeed.x1 },
            { KeyCode.Alpha2, GameSpeed.x2 },
            { KeyCode.Alpha3, GameSpeed.x5 },
            { KeyCode.Alpha4, GameSpeed.x10 }
        };

        public Canvas UI;
        private TextMeshProUGUI textMesh;

        public Models.System systemPrefab;
        public CameraRig cameraRig;
        public SelectionManager objectSelector; 

        private void Awake()
        {
            this.textMesh = this.UI.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            this.SetGameSpeed(GameSpeed.x1);

            // Instantiate the system
            Models.System system = Instantiate(this.systemPrefab);

            // Initialize any other gameplay mechanics or systems here
            this.cameraRig.Range = system.Size;
        }

        private void Update()
        {
            // Check for pause input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.TogglePause();
            }

            foreach (KeyValuePair<KeyCode, GameSpeed> gameSpeedMapping in this.gameSpeeds)
            {
                if (Input.GetKeyDown(gameSpeedMapping.Key))
                {
                    this.SetGameSpeed(gameSpeedMapping.Value);
                    break;
                }
            }
        }

        private void SetGameSpeed(GameSpeed speed)
        {
            this.gameSpeed = speed;
            if (!this.isPaused)
            {
                this.SetTimeScale(speed);
            }
        }

        private void SetTimeScale(GameSpeed speed)
        {
            Time.timeScale = (int)speed / 5f;
            this.textMesh.text = speed == GameSpeed.x1
                ? string.Empty
                : speed.ToString();
        }

        private void TogglePause()
        {
            this.isPaused = !this.isPaused;

            var speed = this.isPaused
                ? GameSpeed.Paused
                : this.gameSpeed;

            this.SetTimeScale(speed);
        }
    }

    public enum GameSpeed
    {
        Paused = 0,
        x1 = 1,
        x2 = 2,
        x5 = 5,
        x10 = 10
    }
}