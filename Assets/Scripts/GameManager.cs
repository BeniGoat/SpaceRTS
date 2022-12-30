using UnityEngine;

namespace SpaceRTS.Managers
{
        public class GameManager : MonoBehaviour
        {
                public GameObject sunPrefab;
                public GameObject planetPrefab;

                void Start()
                {
                        // Instantiate the sun and planet objects
                        GameObject sun = Instantiate(sunPrefab);
                        GameObject planet = Instantiate(planetPrefab);

                        // Set the planet's orbit around the sun
                        planet.transform.parent = sun.transform;
                        planet.transform.localPosition = new Vector3(5, 0, 0);

                        // Initialize any other gameplay mechanics or systems here
                }
        }
}