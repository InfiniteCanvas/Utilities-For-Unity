# Changelog

## [2.0.0] - 2025-03-19
### Changed
- Renamed Repo and Package to clarify it's not a package FROM Unity (name was ambiguous)

## [1.5.0] - 2025-03-19
## Added
- `IEnumerable<T>.IsNullOrEmpty` shorthand check for collections

## Changed
- Removed side effects of clearing collections before returning them to the pool in `UnityPoolExtensions`
- Changed `HashMap<TKey, TValue>.GetValueOrDefault` to return specified default value, if provided

## [1.4.0] - 2025-03-18
### Removed
- Pooling Utility relocated to its own package. Install with ```https://github.com/InfiniteCanvas/Pooling-Utility.git```

### Added
- VContainer Source Generator Dll downloader
- Packages to PackageInstaller
  - Pooling Utility
  - Ink Integration

### Changed
- Relocated menu items of all editor windows to `Tools`

## [1.3.0] - 2025-02-20
### Added
- AddressablesLabelUtility now has a preview of affected assets
- `GetValueOrDefault` extension for NativeHashMap

### Fixed
- PoolExtensions now correctly return a collection containing the referenced item used for the extensions method
- Wording in AddressablesLabelUtility

## [1.2.0] - 2025-02-19

### Added
- AddressablesLabelUtility can add addressable labels to assets
  - filter for assets by type and some other parameters using unity's search
### Changed
- AddressablesLoader reworked now works better and more consistent by using ResourceLocations

## [1.1.1] - 2025-02-16
### Changed
- root name spaces in .asmdef files
- bool extensions now return the result of the operation, allowing you to do things like this:
  ```csharp
  var a = false;
  if (a.Toggle()) //true
  {
      // do something
  }
  ```
  - while it changes the passed variable, it returns a value copy for evaluation

## [1.1.0] - 2025-02-16
### Added
- **Vector Utilities**
  - `CouldSee` extension methods for Vector3 and Vector2
  - Relevant docs

### Changed
- added `in` keywords for number extensions

## [1.0.1] - 2025-02-15
### Added
- extensions for generating random numbers from Unity.Mathematics number structs

### Changed
- changed **RandomsTo** -> **RandomsBetween** for more clarity, since it uses the first parameter as the **amount**
- changed IntExtensions to NumberExtensions

## [1.0.0] - 2025-02-15
### Added
- **Core Utilities:**
  - `DisposableWrapper` for custom resource cleanup
  - `RingBuffer` and thread-safe `RingBufferSafe`
  - `Trigger` class for one-shot boolean logic
- **2D Tools:**
  - ShadowCaster2D shape conversion (collider/sprite paths)
  - Sprite outline generation with configurable settings
- **Addressables Management:**
  - Synchronous/asynchronous asset loading
  - Preloading and cache management
- **Editor Utilities:**
  - Package Installer Window for third-party dependencies
  - GUI interface for VContainer/UniTask/MessagePipe installation
- **Extensions:**
  - Mathematics conversions (Color <-> float4)
  - Pooled collection helpers (List/HashSet)
  - More comfortable randomization methods
- **Pooling System:**
  - `[Pooled]` attribute for automatic object pooling
  - Custom pool configuration with creation/destruction actions
- **Package Infrastructure:**
  - UPM installation support
  - Unity 6000.0.1 compatibility