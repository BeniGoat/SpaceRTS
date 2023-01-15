using UnityEngine;

namespace SpaceRTS.Models.Components
{
        /// <summary>
        /// Component that manages the rotation of an object.
        /// </summary>
        public class Rotator : MonoBehaviour
        {
                /// <summary>
                /// Speed of rotation in degrees per second
                /// </summary>
                public float rotationSpeed;

                /// <summary>
                /// Method called every frame.
                /// </summary>
                private void Update()
                {
                        // Rotate the object around its up axis
                        this.transform.Rotate(Vector3.up, this.rotationSpeed * Time.deltaTime);
                }

                /// <summary>
                /// Sets the rotation speed.
                /// </summary>
                /// <param name="degreesPerSecond">The rotation speed in degrees per second.</param>
                public void SetRotationSpeed(float degreesPerSecond)
                {
                        this.rotationSpeed = degreesPerSecond;
                }
        }
}