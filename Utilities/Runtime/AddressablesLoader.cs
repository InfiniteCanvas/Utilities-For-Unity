using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace InfiniteCanvas.Utilities
{
	/// <summary>
	///     Provides sync and async loading capabilities for Addressable assets with handle-based caching.
	/// </summary>
	/// <remarks>
	///     This class simplifies the process of loading Addressable assets by providing synchronous methods
	///     and automatic handle caching.
	///     Related Unity Documentation:
	///     - Addressables System: https://docs.unity3d.com/Packages/com.unity.addressables@latest
	///     - Synchronous Addressables:
	///     https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/SynchronousAddressables.html
	///     - Memory Management:
	///     https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/MemoryManagement.html
	/// </remarks>
	public static class AddressablesLoader
	{
		/// <summary>
		///     Cache of operation handles for proper release management.
		///     Key: Addressable asset key, Value: AsyncOperationHandle
		/// </summary>
		private static readonly Dictionary<string, AsyncOperationHandle> _handleCache = new();

		/// <summary>
		///     Synchronously loads and returns an asset of type T from the Addressables system.
		/// </summary>
		/// <typeparam name="T">The type of asset to load</typeparam>
		/// <param name="key">The addressable key for the asset to load</param>
		/// <returns>The loaded asset of type T, or null if loading failed</returns>
		/// <remarks>
		///     This method will first check the cache before attempting to load from Addressables.
		///     Warning: Using WaitForCompletion() can cause frame drops in builds.
		///     Related Documentation:
		///     - LoadAssetAsync:
		///     https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/LoadingAddressableAssets.html
		/// </remarks>
		public static T GetAsset<T>(string key) where T : class
		{
			if (_handleCache.TryGetValue(key, out var handle)) return handle.Result as T;

			var op = Addressables.LoadAssetAsync<T>(key);
			op.WaitForCompletion();

			if (op.Status != AsyncOperationStatus.Succeeded || op.Result == null)
			{
				Debug.LogError($"[AddressablesLoader] Failed to load asset of type {typeof(T)} with key: {key}");
				op.Release();
				return null;
			}

			_handleCache[key] = op;
			return op.Result;
		}

		/// <summary>
		///     Asynchronously loads and returns an asset of type T from the Addressables system.
		/// </summary>
		/// <typeparam name="T">The type of asset to load</typeparam>
		/// <param name="key">The addressable key for the asset to load</param>
		/// <returns>The loaded asset of type T, or null if loading failed</returns>
		/// <remarks>
		///     This method will first check the cache before attempting to load from Addressables.
		///     Related Documentation:
		///     - LoadAssetAsync:
		///     https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/LoadingAddressableAssets.html
		/// </remarks>
		public static async Task<T> GetAssetAsync<T>(string key) where T : class
		{
			if (_handleCache.TryGetValue(key, out var handle)) return handle.Result as T;

			var op = Addressables.LoadAssetAsync<T>(key);
			await op.Task;

			if (op.Status != AsyncOperationStatus.Succeeded || op.Result == null)
			{
				Debug.LogError($"[AddressablesLoader] Failed to load asset of type {typeof(T)} with key: {key}");
				op.Release();
				return null;
			}

			_handleCache[key] = op;
			return op.Result;
		}

		/// <summary>
		///     Loads all assets by keys/labels using
		///     <see
		///         cref="Addressables.LoadResourceLocationsAsync(System.Collections.IEnumerable, Addressables.MergeMode, System.Type)" />
		///     .
		/// </summary>
		/// <param name="mergeMode">The mode for merging the results of the found locations.</param>
		/// <param name="keys">Keys/labels to find resource locations</param>
		/// <typeparam name="T">Type of asset to load</typeparam>
		/// <returns>List of matching assets</returns>
		/// <remarks>
		///     Hits the cache first, then loads from Addressables if it misses.
		/// </remarks>
		public static IEnumerable<T> GetAllAssets<T>(Addressables.MergeMode mergeMode, params string[] keys)
			where T : class
		{
			var handle = Addressables.LoadResourceLocationsAsync(keys, mergeMode, typeof(T));
			handle.WaitForCompletion();
			foreach (var resource in handle.Result) yield return GetAsset<T>(resource.PrimaryKey);
		}

		/// <summary>
		///     Loads all assets by keys/labels using
		///     <see
		///         cref="Addressables.LoadResourceLocationsAsync(System.Collections.IEnumerable, Addressables.MergeMode, System.Type)" />
		///     .
		/// </summary>
		/// <param name="mergeMode">The mode for merging the results of the found locations.</param>
		/// <param name="keys">Keys/labels to find resource locations</param>
		/// <typeparam name="T">Type of asset to load</typeparam>
		/// <returns>List of matching assets</returns>
		/// <remarks>
		///     Hits the cache first, then loads from Addressables if it misses.
		/// </remarks>
		public static async IAsyncEnumerable<T> GetAllAssetsAsync<T>(Addressables.MergeMode mergeMode,
		                                                             params string[]        keys)
			where T : class
		{
			var resources = await Addressables.LoadResourceLocationsAsync(keys, mergeMode, typeof(T)).Task;
			foreach (var resource in resources) yield return await GetAssetAsync<T>(resource.PrimaryKey);
		}

		/// <summary>
		///     Preloads multiple assets of type T into the cache using async loading.
		///     Uses to this method to find assets by key/label
		///     <see cref="Addressables.LoadResourceLocationsAsync(System.Collections.IEnumerable, Addressables.MergeMode, System.Type)" />
		/// </summary>
		/// <typeparam name="T">The type of assets to preload</typeparam>
		/// <param name="mergeMode">The mode for merging the results of the found locations.</param>
		/// <param name="keys">Array of addressable keys or labels to preload</param>
		/// <remarks>You can use <see cref="Task{TResult}.Wait()" /> to wait in sync, but it's not recommended.</remarks>
		public static async Task PreloadAssets<T>(Addressables.MergeMode mergeMode, params string[] keys)
			where T : class
		{
			var resourceLocations = await Addressables
			                             .LoadResourceLocationsAsync(keys,
			                                                         mergeMode,
			                                                         typeof(T))
			                             .Task;
			var resources = resourceLocations.Where(key => !_handleCache.ContainsKey(key.PrimaryKey)).ToList();
			if (!resources.Any()) return;

			var tasks = new List<Task>();
			foreach (var key in resources)
			{
				var op = Addressables.LoadAssetAsync<T>(key);
				_handleCache[key.PrimaryKey] = op;
				tasks.Add(op.Task);
			}

			await Task.WhenAll(tasks);

			foreach (var key in resources)
			{
				var op = _handleCache[key.PrimaryKey];
				if (op.Status == AsyncOperationStatus.Succeeded) continue;
				Debug.LogError($"[AddressablesLoader] Failed to preload {key}");
				ReleaseAsset(key.PrimaryKey);
			}
		}

		/// <summary>
		///     Releases a specific asset from the cache.
		/// </summary>
		/// <param name="key">The address of the asset to release</param>
		/// <returns>True if the asset was found and released, false otherwise</returns>
		public static bool ReleaseAsset(string key)
		{
			if (!_handleCache.TryGetValue(key, out var handle)) return false;
			handle.Release();
			_handleCache.Remove(key);
			return true;
		}

		/// <summary>
		///     Releases assets from the cache by keys/labels using
		///     <see
		///         cref="Addressables.LoadResourceLocationsAsync(System.Collections.IEnumerable, Addressables.MergeMode, System.Type)" />
		///     .
		/// </summary>
		/// <param name="mergeMode">The mode for merging the results of the found locations.</param>
		/// <param name="callback">Calls with True if the asset was found and released, false otherwise</param>
		/// <param name="keys">The labels of assets to be released</param>
		/// <remarks>This method will block the thread!</remarks>
		public static void ReleaseAssets(Addressables.MergeMode mergeMode = Addressables.MergeMode.Union,
		                                 Action<bool, string>   callback  = null,
		                                 params string[]        keys)
		{
			var resourceLocations = Addressables.LoadResourceLocationsAsync(keys, mergeMode).WaitForCompletion();
			foreach (var resourceLocation in resourceLocations)
			{
				var key = resourceLocation.PrimaryKey;
				callback?.Invoke(ReleaseAsset(key), key);
			}
		}

		/// <summary>
		///     Clears the cache and releases all loaded assets back to the Addressables system.
		/// </summary>
		public static void ClearCache()
		{
			foreach (var handle in _handleCache.Values) handle.Release();

			_handleCache.Clear();
		}
	}
}