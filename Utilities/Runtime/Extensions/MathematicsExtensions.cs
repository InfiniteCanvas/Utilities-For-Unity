using Unity.Mathematics;
using UnityEngine;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class MathematicsExtensions
    {
        public static Color ToColor(this float4 floats) => new(floats.x, floats.y, floats.z, floats.w);

        public static float4 ToFloat4(this Color color) => new(color.r, color.g, color.b, color.a);
    }
}