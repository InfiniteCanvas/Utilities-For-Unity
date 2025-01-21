# Unity Addressables Loader

A utility class for managing Unity Addressables with synchronous and asynchronous loading methods. This loader provides caching, handle management, and preloading.

## Features

- Wrappers to load synchronously and asynchronously
- Automatic handle caching
- Preload into cache

## Usage

### Basic Asset Loading

```csharp
// Synchronously load a GameObject
GameObject prefab = AddressablesLoader.GetAsset<GameObject>("PrefabKey");
if (prefab != null)
{
    Instantiate(prefab);
}

// Load a material
Material material = AddressablesLoader.GetAsset<Material>("MaterialKey");
```

### Async Loading with Task

```csharp
public class GameManager : MonoBehaviour
{
    private async Task LoadGameAssets()
    {
        try
        {
            await AddressablesLoader.PreloadAssetsAsync<GameObject>(
                "PlayerPrefab",
                "EnemyPrefab",
                "WeaponPrefab"
            );
            Debug.Log("All assets loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load assets: {e}");
        }
    }
}
```

### Cleanup

```csharp
// Release a single asset
AddressablesLoader.ReleaseAsset("PlayerPrefab");

// Clear all cached assets
AddressablesLoader.ClearCache();
```

## Best Practices

1. **Memory Management**
   - Call `ClearCache()` when transitioning between scenes (or use `ReleaseAsset(address)` a bunch if you want to reuse assets)
   - Release individual assets when no longer needed
   - Use preloading during loading screens or scene transitions

2. **Performance**
   - Use async loading when possible to avoid frame drops
   - Preload assets in batches rather than individual loads
   - Consider using the async Task-based approach for modern Unity versions

3. **Error Handling**
   - Always check for null when getting assets
   - Monitor logs for loading failures

## API Reference

### Methods

#### `GetAsset<T>`
```csharp
public static T GetAsset<T>(string key) where T : class
```
Synchronously loads and returns an asset from the cache or Addressables system.

#### `GetAssetAsync<T>`
```csharp
public static async Task<T> GetAsset<T>(string key) where T : class
```
Asynchronously loads and returns an asset from the cache or Addressables system.

#### `PreloadAssetsAsync<T>`
```csharp
public static Task PreloadAssetsAsync<T>(params string[] keys) where T : class
```
Asynchronously preloads multiple assets using Task-based async/await pattern.

#### `ReleaseAsset`
```csharp
public static bool ReleaseAsset(string key)
```
Releases a specific asset from the cache.

#### `ClearCache`
```csharp
public static void ClearCache()
```
Releases all cached assets and clears the cache.