using SpaceRTS.Spawners;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRTS.Models
{
        public class Planet : MonoBehaviour
        {
                public Moon moonPrefab;
                public float maxOrbitalSeparationDistance;
                public float minOrbitalSeparationDistance;
                public int maxNumOfMoons;
                public int minNumOfMoons;
                public float maxMoonSize;
                public float minMoonSize;

                public SystemBody Body { get; set; }

                private readonly List<Moon> moons = new List<Moon>();
                private SystemBodySpawner bodySpawner;

                private void Awake()
                {
                        this.bodySpawner = this.GetComponent<SystemBodySpawner>();
                }

                public void SpawnBody(int index, float orbitalDistance, float size)
                {
                        this.name = $"Planet_{index}";
                        this.Body = this.bodySpawner.SpawnChildBody(orbitalDistance, size);

                        float previousMoonOrbitDistance = 0f;

                        // Spawn the planet's moons
                        int numOfMoons = Random.Range(this.minNumOfMoons, this.maxNumOfMoons + 1);
                        for (int i = 0; i < numOfMoons; i++)
                        {
                                Moon moon = Instantiate(this.moonPrefab, this.transform);
                                moon.transform.position = this.Body.transform.position;

                                float orbitalSeparationDistance = Random.Range(this.minOrbitalSeparationDistance, this.maxOrbitalSeparationDistance);
                                float moonOrbitalDistance = i == 0
                                        ? this.Body.MaxDiameter + (orbitalSeparationDistance * 0.25f)
                                        : previousMoonOrbitDistance + orbitalSeparationDistance;
                                float moonSize = Random.Range(this.minMoonSize, this.maxMoonSize);

                                moon.SpawnBody(i + 1, moonOrbitalDistance, moonSize);
                                previousMoonOrbitDistance = moon.Body.OrbitalDistance;
                                this.moons.Add(moon);
                        }
                }
        }
}
