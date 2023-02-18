using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Managers
{
        public class SystemManager : MonoBehaviour
        {
                public GameObject sunPrefab;
                public Planet planetPrefab;
                public int numOfPlanets;
                public int orbitalSeparationDistance;

                private void Awake()
                {
                        Instantiate(this.sunPrefab);

                        for (int i = 0; i < this.numOfPlanets; i++)
                        {
                                Planet planet = Instantiate(this.planetPrefab);
                                planet.SpawnPlanetBody(i + 1, this.orbitalSeparationDistance);
                        }
                }
        }
}