using SpaceRTS.Models.Components;
using UnityEngine;

namespace SpaceRTS.Spawners
{
        public class PlanetSpawner : MonoBehaviour
        {
                public GameObject planetPrefab;
                public int orbitalSeparationDistance = 5;

                public void SpawnPlanets(GameObject sun, int numPlanets)
                {
                        for (int i = 1; i < numPlanets + 1; i++)
                        {
                                this.SpawnPlanet(sun, i);
                        }
                }

                private void SpawnPlanet(GameObject sun, int index)
                {
                        GameObject planet = Instantiate(this.planetPrefab);
                        planet.transform.parent = sun.transform;
                        planet.name = $"Planet_{index}";

                        Rotator rotator = planet.GetComponentInChildren<Rotator>();
                        rotator.SetRotationSpeed(15f);

                        // Set the planet's initial position in its orbit 
                        int positionInOrbit = Random.Range(0, 359);
                        float angle = positionInOrbit * Mathf.Deg2Rad;
                        float orbitalDistance = this.orbitalSeparationDistance * index;
                        float x = orbitalDistance * Mathf.Cos(angle);
                        float z = orbitalDistance * Mathf.Sin(angle);

                        planet.transform.localPosition = new Vector3(x, 0, z);
                }
        }
}