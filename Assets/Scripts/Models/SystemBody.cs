using UnityEngine;

namespace SpaceRTS.Models
{
    public class SystemBody : SelectableObject
    {
        public float OrbitalDistance { get; set; }

        public float MaxDiameter
        {
            get
            {
                float maxRadius = this.transform.lossyScale.z;
                if (this.transform.lossyScale.y > this.transform.lossyScale.z)
                {
                    maxRadius = this.transform.lossyScale.y;
                }

                if (this.transform.lossyScale.x > maxRadius)
                {
                    maxRadius = this.transform.lossyScale.x;
                }

                return maxRadius;
            }
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

        protected override void ConfigureSelectionOutline()
        {
            //TODO: vary outline width based on body size and camera distance
        }
    }
}