using UnityEngine;

namespace SpaceRTS.Models.Components
{
	/// <summary>
	/// Renders a world-space curved navigation path from the ship to a destination transform using a LineRenderer.
	/// </summary>
	/// <remarks>Configures line width and gradient styling, then recalculates a quadratic arc each frame while a
	/// destination is active. Requires a LineRenderer component on the same GameObject.</remarks>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LineRenderer))]
	public class ShipPathFinder : MonoBehaviour
	{
		[SerializeField] private Color pathLineColour = new(0.1f, 1f, 0.1f, 0.5f);
		[SerializeField, Range(0f, 5f)] private float pathArcHeightFactor = 0.2f;
		[SerializeField, Range(4, 64)] private int pathArcSegmentCount = 20;
		[SerializeField, Range(0.001f, 0.05f)] private float pathLineStartWidth = 0.003f;
		[SerializeField, Range(0.001f, 0.05f)] private float pathLineMiddleWidth = 0.012f;
		[SerializeField, Range(0.001f, 0.05f)] private float pathLineEndWidth = 0.002f;

		private LineRenderer pathLine;
		private Transform destination;

		private void Awake()
		{
			// Configure the LineRenderer for the path visualization
			this.pathLine = this.GetComponent<LineRenderer>();
			this.ConfigurePathLine();
		}

		private void Update()
		{
			if (this.destination == null)
				return;

			this.UpdatePathLine();
		}

		/// <summary>
		/// Displays and updates the path line to the specified destination transform.
		/// </summary>
		/// <remarks>If <paramref name="destinationTransform"/> is <see langword="null"/>, no changes are
		/// made.</remarks>
		/// <param name="destinationTransform">Transform that defines the destination for the path line.</param>
		public void ShowPath(Transform destinationTransform)
		{
			if (destinationTransform == null)
				return;

			this.destination = destinationTransform;
			this.pathLine.enabled = true;
			this.pathLine.forceRenderingOff = false;
			this.UpdatePathLine();
		}

		/// <summary>
		/// Clears the current destination and hides the path line from rendering.
		/// </summary>
		public void HidePath()
		{
			this.destination = null;
			this.pathLine.enabled = false;
			this.pathLine.forceRenderingOff = true;
		}

		/// <summary>
		/// Configures the path line renderer with initial rendering state, segment count, width profile, and color gradient.
		/// </summary>
		/// <remarks>Sets world-space, non-looping line behavior, applies a three-key width curve, and uses alpha keys
		/// to fade the line in and out along its length.</remarks>
		private void ConfigurePathLine()
		{
			this.pathLine.enabled = false;
			this.pathLine.forceRenderingOff = true;
			this.pathLine.useWorldSpace = true;
			this.pathLine.loop = false;
			this.pathLine.positionCount = this.pathArcSegmentCount + 1;
			this.pathLine.widthCurve = new AnimationCurve(
				new Keyframe(0f, this.pathLineStartWidth),
				new Keyframe(0.5f, this.pathLineMiddleWidth),
				new Keyframe(1f, this.pathLineEndWidth));
			this.pathLine.widthMultiplier = 1f;

			Color baseColour = this.pathLineColour;
			Gradient gradient = new();
			gradient.SetKeys(
				new[]
				{
					new GradientColorKey(baseColour, 0f),
					new GradientColorKey(baseColour, 1f)
				},
				new[]
				{
					new GradientAlphaKey(baseColour.a * 0.15f, 0f),
					new GradientAlphaKey(baseColour.a, 0.2f),
					new GradientAlphaKey(baseColour.a * 0.9f, 0.8f),
					new GradientAlphaKey(baseColour.a * 0.1f, 1f)
				});
			this.pathLine.colorGradient = gradient;
		}

		/// <summary>
		/// Updates the path line positions between the current transform
		/// position and destination using a quadratic Bézier arc.
		/// </summary>
		/// <remarks>Sets the line position count to at least two points, handles near-zero distance by collapsing
		/// intermediate points to the start position, and derives a bend direction from a plane projection to shape the arc
		/// height.</remarks>
		private void UpdatePathLine()
		{
			Vector3 start = this.transform.position;
			Vector3 end = this.destination.position;
			Vector3 route = end - start;
			float routeDistance = route.magnitude;
			int positionCount = Mathf.Max(2, this.pathArcSegmentCount + 1);

			if (this.pathLine.positionCount != positionCount)
			{
				this.pathLine.positionCount = positionCount;
			}

			// Handle the case where the start and end positions are extremely close together
			if (routeDistance <= 0.001f)
			{
				this.pathLine.SetPosition(0, start);
				this.pathLine.SetPosition(positionCount - 1, end);
				for (int i = 1; i < positionCount - 1; i++)
				{
					this.pathLine.SetPosition(i, start);
				}

				return;
			}

			// Calculate the bend direction for the quadratic Bézier curve based on the route vector
			Vector3 routeDirection = route / routeDistance;
			Vector3 bendDirection = Vector3.ProjectOnPlane(Vector3.up, routeDirection).normalized;
			if (bendDirection.sqrMagnitude <= 0.0001f)
			{
				bendDirection = Vector3.ProjectOnPlane(Vector3.right, routeDirection).normalized;
			}

			// Calculate the control point for the quadratic Bézier curve, which determines the arc's height and curvature
			Vector3 controlPoint = (start + end) * 0.5f + bendDirection * (routeDistance * this.pathArcHeightFactor);
			for (int i = 0; i < positionCount; i++)
			{
				float t = i / (float)(positionCount - 1);
				float oneMinusT = 1f - t;
				Vector3 curvePoint =
					(oneMinusT * oneMinusT * start) +
					(2f * oneMinusT * t * controlPoint) +
					(t * t * end);
				this.pathLine.SetPosition(i, curvePoint);
			}
		}
	}
}
