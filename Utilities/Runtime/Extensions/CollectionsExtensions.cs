using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace InfiniteCanvas.Utilities.Extensions
{
	public static class CollectionsExtensions
	{
		public static TValue GetValueOrDefault<TKey, TValue>(this NativeHashMap<TKey, TValue> map, TKey key, TValue defaultValue = default)
			where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
			=> map.TryGetValue(key, out var value) ? value : defaultValue;

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
		{
			if (collection == null) return true;
			return !collection.Any();
		}
	}
}