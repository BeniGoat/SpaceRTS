namespace SpaceRTS.Models
{
        public class SystemBody : SelectableObject
        {
                public float Radius
                {
                        get
                        {
                                if (this.transform.lossyScale.x > this.transform.lossyScale.z)
                                {
                                        return this.transform.lossyScale.x * 0.5f;
                                }

                                return this.transform.lossyScale.z * 0.5f;
                        }
                }
        }
}