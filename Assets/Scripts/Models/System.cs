using System.Collections.Generic;
using UnityEngine;

namespace SpaceRTS.Models
{
    public class System : MonoBehaviour
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

        private int numOfPlanets;
        private readonly List<float> planetOrbitalDistances = new List<float>();

        public float Size { get; private set; }

        private void Awake()
        {
            this.sun = Instantiate(this.sunPrefab);
            this.sun.name = "Sun_1";
            float minOrbitalDistance = this.sun.transform.localScale.x;

            this.numOfPlanets = Random.Range(this.minNumOfPlanets, this.maxNumOfPlanets + 1);

            for (int i = 0; i < this.numOfPlanets; i++)
            {
                float orbitalSeparationDistance = Random.Range(this.minOrbitalSeparationDistance, this.maxOrbitalSeparationDistance);

                if (i == 0)
                {
                    this.planetOrbitalDistances.Add(minOrbitalDistance + (orbitalSeparationDistance * 0.5f));
                    continue;
                }

                this.planetOrbitalDistances.Add(this.planetOrbitalDistances[i - 1] + orbitalSeparationDistance);
            }

            this.Size = this.planetOrbitalDistances[this.numOfPlanets - 1];
        }

        private void Start()
        {
            for (int i = 0; i < this.numOfPlanets; i++)
            {
                Planet planet = Instantiate(this.planetPrefab);
                float planetSize = Random.Range(this.minPlanetSize, this.maxPlanetSize);
                planet.SpawnBody(i + 1, this.planetOrbitalDistances[i], planetSize);
                this.planets.Add(planet);
            }
        }
    }
}