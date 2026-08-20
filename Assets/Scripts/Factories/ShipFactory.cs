using System;
using System.Collections.Generic;
using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Factories
{
    /// <summary>
    /// Factory responsible for spawning ships in orbit around a source body.
    /// </summary>
    public class ShipFactory : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when a new ship is spawned.
        /// </summary>
        public static event Action<Ship> OnShipSpawned;

        [SerializeField] private Ship shipPrefab;
        private SystemBody sourceBody;
        private int numOfShipsInOrbit;

        private readonly List<(int position, int rotation)> orbitalSlots = new List<(int, int)>
        {
            (0, 90), (45, 45), (90, 0), (135, 315),
            (180, 270), (225, 225), (270, 180), (315, 135)
        };

        private void Start()
        {
            // Initialize the source body reference from the child SystemBody component
            this.sourceBody = this.GetComponentInChildren<SystemBody>();
        }

        /// <summary>
        /// Spawns a ship prefab into orbit around the configured source body by instantiating the prefab, naming it,
        /// assigning its CurrentSystemBody, positioning and orienting it at the next available orbital slot, scaling it
        /// relative to the source body, invoking OnShipSpawned, logging the spawn, and incrementing the count of ships
        /// in orbit.
        /// </summary>
        /// <remarks>Checks for a valid source body and available orbital slots and logs a warning when
        /// either is missing. Orbital position and rotation are taken from orbitalSlots and orbital distance is
        /// computed from the source body's MaxRadius.</remarks>
        public void TrySpawnShip()
        {
            if (this.sourceBody == null || this.numOfShipsInOrbit >= this.orbitalSlots.Count)
            {
                Debug.LogWarning($"Cannot spawn ship: {(this.sourceBody == null ? "Source body is null." : "No available orbital slots.")}");
                return;
            }

            Ship newShip = Instantiate(this.shipPrefab);
            newShip.name = $"Ship_{this.numOfShipsInOrbit}_From_{this.sourceBody.name}";
            newShip.CurrentSystemBody = this.sourceBody;

            var (positionAngle, rotationAngle) = this.orbitalSlots[this.numOfShipsInOrbit];
            float angle = positionAngle * Mathf.Deg2Rad;
            float orbitalDistance = this.sourceBody.MaxRadius * 1.2f;

            newShip.transform.parent = this.sourceBody.transform;
            newShip.transform.localPosition = new Vector3(
                orbitalDistance * Mathf.Cos(angle),
                0,
                orbitalDistance * Mathf.Sin(angle));
            newShip.transform.rotation = this.sourceBody.transform.rotation * Quaternion.Euler(0, rotationAngle, 0);
            newShip.transform.localScale = new Vector3(
                (float)Math.Round(newShip.transform.localScale.x * this.sourceBody.transform.localScale.x, 0),
                (float)Math.Round(newShip.transform.localScale.y * this.sourceBody.transform.localScale.y, 0),
                (float)Math.Round(newShip.transform.localScale.z * this.sourceBody.transform.localScale.z, 0));

            OnShipSpawned?.Invoke(newShip);
            Debug.Log($"Spawned {newShip.name} in orbit around {this.sourceBody.name} at slot {this.numOfShipsInOrbit}.");

            this.numOfShipsInOrbit++;
        }
    }
}