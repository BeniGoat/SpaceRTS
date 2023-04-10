using SpaceRTS.Camera;
using SpaceRTS.Models;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRTS.Managers
{
    public class SystemManager : MonoBehaviour
    {
        public GameObject sunPrefab;
        public Planet planetPrefab;
        public float maxOrbitalSeparationDistance;
        public float minOrbitalSeparationDistance;
        public float maxPlanetSize;
        public float minPlanetSize;
        public int minNumOfPlanets;
        public int maxNumOfPlanets;

        private readonly List<Planet> planets = new List<Planet>();
        private GameObject sun;

        public float Size { get; private set; }

        private void Start()
        {
            this.sun = Instantiate(this.sunPrefab);
            this.sun.name = "Sun_1";

            float minOrbitalDistance = this.sun.transform.localScale.x;
            float previousPlanetOrbitalDistance = 0f;

            int numOfPlanets = Random.Range(this.minNumOfPlanets, this.maxNumOfPlanets + 1);
            for (int i = 0; i < numOfPlanets; i++)
            {
                Planet planet = Instantiate(this.planetPrefab);
                float orbitalSeparationDistance = Random.Range(this.minOrbitalSeparationDistance, this.maxOrbitalSeparationDistance);
                float planetOrbitalDistance = i == 0
                        ? minOrbitalDistance + (orbitalSeparationDistance * 0.5f)
                        : previousPlanetOrbitalDistance + orbitalSeparationDistance;
                float planetSize = Random.Range(this.minPlanetSize, this.maxPlanetSize);

                planet.SpawnBody(i + 1, planetOrbitalDistance, planetSize);
                previousPlanetOrbitalDistance = planet.Body.OrbitalDistance;
                this.planets.Add(planet);

                this.Size = planetOrbitalDistance;
            }

            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            CameraMovement cameraMovement = mainCamera.GetComponentInParent<CameraMovement>();
            cameraMovement.Range = this.Size;
        }
    }
}