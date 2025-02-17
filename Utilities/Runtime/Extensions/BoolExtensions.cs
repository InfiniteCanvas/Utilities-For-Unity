using System.Runtime.CompilerServices;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class BoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Toggle(this ref bool b) => b = !b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool True(this ref bool b) => b = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool False(this ref bool b) => b = false;
    }
}