using UnityEngine;

namespace SpaceRTS.Models
{
    
    [RequireComponent(typeof(SelectableComponent))]
    public class SystemBody : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the orbital distance of the celestial body from its parent object.
        /// </summary>
        public float OrbitalDistance { get; set; }

        /// <summary>
        /// Gets the maximum radius of the celestial body based on its current scale.
        /// </summary>
        public float MaxRadius
        {
            get
            {
                // Calculate the maximum radius based on the lossy scale of the transform.
                float maxRadius = this.transform.lossyScale.normalized.z;
                if (this.transform.lossyScale.normalized.y > this.transform.lossyScale.normalized.z)
                {
                    maxRadius = this.transform.lossyScale.normalized.y;
                }

                if (this.transform.lossyScale.normalized.x > maxRadius)
                {
                    maxRadius = this.transform.lossyScale.normalized.x;
                }

                return maxRadius;
            }
        }

        private void Awake()
        {
            // Configure the selection outline for the celestial body on awake
            var selectable = this.GetComponent<SelectableComponent>();
            selectable.ConfigureSelectionOutline(this.MaxRadius * 2f);
        }

        /// <summary>
        /// Sets the size of the celestial body uniformly in all dimensions based on the specified diameter.
        /// </summary>
        /// <param name="diameter">The diameter of the celestial body.</param>
        public void SetBodySize(float diameter)
        {
            this.SetBodySize(diameter, diameter, diameter);
        }

        /// <summary>
        /// Sets the size of the celestial body in each dimension based on the specified values.
        /// </summary>
        /// <param name="x">The scale along the x-axis.</param>
        /// <param name="y">The scale along the y-axis.</param>
        /// <param name="z">The scale along the z-axis.</param>
        public void SetBodySize(float x, float y, float z)
        {
            this.SetBodySize(new Vector3(x, y, z));
        }

        /// <summary>
        /// Sets the size of the celestial body based on the specified scale.
        /// </summary>
        /// <param name="scale">The scale vector for each dimension.</param>
        public void SetBodySize(Vector3 scale)
        {
            this.transform.localScale = scale;
        }
    }
}