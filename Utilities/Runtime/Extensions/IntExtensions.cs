using System.Collections.Generic;
using Unity.Mathematics;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class IntExtensions
    {
        private static Random _random = new(42);

        public static int RandomTo(this int excludedMax, int includedMin = 0)
            => _random.NextInt(includedMin, excludedMax);

        public static IEnumerable<int> RandomsTo(this int excludedMax, int amount)
        {
            for (var i = 0; i < amount; i++)
                yield return _random.NextInt(excludedMax);
        }
    }
}