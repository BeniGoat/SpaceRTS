using SpaceRTS.Models;
using UnityEngine;

namespace SpaceRTS.Factories
{
    /// <summary>
    /// Factory responsible for spawning ships in orbit around a source body.
    /// </summary>
    public class ShipFactory : MonoBehaviour
    {
        [SerializeField] private Ship shipPrefab;

        [SerializeField, Min(1)] private int minOrbitalSlots = 4;
        [SerializeField, Min(1)] private int maxOrbitalSlots = 16;
        [SerializeField, Min(0.01f)] private float bodyRadiusPerAdditionalSlot = 0.05f;
        [SerializeField, Min(0.1f)] private float shipPrefabScale = 0.25f;

		private SystemBody sourceBody;
		private int numOfShipsInOrbit;

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
		/// <returns>The newly spawned ship, or null if spawning was unsuccessful.</returns>
		public Ship TrySpawnShip()
        {
			if (this.sourceBody == null)
				return null;

			int orbitalSlotCount = this.GetOrbitalSlotCount();
			if (this.numOfShipsInOrbit >= orbitalSlotCount)            
                return null;            

			// Get the position and rotation angles for the next available orbital slot
			float positionAngle = this.numOfShipsInOrbit * 360f / orbitalSlotCount;
			float rotationAngle = 90f - positionAngle;
            float angle = positionAngle * Mathf.Deg2Rad;

			// Calculate the orbital distance based on the source body's radius and a minimum distance
			float orbitalDistance = this.sourceBody.WorldRadius 
				+ Mathf.Max(this.sourceBody.WorldRadius * 0.25f, 0.05f);

			// Calculate the orbital offset and rotation for the new ship
			Vector3 orbitalOffset = new(
				orbitalDistance * Mathf.Cos(angle),
				0,
				orbitalDistance * Mathf.Sin(angle));
			Quaternion rotation = this.sourceBody.transform.rotation
                * Quaternion.Euler(0f, rotationAngle, 0f);

			// Calculate the world position for the new ship based on the source body's position and rotation
			Vector3 position = this.sourceBody.transform.position
                + this.sourceBody.transform.rotation * orbitalOffset;

			// Instantiate the ship prefab at the calculated position and rotation,
			// as a child of the object this ship factory is attached to, and scale it relative to the source body
			Ship newShip = Instantiate(
                this.shipPrefab,
                position,
                rotation,
                this.transform);
			newShip.transform.localScale = this.shipPrefab.transform.localScale * this.shipPrefabScale;

			newShip.CurrentSystemBody = this.sourceBody;		
            this.numOfShipsInOrbit++;

			return newShip;
        }

		/// <summary>
		/// Calculates the number of orbital slots based on the source body's radius.
		/// </summary>
		/// <returns>The calculated orbital slot count, clamped between the configured minimum and maximum values.</returns>
		private int GetOrbitalSlotCount()
		{
			// Calculate the number of additional orbital slots based on the source body's radius
			// and the configured body radius per additional slot
			int additionalSlots = Mathf.FloorToInt(
				this.sourceBody.WorldRadius / this.bodyRadiusPerAdditionalSlot);

			// Clamp the total number of orbital slots between the minimum and maximum values
			return Mathf.Clamp(
				this.minOrbitalSlots + additionalSlots,
				this.minOrbitalSlots,
				this.maxOrbitalSlots);
		}
	}
}