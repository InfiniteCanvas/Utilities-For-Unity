using System.Runtime.CompilerServices;
using UnityEngine;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class Vector2IntExtensions
    {
        /// <summary>
        ///     Return a random int within [minInclusive...maxExclusive) (Read Only)
        /// </summary>
        /// <param name="v">x is minInclusive, y is maxExclusive</param>
        /// <returns>A random integer between x(inclusive) and y(exclusive) components of input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RandomBetween(this in Vector2Int v) => Random.Range(v.x, v.y);
    }
}