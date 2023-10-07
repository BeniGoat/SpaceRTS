using SpaceRTS.Managers.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public partial class GameManager : MonoBehaviour
    {
        public Canvas UI;
        public Models.System systemPrefab;
        //public CameraRig cameraRig;
        public CameraManager cameraManager;
        public SelectionManager objectSelector;

        public GameSpeed? currentGameSpeed;
        public CameraMode cameraMode = CameraMode.Perspective;

        private bool isPaused;
        private readonly Dictionary<KeyCode, GameSpeed> gameSpeeds = new Dictionary<KeyCode, GameSpeed>()
        {
            { KeyCode.Space, GameSpeed.Paused },
            { KeyCode.Alpha1, GameSpeed.x1 },
            { KeyCode.Alpha2, GameSpeed.x2 },
            { KeyCode.Alpha3, GameSpeed.x5 },
            { KeyCode.Alpha4, GameSpeed.x10 }
        };

        private TextMeshProUGUI textMesh;

        private void Awake()
        {
            this.textMesh = this.UI.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            // Instantiate the system
            Models.System system = Instantiate(this.systemPrefab);

            // Initialise any other gameplay mechanics or systems here
            //this.cameraRig.Range = system.Size;
            this.cameraManager.SetCamera(this.cameraMode, system.Size);

            // Initialise game speed
            this.SetTimeScale(GameSpeed.Paused);
            this.isPaused = true;
        }

        private void Update()
        {
            this.HandleGameSpeedChange();
        }

        private void HandleGameSpeedChange()
        {
            // Listen for any key presses related to changing the game speed
            foreach (KeyValuePair<KeyCode, GameSpeed> gameSpeedMapping in this.gameSpeeds)
            {
                if (Input.GetKeyDown(gameSpeedMapping.Key))
                {
                    GameSpeed speedToSet = gameSpeedMapping.Value;

                    // If the pause button has been pressed
                    if (speedToSet == GameSpeed.Paused)
                    {
                        // Check if the game is already paused
                        if (this.isPaused)
                        {
                            // If so, resume the game at the stored game speed, or at 1x speed if first time unpausing
                            speedToSet = this.currentGameSpeed ?? GameSpeed.x1;
                        }

                        // Toggle the paused flag
                        this.isPaused = !this.isPaused;
                    }
                    else
                    {
                        // If not, set the game speed to the value and cache it
                        this.currentGameSpeed = speedToSet;
                    }

                    this.SetTimeScale(speedToSet);
                    break;
                }
            }
        }

        private void SetTimeScale(GameSpeed speed)
        {
            Time.timeScale = (int)speed / 5f;
            this.textMesh.text = speed == GameSpeed.x1
                ? string.Empty
                : speed.ToString();
        }
    }
}