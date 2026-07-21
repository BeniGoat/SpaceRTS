using SpaceRTS.Managers;
using SpaceRTS.Models.Interfaces;
using UnityEngine;

namespace SpaceRTS.Models
{
    /// <summary>
    /// Represents a ship in the game, which can be selected, moved to a destination system body, and visualized with a path line.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(SelectableComponent))]
    public class Ship : MonoBehaviour
    {
        [SerializeField] private float travelSpeed = 1f;
        [SerializeField] private float arrivalDistance = 0.05f;
        [SerializeField] private Color pathLineColour = new Color(0.1f, 1, 0.1f, 0.5f);
        [SerializeField] private MovementManager movementManager;

        private ISelectable selectable;
        private LineRenderer path;
        private SystemBody destinationBody;

        /// <summary>
        /// Gets or sets the current system body where the ship is located.
        /// </summary>
        public SystemBody CurrentSystemBody { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ship has arrived at its destination system body.
        /// </summary>
        public bool HasArrived { get; internal set; }

        private void Awake()
        {
            this.movementManager = this.movementManager != null 
                ? this.movementManager 
                : FindAnyObjectByType<MovementManager>();
            
            // Configure the selection outline for the ship on awake
            this.selectable = this.GetComponent<SelectableComponent>();
            this.selectable.ConfigureSelectionOutline(1f);
            this.ConfigureShipPathLine();
        }

        private void Update()
        {
            // Update the ship path line if it is enabled and a destination body is set
            if (this.path.enabled && this.destinationBody != null)
            {
                this.UpdatePathLine();
            }
        }

        /// <summary>
        /// Sets the destination system body for the ship and enables the path line renderer to visualize the path.
        /// </summary>
        /// <param name="target">The target system body to set as the destination.</param>
        public void SetDestination(SystemBody target)
        {
            // Set the destination system body for the ship and enable the path line renderer
            this.destinationBody = target;
            this.HasArrived = false;

            this.path.enabled = true;
            this.path.forceRenderingOff = false;


            if (this.movementManager != null)
            {
                this.movementManager.RegisterMovingShip(this);
            }
        }

        /// <summary>
        /// Clears the destination system body for the ship and disables the path line renderer.
        /// </summary>
        public void ClearDestination()
        {
            this.destinationBody = null;
            this.HasArrived = false;

            // Unregister the ship from the movement manager when clearing the destination
            if (this.movementManager != null)
            {
                this.movementManager.UnregisterMovingShip(this);
            }

            this.path.enabled = false;
            this.path.forceRenderingOff = true;
        }

        /// <summary>
        /// Configures the LineRenderer component for the ship's path line visualization.
        /// </summary>
        private void ConfigureShipPathLine()
        {
            // Configure the LineRenderer component for the ship's path line visualization
            this.path = this.GetComponent<LineRenderer>();
            this.path.enabled = false;
            this.path.forceRenderingOff = true;
            this.path.useWorldSpace = true;
            this.path.loop = false;
            this.path.startWidth = 0.01f;
            this.path.endWidth = 0.01f;
            this.path.startColor = new Color(
                this.pathLineColour.r,
                this.pathLineColour.g,
                this.pathLineColour.b,
                this.pathLineColour.a * 0.5f);
            this.path.endColor = this.pathLineColour;
            this.path.positionCount = 2;
        }

        /// <summary>
        /// Updates the positions of the path line renderer to visualize the path
        /// from the current system body to the destination system body.
        /// </summary>
        private void UpdatePathLine()
        {
            this.path.SetPosition(0, this.transform.position);
            this.path.SetPosition(1, this.destinationBody.transform.position);
        }

        /// <summary>
        /// Completes the ship's travel to the destination system body, updating the
        /// current system body and disabling the path line renderer.
        /// </summary>
        internal void CompleteTravel()
        {
            // If there is no destination body set, return early
            if (this.destinationBody == null)
            {
                return;
            }

            // Update the current system body to the destination body and clear the destination
            this.CurrentSystemBody = this.destinationBody;
            this.destinationBody = null;
            this.HasArrived = false;

            // Unregister the ship from the movement manager
            if (this.movementManager != null)
            {
                this.movementManager.UnregisterMovingShip(this);
            }

            // Disable the path line renderer after completing the travel
            this.path.enabled = false;
            this.path.forceRenderingOff = true;
        }

        /// <summary>
        /// Processes the ship's travel towards its destination system body based on the specified delta time.
        /// If the ship has arrived at the destination, it sets the HasArrived property to true.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        internal void ProcessTravel(float deltaTime)
        {
            // If there is no destination body set, return early
            if (this.destinationBody == null)
            {
                return;
            }

            // Move the ship towards the destination body at the specified travel speed
            Vector3 targetPosition = this.destinationBody.transform.position;
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                targetPosition,
                this.travelSpeed * deltaTime);

            //Update the path line to reflect the ship's current position and the destination
            this.UpdatePathLine();

            // Check if the ship has arrived at the destination body
            if (Vector3.Distance(this.transform.position, targetPosition) <= this.arrivalDistance)
            {
                this.HasArrived = true;
            }
        }
    }
}