using SpaceRTS.Models.Components;
using System;
using UnityEngine;

namespace SpaceRTS.Models
{
        public class Planet : MonoBehaviour
        {
                public SystemBody planetPrefab;
                private SystemBody planet;

                public void SpawnPlanetBody(int index, float orbitalSeparationDistance)
                {
                        // Set the planet's initial position in its orbit 
                        int positionInOrbit = UnityEngine.Random.Range(0, 359);
                        float angle = positionInOrbit * Mathf.Deg2Rad;
                        float orbitalDistance = orbitalSeparationDistance * index;
                        float x = orbitalDistance * Mathf.Cos(angle);
                        float z = orbitalDistance * Mathf.Sin(angle);

                        this.planet = Instantiate(this.planetPrefab, new Vector3(x, 0, z), Quaternion.identity, this.transform);
                        this.name = $"Planet_{index}";

                        //TODO: Generate variable planet sizes
                        this.planet.SetBodySize(0.5f, 0.5f, 0.5f);

                        // Get the orbital speed based on the orbital distance
                        float orbitalSpeed = this.GetOrbitalSpeed(orbitalDistance);
                        Rotator orbitRotator = this.GetComponent<Rotator>();
                        orbitRotator.SetRotationSpeed(orbitalSpeed);

                        // Set the planet body rotation speed
                        Rotator bodyRotator = this.planet.GetComponent<Rotator>();
                        bodyRotator.SetRotationSpeed(15f);

                        // Create the orbit line
                        OrbitLine orbitLine = this.GetComponent<OrbitLine>();
                        orbitLine.CreateOrbitalPathLine(orbitalDistance, positionInOrbit);
                }

                /// <summary>
                /// Gets the orbital speed based on the specified orbital distance.
                /// </summary>
                /// <param name="orbitalDistance">The orbital distance.</param>
                /// <returns>The orbital speed.</returns>
                private float GetOrbitalSpeed(float orbitalDistance)
                {
                        double orbitalPeriod = Math.Sqrt(Math.Pow(orbitalDistance, 3));
                        double orbitalLength = 2 * Math.PI * orbitalDistance;
                        return (float)(orbitalLength / orbitalPeriod);
                }
        }
}
