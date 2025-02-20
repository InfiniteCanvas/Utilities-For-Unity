using System;
using Unity.Collections;

namespace InfiniteCanvas.Utilities.Extensions
{
    public static class CollectionsExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this NativeHashMap<TKey, TValue> map, TKey key)
            where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
        {
            return map.TryGetValue(key, out var value) ? value : default;
        }
    }
}