using UnityEngine;

namespace SpaceRTS.Models
{
    public class SystemBody : SelectableObject
    {
        public float OrbitalDistance { get; set; }

        public float MaxRadius
        {
            get
            {
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
            this.ConfigureSelectionOutline(this.MaxRadius * 2f);
        }

        public void SetBodySize(float diameter)
        {
            this.SetBodySize(diameter, diameter, diameter);
        }

        public void SetBodySize(float x, float y, float z)
        {
            this.SetBodySize(new Vector3(x, y, z));
        }

        public void SetBodySize(Vector3 scale)
        {
            this.transform.localScale = scale;
        }
    }
}