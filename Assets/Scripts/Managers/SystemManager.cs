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

                private void Awake()
                {
                        this.sun = Instantiate(this.sunPrefab);
                        this.sun.name = "Sun_1";
                }

                private void Start()
                {
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
                        }
                }
        }
}