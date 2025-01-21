using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfiniteCanvas.Utilities
{
    /// <summary>
    /// Provides synchronous loading capabilities for Addressable assets with handle-based caching.
    /// </summary>
    /// <remarks>
    /// This class simplifies the process of loading Addressable assets by providing synchronous methods
    /// and automatic handle caching for proper memory management.
    /// 
    /// Related Unity Documentation:
    /// - Addressables System: https://docs.unity3d.com/Packages/com.unity.addressables@latest
    /// - Synchronous Addressables: https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/SynchronousAddressables.html
    /// - Memory Management: https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/MemoryManagement.html
    /// </remarks>
    public static class AddressablesLoader
    {
        /// <summary>
        /// Cache of operation handles for proper release management.
        /// Key: Addressable asset key, Value: AsyncOperationHandle
        /// </summary>
        private static readonly Dictionary<string, AsyncOperationHandle> _handleCache = new();

        /// <summary>
        /// Synchronously loads and returns an asset of type T from the Addressables system.
        /// </summary>
        /// <typeparam name="T">The type of asset to load</typeparam>
        /// <param name="key">The addressable key for the asset to load</param>
        /// <returns>The loaded asset of type T, or null if loading failed</returns>
        /// <remarks>
        /// This method will first check the cache before attempting to load from Addressables.
        /// Warning: Using WaitForCompletion() can cause frame drops in builds.
        /// 
        /// Related Documentation:
        /// - LoadAssetAsync: https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/LoadingAddressableAssets.html
        /// </remarks>
        public static T GetAsset<T>(string key) where T : class
        {
            // Check if we already have a handle cached
            if (_handleCache.TryGetValue(key, out AsyncOperationHandle handle))
            {
                return handle.Result as T;
            }

            // Create new load operation
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
        /// Asynchronously loads and returns an asset of type T from the Addressables system.
        /// </summary>
        /// <typeparam name="T">The type of asset to load</typeparam>
        /// <param name="key">The addressable key for the asset to load</param>
        /// <returns>The loaded asset of type T, or null if loading failed</returns>
        /// <remarks>
        /// This method will first check the cache before attempting to load from Addressables.
        /// 
        /// Related Documentation:
        /// - LoadAssetAsync: https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/LoadingAddressableAssets.html
        /// </remarks>
        public static async Task<T> GetAssetAsync<T>(string key) where T : class
        {
            // Check if we already have a handle cached
            if (_handleCache.TryGetValue(key, out AsyncOperationHandle handle))
            {
                return handle.Result as T;
            }

            // Create new load operation
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
        /// Preloads multiple assets of type T into the cache using multithreaded loading.
        /// </summary>
        /// <typeparam name="T">The type of assets to preload</typeparam>
        /// <param name="keys">Array of addressable keys to preload</param>
        /// <remarks>
        /// Uses LoadAssetsAsync for parallel loading of multiple assets, improving performance
        /// compared to loading assets individually.
        /// 
        /// Related Documentation:
        /// - Loading Multiple Assets: https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/manual/LoadingAddressableAssets.html#loading-multiple-assets
        /// </remarks>
        public static async Task PreloadAssets<T>(Action<T> callback = null, params string[] keys) where T : class
        {
            // Filter out keys that are already cached
            var keysToLoad = keys.Where(key => !_handleCache.ContainsKey(key)).ToList();
            if (!keysToLoad.Any()) return;

            // Load all assets in parallel
            var tasks = new List<Task>();
            foreach (var key in keysToLoad)
            {
                var op = Addressables.LoadAssetAsync<T>(key);
                _handleCache[key] = op;
                tasks.Add(op.Task);
            }

            await Task.WhenAll(tasks);

            foreach (var key in keysToLoad)
            {
                var op = _handleCache[key];
                if (op.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"[AddressablesLoader] Failed to preload {key}");
                    ReleaseAsset(key);
                }
            }
        }

        /// <summary>
        /// Releases a specific asset from the cache.
        /// </summary>
        /// <param name="key">The addressable key of the asset to release</param>
        /// <returns>True if the asset was found and released, false otherwise</returns>
        public static bool ReleaseAsset(string key)
        {
            if (!_handleCache.TryGetValue(key, out var handle)) return false;
            handle.Release();
            _handleCache.Remove(key);
            return true;
        }

        /// <summary>
        /// Clears the cache and releases all loaded assets back to the Addressables system.
        /// </summary>
        /// <remarks>
        /// Call this method during scene transitions or when assets are no longer needed
        /// to prevent memory leaks.
        /// 
        /// Related Documentation:
        /// - Release: https://docs.unity3d.com/Packages/com.unity.addressables@latest/index.html?subfolder=/api/UnityEngine.AddressableAssets.Addressables.Release.html
        /// </remarks>
        public static void ClearCache()
        {
            foreach (var handle in _handleCache.Values)
            {
                handle.Release();
            }

            _handleCache.Clear();
        }
    }
}