using System.Collections.Generic;
using UnityEngine;
using SpaceRTS.Models;
using System;

namespace SpaceRTS.Spawners
{
    public class ShipSpawner : MonoBehaviour
    {
        public GameObject shipPrefab;
        public float distanceFromSurface;
        private SystemBody sourceBody;
        private int numOfShipsInOrbit;

        private readonly List<int> orbitalPositions = new List<int> { 0, 45, 90, 135, 180, 225, 270, 315 };
        private readonly List<int> orbitalRotations = new List<int> { 90, 45, 0, 315, 270, 225, 180, 135 };

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
            if (this.numOfShipsInOrbit < this.orbitalPositions.Count)
            {
                GameObject newShip = Instantiate(this.shipPrefab);

                // Set the object's initial position and rotation in its orbit 
                int positionInOrbit = this.orbitalPositions[this.numOfShipsInOrbit];
                int rotationInOrbit = this.orbitalRotations[this.numOfShipsInOrbit];
                float angle = positionInOrbit * Mathf.Deg2Rad;
                float orbitalDistance = this.sourceBody.MaxDiameter + this.distanceFromSurface;
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
                Debug.Log($"Ship #{this.numOfShipsInOrbit} spawned at {this.name}");
            }
        }
    }
}