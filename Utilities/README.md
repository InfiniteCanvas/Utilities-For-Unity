# Utility Classes

- [Utility Classes](#utility-classes)
  - [Disposable Wrapper](#disposable-wrapper)
  - [Addressables Loader](#addressables-loader)
  - [Addressables Label Utility (Editor)](#addressables-label-utility-editor)
  - [2D Lighting \& Sprite Utilities](#2d-lighting--sprite-utilities)
    - [ShadowCaster2D Tools](#shadowcaster2d-tools)
    - [Sprite Outline Configuration](#sprite-outline-configuration)
  - [Package Installer Utility](#package-installer-utility)
  - [RingBuffer](#ringbuffer)
  - [RingBufferSafe](#ringbuffersafe)
  - [Trigger](#trigger)
- [Extensions](#extensions)
  - [CollectionsExtensions](#collectionsextensions)
  - [Generic Extensions](#generic-extensions)
  - [Mathematics Extensions](#mathematics-extensions)
  - [Number Extensions](#number-extensions)
  - [Boolean Extensions](#boolean-extensions)
  - [Span Extensions](#span-extensions)
  - [Unity Pool Extensions](#unity-pool-extensions)
  - [Vector Extensions](#vector-extensions)
    - [View Cone Checks](#view-cone-checks)

## Disposable Wrapper

The `DisposableWrapper<T>` class allows you to wrap any object with a custom disposal action. When the wrapper is
disposed (for example, at the end of a `using` statement), the specified action is executed, ensuring proper cleanup of
resources.
<br>Useful for classes you cannot modify to inherit from ``IDisposable``.

**Key Features:**

- Generic implementation, applicable to any type
- Custom cleanup logic via a provided action
- Implements the `IDisposable` interface for deterministic resource management

**Example Usage:**

```cs
var resource = new Resource(); // Replace with your actual resource type
using (var wrapper = new DisposableWrapper<Resource>(resource, r => r.Cleanup()))
{
    // Use the resource here
}
// Upon exiting the using block, the Cleanup method is automatically called on the resource.
```

## Addressables Loader

**Key Features:**

- Loading (sync/async) with handle caching
- Batch preloading into cache
- Bulk releasing from cache
- Bulk operations use Addressables.LoadResourceLocationsAsync to get primary keys

**Usage:**

```cs
// Synchronous load
var weaponPrefab = AddressablesLoader.GetAsset("Weapons/LaserRifle");

// Async load
var asyncMaterial = await AddressablesLoader.GetAssetAsync("Materials/Holographic");

// Batch preloading
await AddressablesLoader.PreloadAssets(
    Addressables.MergeMode.Intersection, 
    "Enemies", "HQ_Textures"
);

// Release resources
AddressablesLoader.ReleaseAssets(callback: (success, key) => 
{
    Debug.Log($"Released[{success}] - {key}");
}, "Weapons", "Enemies");
```

**Performance Notes:**

- `GetAsset()` uses `WaitForCompletion` - avoid in performance-critical paths
- Preloading batches reduce runtime hitches
- Cache checking happens before Addressables API calls
    - `GetAllAssets` uses `WaitForCompletion` to get resource location, then uses `GetAsset` to load assets not in cache

## Addressables Label Utility (Editor)

**Workflow:**

1. Open via `Window > Asset Management > Addressables > Label By Type`
2. Select target type
3. Specify label and target Addressables group
4. Click "Apply Labels"

**Features:**

- Type filtering with regex support
- Automatic label creation
- Group assignment validation
- Progress tracking through console logs
- Assembly-optimized type discovery

**Usage Example:**

```cs
// Batch label all WeaponConfig assets
LabelAssetsOfType(typeof(WeaponConfig), 
    "CombatData", 
    "WeaponsGroup"
);
```

**Requirements:**

- Addressables package
- Existing Addressables group configuration

## 2D Lighting & Sprite Utilities

### ShadowCaster2D Tools

**Context menu integration for shadow shape creation:**

```cs
[MenuItem("CONTEXT/ShadowCaster2D/Copy Collider Shape")]
[MenuItem("CONTEXT/ShadowCaster2D/Copy Sprite Shape")]
```

**Features:**

- Convert PolygonCollider2D shapes to ShadowCaster2D paths
- Generate shadow shapes directly from sprites (sets casting source to ShapeEditor)

**Workflow:**

1. Right-click any ShadowCaster2D component
2. Choose between:
    - **Copy Collider Shape** - Requires PolygonCollider2D on same GameObject
    - **Copy Sprite Shape** - Requires SpriteRenderer with valid sprite

### Sprite Outline Configuration

**How To Use:**

1. Create outline settings via Assets > Create > InfiniteCanvas > Settings
2. Use `Copy Sprite Shape` for dynamic shadow updates
3. Combine with PolygonCollider2D for physics-accurate shadows

**Key parameters:**

- `Tolerance`: Outline simplification (0=simplified, 1=precise)
- `AlphaTolerance`: Transparency threshold (0-255)
- `HoleDetection`: Automatic recognition of sprite holes

## Package Installer Utility

An editor window that simplifies installation of some Unity packages:

**Access:**

```
[MenuItem("InfiniteCanvas/Package Installer")]
```

**Supported Packages:**
| Package | Description |
|---------|-------------|
| VContainer | DI container with Unity integration |
| UniTask | Async/await optimization framework |
| MessagePipe | Pub/Sub messaging system |
| MasterMemory | Embedded database solution |
| Nuget For Unity | NuGet package manager integration for Unity |
| R3 | Reactive Extensions for Unity |

**Features:**

- One-click installation of multiple packages
- That's it ᕙ(⇀‸↼‶)ᕗ

**Usage:**

1. Open through Unity's top menu
2. Select desired packages
3. Click "Add Selected Packages"
4. Wait for installation completion

## RingBuffer

The `RingBuffer` class offers a generic, efficient circular buffer with constant time operations for enqueueing,
dequeueing, peeking, and index-based access. Its capacity is automatically rounded up to the next power of two to
optimize performance.

**Example Usage:**

```cs
var buffer = new RingBuffer<int>(8);
buffer.Enqueue(1);
buffer.Enqueue(2);
var firstItem = buffer.Dequeue();
Debug.Log($"Dequeued item: {firstItem}");
```

## RingBufferSafe

The `RingBufferSafe` class provides a thread-safe variant of the ring buffer. It uses locks and monitors to ensure safe
operations across multiple threads, making it ideal for concurrent environments.
<br>*When enqueueing and dequeueing on separate threads, the Dequeue waits on Enqueue of other threads.*

**Example Usage:**

```cs
var safeBuffer = new RingBufferSafe<int>(8);
safeBuffer.Enqueue(10);
var safeItem = safeBuffer.Dequeue();
Debug.Log($"Dequeued item from safe buffer: {safeItem}");
```

## Trigger

The `Trigger` class offers a simple mechanism for one-shot boolean flags. Use the `Prime` method to set the trigger, and
the field `Fire` to atomically check and reset it, ensuring that it fires only once per cycle.

**Example Usage:**

```cs
Trigger trigger = new Trigger();
trigger.Prime();
if (trigger.Fire)
{
    Debug.Log("Trigger fired successfully!");
}
```

# Extensions

This section provides additional utility extensions that enhance the functionality of your project by offering helper
methods for common operations.

## CollectionsExtensions

**HashMap<TKey, TValue>.GetValueOrDefault:**
Returns the value or a default value.

**IEnumerable<T>.IsNullOrEmpty:**
Returns true if collection is null or empty.

## Generic Extensions

Provides a helper method for creating a disposable wrapper around objects.

**CreateDisposableWrapper:**  
Wraps any object with a cleanup action, streamlining resource management.

**Example Usage:**

```cs
var disposable = myObject.CreateDisposableWrapper(o =>
{
    // Execute cleanup actions here
});
```

## Mathematics Extensions

Offers methods for converting between Unity types, particularly for simplifying color and vector conversions.

**ToColor / ToFloat4:**  
Provides conversions between a Unity `float4` and `Color` type.

**Example Usage:**

```cs
using Unity.Mathematics;
using UnityEngine;

float4 vectorColor = new float4(1, 0, 0, 1);
Color unityColor = vectorColor.ToColor();

Color myColor = new Color(1, 0, 0, 1);
float4 floatColor = myColor.ToFloat4();
```

## Number Extensions

Introduces methods to simplify random number generation using integer or float values. Works with **[int | int2 | int3 | int4]** and
**[float | float2 | float3 | float4]**.

**RandomTo:**  
Returns a random integer between a specified minimum (default is 0) and the given maximum value.

**RandomsBetween:**  
Generates a sequence of random integers within a specified range (minimum defaults to 0).

**Example Usage:**

```cs
int randomValue = 10.RandomTo(); // Returns a random number between 0 and 10

var max = 5;
var min = 1;
foreach (var value in 10.RandomsBetween(max, min))
{
    Debug.Log(value); // value between 1 and 5
}
```

## Boolean Extensions

Simplifies boolean operations by offering straightforward methods to toggle or explicitly set boolean values.

**Toggle:**  
Inverts the current boolean value.

**True / False:**  
Directly sets the boolean value to true or false, respectively.

**Example Usage:**

```cs
bool flag = false;
flag.Toggle(); // flag becomes true
flag.False();  // flag becomes false
flag.True();   // flag becomes true
```

## Span Extensions
Adds utility functions to spans.

**IndicesOf:**
```csharp
ReadOnlySpan<char> data = "hello world".AsSpan();
var indices = data.IndicesOf('l');
// Returns [2, 3, 9] (indices of 'l' in "hello world")
```

## Unity Pool Extensions

These extensions offer helper methods to retrieve and release pooled collections using Unity's built-in pool system.

**GetList<T>**  
Retrieves a pooled list of type T and puts element into the collection.

```cs
var gameObjectList = gameObject.GetList();
```

**Release (List)**  
Releases a list back to the pool.

```cs
gameObjectList.Release();
```

**GetHashSet<T>**  
Retrieves a pooled HashSet of type T and puts element into the collection.

```cs
var gameObjectHashSet = gameObject.GetHashSet();
```

**Release (HashSet)**  
Releases a HashSet back to the pool.

```cs
gameObjectHashSet.Release();
```

## Vector Extensions

This extension adds additional functionality for Unity's `Vector2` and `Vector2Int` type.

**RandomBetween**  
Returns a random number between the x (minInclusive) and y (maxExclusive) components.

```cs
var randomFloat = Vector2.up.RandomBetween();
var randomInt = Vector2Int.up.RandomBetween();
```

### View Cone Checks

**Vector3**
<br>
Checks if a 3D point is within a view cone defined by direction, distance, and angles.

```

public static bool CouldSee(this in Vector3 origin,
in Vector3 viewDirection,
in Vector3 target,
in float viewDistance,
in float horizontalAngle,
in float verticalAngle)

**Example:**

bool couldSee = transform.position.CouldSee(
transform.forward,
enemyPosition,
maxViewDistance: 20f,
horizontalFOV: 45f,
verticalFOV: 30f
);

```

**Vector2**
<br>
2D version with angular and distance constraints.

```

public static bool CouldSee(this in Vector2 origin,
in Vector2 viewDirection,
in Vector2 target,
in float viewDistance,
in float viewAngle)

**Example:**

bool inSight = playerPosition.CouldSee(
Vector2.right,
enemyPosition,
viewDistance: 15f,
viewAngle: 60f
);

```