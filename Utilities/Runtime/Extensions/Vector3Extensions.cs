using UnityEngine;

namespace InfiniteCanvas.Utilities.Extensions
{
	public static class Vector3Extensions
	{
		/// <summary>
		///     Checks if a point is within a view cone defined by direction, distance, and angular constraints
		/// </summary>
		/// <param name="origin">View origin position</param>
		/// <param name="viewDirection">Normalized viewing direction</param>
		/// <param name="target">Target position to check</param>
		/// <param name="viewDistance">Maximum viewing distance</param>
		/// <param name="horizontalAngle">Horizontal FOV in degrees</param>
		/// <param name="verticalAngle">Vertical FOV in degrees</param>
		/// <returns>True if point is within visible cone</returns>
		/// <remarks>
		///     The <see cref="horizontalAngle" /> extends to both sides of the <see cref="viewDirection" />, so for a total FoV of
		///     60 you need to set <see cref="horizontalAngle" /> to 30
		///     The same applies to <see cref="verticalAngle" />
		/// </remarks>
		public static bool CouldSee(this in Vector3 origin,
		                            in      Vector3 viewDirection,
		                            in      Vector3 target,
		                            in      float   viewDistance,
		                            in      float   horizontalAngle,
		                            in      float   verticalAngle)
		{
			var lineOfSight = target - origin;
			var sqrDistance = lineOfSight.sqrMagnitude;

			if (sqrDistance > viewDistance * viewDistance) return false;
			if (sqrDistance < Mathf.Epsilon) return true;

			lineOfSight.Normalize();

			// Check if target is behind the view origin
			if (Vector3.Dot(lineOfSight, viewDirection) <= 0) return false;

			// Create view rotation using world up
			var viewRotation = Quaternion.LookRotation(viewDirection, Vector3.up);
			var localDir = Quaternion.Inverse(viewRotation) * lineOfSight;

			// Calculate angular deviations
			var yaw = Mathf.Atan2(localDir.x,   localDir.z) * Mathf.Rad2Deg;
			var pitch = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

			// Angular constraints check
			return Mathf.Abs(yaw) <= horizontalAngle && Mathf.Abs(pitch) <= verticalAngle;
		}
	}
}