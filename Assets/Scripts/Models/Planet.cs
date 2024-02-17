using SpaceRTS.Spawners;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRTS.Models
{
    public class Planet : MonoBehaviour
    {
        public Moon moonPrefab;
        public int maxOrbitalSeparationDistance;
        public int minOrbitalSeparationDistance;
        public int maxNumOfMoons;
        public int minNumOfMoons;
        public int maxMoonSize;
        public int minMoonSize;

        public SystemBody Body { get; set; }

        private readonly List<Moon> moons = new List<Moon>();
        private SystemBodyFactory bodySpawner;

        private void Awake()
        {
            this.bodySpawner = this.GetComponent<SystemBodyFactory>();
        }

        public void SpawnBody(int index, float orbitalDistance, float size)
        {
            this.name = $"Planet_{index}";
            this.Body = this.bodySpawner.SpawnChildBody(orbitalDistance, size);

            int numOfMoons = Random.Range(this.minNumOfMoons, this.maxNumOfMoons + 1);
            for (int i = 0; i < numOfMoons; i++)
            {
                Moon moon = Instantiate(this.moonPrefab, this.transform);
                moon.transform.position = this.Body.transform.position;

                // Get the orbital distance difference between this moon's orbit and either the planet's surface or the previous moon's orbit
                float orbitalSeparationDistance = Random.Range(this.minOrbitalSeparationDistance, this.maxOrbitalSeparationDistance + 1) * 0.01f;
                float moonOrbitalDistance = i == 0
                    ? this.Body.MaxRadius + orbitalSeparationDistance
                    : this.moons[i-1].Body.OrbitalDistance + orbitalSeparationDistance;

                // Spawn the moon
                float moonSize = Random.Range(this.minMoonSize, this.maxMoonSize + 1) * 0.02f;
                moon.SpawnBody(i + 1, moonOrbitalDistance, moonSize);
                this.moons.Add(moon);
            }
        }
    }
}
