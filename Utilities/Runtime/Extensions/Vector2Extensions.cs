using System.Runtime.CompilerServices;
using UnityEngine;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        ///     Return a random int within [minInclusive...maxExclusive) (Read Only)
        /// </summary>
        /// <param name="v">x is minInclusive, y is maxExclusive</param>
        /// <returns>A random integer between x(inclusive) and y(exclusive) components of input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomBetween(this in Vector2 v) => Random.Range(v.x, v.y);

        /// <summary>
        ///     Efficient 2D view cone check with angular and distance constraints
        /// </summary>
        /// <param name="origin">Normalized source position</param>
        /// <param name="viewDirection">Normalized viewing direction</param>
        /// <param name="target">Normalized target position</param>
        /// <param name="viewDistance">Maximum view distance</param>
        /// <param name="viewAngle">Half of view cone angle in degrees</param>
        /// <remarks>
        ///     The <see cref="viewAngle" /> extends to both sides of the <see cref="viewDirection" />, so for a total FoV of
        ///     60 you need to set <see cref="viewAngle" /> to 30
        /// </remarks>
        public static bool CouldSee(this in Vector2 origin,
                                    in      Vector2 viewDirection,
                                    in      Vector2 target,
                                    in      float   viewDistance,
                                    in      float   viewAngle)
        {
            var lineOfSight = target - origin;
            var sqrDistance = lineOfSight.sqrMagnitude;

            if (sqrDistance > viewDistance * viewDistance) return false;
            if (sqrDistance < Mathf.Epsilon) return true;

            lineOfSight.Normalize();

            // Behind check using dot product
            if (Vector2.Dot(lineOfSight, viewDirection) <= 0) return false;

            var angle = Vector2.Angle(viewDirection, lineOfSight);
            return angle <= viewAngle;
        }
    }
}