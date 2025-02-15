using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class NumberExtensions
    {
        private static Random _random = new(42);

    #region ints

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RandomTo(this int exclusiveMax, int inclusiveMin = 0)
            => _random.NextInt(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int> RandomsBetween(this int amount, int exclusiveMax, int inclusiveMin = 0)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextInt(inclusiveMin, exclusiveMax);
        }

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 RandomTo(this int2 exclusiveMax, int2 inclusiveMin = default)
            => _random.NextInt2(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int2> RandomsBetween(this int amount, int2 exclusiveMax, int2 inclusiveMin = default)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextInt2(inclusiveMin, exclusiveMax);
        }

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 RandomTo(this int3 exclusiveMax, int3 inclusiveMin = default)
            => _random.NextInt3(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int3> RandomsBetween(this int amount, int3 exclusiveMax, int3 inclusiveMin = default)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextInt3(inclusiveMin, exclusiveMax);
        }

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 RandomTo(this int4 exclusiveMax, int4 inclusiveMin = default)
            => _random.NextInt4(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<int4> RandomsBetween(this int amount, int4 exclusiveMax, int4 inclusiveMin = default)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextInt4(inclusiveMin, exclusiveMax);
        }

    #endregion

    #region floats

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomTo(this float exclusiveMax, float inclusiveMin = 0)
            => _random.NextFloat(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float> RandomsBetween(this float amount, float exclusiveMax, float inclusiveMin = 0)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextFloat(inclusiveMin, exclusiveMax);
        }

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 RandomTo(this float2 exclusiveMax, float2 inclusiveMin = default)
            => _random.NextFloat2(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float2> RandomsBetween(this int amount,
                                                         float2   exclusiveMax,
                                                         float2   inclusiveMin = default)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextFloat2(inclusiveMin, exclusiveMax);
        }

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RandomTo(this float3 exclusiveMax, float3 inclusiveMin = default)
            => _random.NextFloat3(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float3> RandomsBetween(this int amount,
                                                         float3   exclusiveMax,
                                                         float3   inclusiveMin = default)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextFloat3(inclusiveMin, exclusiveMax);
        }

        /// <summary>
        ///     Generate random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random number between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 RandomTo(this float4 exclusiveMax, float4 inclusiveMin = default)
            => _random.NextFloat4(inclusiveMin, exclusiveMax);

        /// <summary>
        ///     Generate <see cref="amount" /> of random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]
        /// </summary>
        /// <param name="amount">Amount of numbers to generate</param>
        /// <param name="exclusiveMax">Excluded highest random possible number</param>
        /// <param name="inclusiveMin">Included lowest random possible number</param>
        /// <returns>Random numbers between [<see cref="inclusiveMin" />...<see cref="exclusiveMax" />]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<float4> RandomsBetween(this int amount,
                                                         float4   exclusiveMax,
                                                         float4   inclusiveMin = default)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextFloat4(inclusiveMin, exclusiveMax);
        }

    #endregion
    }
}