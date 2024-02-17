using SpaceRTS.Spawners;
using UnityEngine;

namespace SpaceRTS.Models
{
    public class Moon : MonoBehaviour
    {
        public SystemBody Body { get; set; }
        private SystemBodyFactory bodySpawner;

        private void Awake()
        {
            this.bodySpawner = this.GetComponent<SystemBodyFactory>();
        }

        public void SpawnBody(int index, float orbitalDistance, float size)
        {
            this.name = $"Moon_{index}";
            this.Body = this.bodySpawner.SpawnChildBody(orbitalDistance, size);
        }
    }
}
