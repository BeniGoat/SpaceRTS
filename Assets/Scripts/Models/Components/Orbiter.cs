using System;
using UnityEngine;

namespace SpaceRTS.Model.Components
{
        [RequireComponent(typeof(LineRenderer))]
        public class Orbiter : MonoBehaviour
        {
                public Color orbitLineColour = new Color(1, 1, 1, 0.5f);

                private LineRenderer orbitalPath;
                private readonly int numVertices = 360;

                public float orbitalDistance;
                public float orbitSpeed;

                private void Start()
                {
                        this.orbitalDistance = Vector3.Distance(this.transform.position, this.transform.parent.position);

                        this.SetOrbitalSpeed();
                        this.CreateOrbitalPathLine();
                }

                private void Update()
                {
                        // Rotate the object around a focal point
                        this.transform.RotateAround(this.transform.parent.position, Vector3.up, this.orbitSpeed * Time.deltaTime);
                }

                private void SetOrbitalSpeed()
                {
                        double orbitalPeriod = Math.Sqrt(Math.Pow(this.orbitalDistance, 3));
                        double orbitalLength = 2 * Math.PI * this.orbitalDistance;
                        this.orbitSpeed = (float)(orbitalLength / orbitalPeriod);
                }

                private void CreateOrbitalPathLine()
                {
                        this.orbitalPath = this.GetComponent<LineRenderer>();
                        this.orbitalPath.startWidth = 0.05f;
                        this.orbitalPath.endWidth = 0.05f;
                        this.orbitalPath.startColor = this.orbitLineColour;
                        this.orbitalPath.endColor = this.orbitLineColour;

                        // Initialize the orbital path line with a fixed number of vertices
                        this.orbitalPath.positionCount = this.numVertices;
                        Vector3 orbitalPoint;

                        for (int i = 0; i < this.numVertices; i++)
                        {
                                float angle = i * Mathf.Deg2Rad;
                                float x = this.orbitalDistance * Mathf.Cos(angle);
                                float z = this.orbitalDistance * Mathf.Sin(angle);
                                orbitalPoint = new Vector3(x, 0, z);
                                this.orbitalPath.SetPosition(i, orbitalPoint);
                        }
                }
        }
}
