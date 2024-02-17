using UnityEngine;

namespace SpaceRTS.Models.Components
{
    /// <summary>
    /// Component that manages the rotation of an object.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        /// <summary>
        /// The axis of rotation
        /// </summary>
        private Vector3 axis;

        /// <summary>
        /// Speed of rotation in degrees per second
        /// </summary>
        private float degreesPerSecond;

        /// <summary>
        /// Gets the angular velocity of the system body in radians per second.
        /// </summary>
        public float AngularVelocity => this.degreesPerSecond * 2 * Mathf.PI / 360;

        /// <summary>
        /// Method called every frame.
        /// </summary>
        private void Update()
        {
            // Rotate the object around its up axis
            this.transform.Rotate(this.axis, this.degreesPerSecond * Time.deltaTime);
        }

        /// <summary>
        /// Sets the orbital speed.
        /// </summary>
        /// <param name="degreesPerSecond">The rotation speed in degrees per second.</param>
        /// <param name="axis">The axis of rotation.</param>
        public void SetOrbitalSpeed(float degreesPerSecond, Vector3 axis)
        {
            this.axis = axis;
            this.degreesPerSecond = degreesPerSecond;
        }
    }
}