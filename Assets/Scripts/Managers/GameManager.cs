using SpaceRTS.Spawners;
using TMPro;
using UnityEngine;

namespace SpaceRTS.Managers
{
        public class GameManager : MonoBehaviour
        {
                public static bool isPaused = false;
                public int numPlanets = 5;

                public Canvas UI;
                private TextMeshProUGUI textMesh;
                private PlanetSpawner planetSpawner;

                public GameObject sunPrefab;

                private void Awake()
                {
                        this.textMesh = this.UI.GetComponentInChildren<TextMeshProUGUI>();
                        this.planetSpawner = this.GetComponent<PlanetSpawner>();

                        // Instantiate the sun and planet objects
                        GameObject sun = Instantiate(this.sunPrefab);

                        this.planetSpawner.SpawnPlanets(sun, this.numPlanets);

                        // Initialize any other gameplay mechanics or systems here
                }

                private void Update()
                {
                        // Check for pause input
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                                this.TogglePause();
                        }
                }

                private void TogglePause()
                {
                        isPaused = !isPaused;

                        if (isPaused)
                        {
                                Time.timeScale = 0;
                                this.textMesh.text = "Paused";
                        }
                        else
                        {
                                Time.timeScale = 1;
                                this.textMesh.text = string.Empty;
                        }
                }
        }
}