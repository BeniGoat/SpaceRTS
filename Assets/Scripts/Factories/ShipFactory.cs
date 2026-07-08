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
            this.sourceBody = this.GetComponentInChildren<SystemBody>();
        }

        /// <summary>
        /// Attempts to spawn a ship in the next available orbital slot.
        /// Returns null if no slots are available.
        /// </summary>
        public Ship TrySpawnShip()
        {
            if (this.sourceBody == null || this.numOfShipsInOrbit >= this.orbitalSlots.Count)
                return null;

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

            this.numOfShipsInOrbit++;
            OnShipSpawned?.Invoke(newShip);
            return newShip;
        }
    }
}