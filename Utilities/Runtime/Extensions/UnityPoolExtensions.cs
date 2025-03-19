using System.Collections.Generic;
using UnityEngine.Pool;

namespace InfiniteCanvas.Utilities.Extensions
{
	public static class UnityPoolExtensions
	{
		public static List<T> GetList<T>(this T element)
		{
			var collection = ListPool<T>.Get();
			collection.Add(element);
			return collection;
		}

		public static void Release<T>(this List<T> collection)
		{
			ListPool<T>.Release(collection);
		}

		public static HashSet<T> GetHashSet<T>(this T element)
		{
			var collection = HashSetPool<T>.Get();
			collection.Add(element);
			return collection;
		}

		public static void Release<T>(this HashSet<T> collection)
		{
			HashSetPool<T>.Release(collection);
		}
	}
}