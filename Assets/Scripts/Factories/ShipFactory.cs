using System.Collections.Generic;
using UnityEngine;
using SpaceRTS.Models;
using System;

namespace SpaceRTS.Spawners
{
    public class ShipFactory : MonoBehaviour
    {
        public Ship shipPrefab;
        private SystemBody sourceBody;
        private int numOfShipsInOrbit;

        private readonly List<(int, int)> orbitalPositionAndRotationAngles = new List<(int, int)>
        {
            (0, 90),
            (45, 45),
            (90, 0),
            (135, 315),
            (180, 270),
            (225, 225),
            (270, 180),
            (315, 135)
        };

        private void Start()
        {
            this.sourceBody = this.GetComponentInChildren<SystemBody>();
        }

        private void Update()
        {
            if (this.sourceBody != null && this.sourceBody.IsSelected && Input.GetKeyDown(KeyCode.B))
            {
                this.SpawnShip();
            }
        }

        private void SpawnShip()
        {
            if (this.numOfShipsInOrbit < this.orbitalPositionAndRotationAngles.Count)
            {
                Ship newShip = Instantiate(this.shipPrefab);
                newShip.name = $"Ship_{this.numOfShipsInOrbit}_From_{this.sourceBody.name}";
                newShip.CurrentSystemBody = this.sourceBody;

                // Set the object's initial position and rotation angle in its orbit 
                int positionInOrbit = this.orbitalPositionAndRotationAngles[this.numOfShipsInOrbit].Item1;
                int rotationInOrbit = this.orbitalPositionAndRotationAngles[this.numOfShipsInOrbit].Item2;
                float angle = positionInOrbit * Mathf.Deg2Rad;
                float orbitalDistance = this.sourceBody.MaxRadius * 1.2f;
                float x = orbitalDistance * Mathf.Cos(angle);
                float z = orbitalDistance * Mathf.Sin(angle);

                newShip.transform.parent = this.sourceBody.transform;
                newShip.transform.localPosition = new Vector3(x, 0, z);
                newShip.transform.rotation = this.sourceBody.transform.rotation * Quaternion.Euler(0, rotationInOrbit, 0);
                newShip.transform.localScale = new Vector3(
                        (float)Math.Round(newShip.transform.localScale.x * this.sourceBody.transform.localScale.x, 0),
                        (float)Math.Round(newShip.transform.localScale.y * this.sourceBody.transform.localScale.y, 0),
                        (float)Math.Round(newShip.transform.localScale.z * this.sourceBody.transform.localScale.z, 0));

                this.numOfShipsInOrbit++;
                Debug.Log($"{newShip.name} spawned");
            }
        }
    }
}