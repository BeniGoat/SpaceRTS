using System;
using UnityEngine;

namespace SpaceRTS.Models.Components
{
        [RequireComponent(typeof(LineRenderer))]
        public class OrbitLine : MonoBehaviour
        {
                public Color orbitLineColour = new Color(1, 1, 1, 0.5f);

                private LineRenderer orbitalPath;

                public void CreateOrbitalPathLine(float orbitalDistance, int startingPosition)
                {
                        this.orbitalPath = this.GetComponent<LineRenderer>();
                        this.orbitalPath.startWidth = 0.025f;
                        this.orbitalPath.endWidth = 0.025f;
                        this.orbitalPath.startColor = this.orbitLineColour;
                        this.orbitalPath.endColor = new Color(
                                this.orbitLineColour.r,
                                this.orbitLineColour.g,
                                this.orbitLineColour.b,
                                this.orbitLineColour.a * 0.1f);

                        // Initialize the orbital path line with a fixed number of vertices
                        this.orbitalPath.positionCount = 360;
                        Vector3 orbitalPoint;

                        for (int i = 0; i < this.orbitalPath.positionCount; i++)
                        {
                                float angle = (i + startingPosition) * Mathf.Deg2Rad;
                                float x = orbitalDistance * Mathf.Cos(angle);
                                float z = orbitalDistance * Mathf.Sin(angle);
                                orbitalPoint = new Vector3(x, 0, z);
                                this.orbitalPath.SetPosition(i, orbitalPoint);
                        }
                }
        }
}
