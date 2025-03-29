# Utilities For Unity

Some utilities and other things I usually need and ended up writing again and again when using Unity.

## Installation

### Via Unity Package Manager

1. Open the Package Manager window in Unity
2. Click the + button and select "Add package from git URL"
3. Enter: `https://github.com/InfiniteCanvas/Utilities-For-Unity.git`

### Via manifest.json

Add this line to your `manifest.json` under the dependencies section:

```json
{
  "dependencies": {
    "io.infinitecanvas.unityutilities": "https://github.com/InfiniteCanvas/Utilities-For-Unity.git"
  }
}
```

## Requirements

- Unity 6000.0.1 or later
- Addressables package installed

## [Utility Classes](./Utilities/README.md)

Classes and extension methods that make life easier in Unity.

- [Utility Classes](./Utilities/README.md#utility-classes)
    - [Disposable Wrapper](./Utilities/README.md#disposable-wrapper)
    - [Addressables Loader](./Utilities/README.md#addressables-loader)
    - [Addressables Label Utility (Editor)](./Utilities/README.md#addressables-label-utility-editor)
    - [2D Lighting \& Sprite Utilities](./Utilities/README.md#2d-lighting--sprite-utilities)
        - [ShadowCaster2D Tools](./Utilities/README.md#shadowcaster2d-tools)
        - [Sprite Outline Configuration](./Utilities/README.md#sprite-outline-configuration)
    - [Package Installer Utility](./Utilities/README.md#package-installer-utility)
    - [RingBuffer](./Utilities/README.md#ringbuffer)
    - [RingBufferSafe](./Utilities/README.md#ringbuffersafe)
    - [Trigger](./Utilities/README.md#trigger)
- [Extensions](./Utilities/README.md#extensions)
    - [CollectionsExtensions](./Utilities/README.md#collectionsextensions)
    - [Generic Extensions](./Utilities/README.md#generic-extensions)
    - [Mathematics Extensions](./Utilities/README.md#mathematics-extensions)
    - [Number Extensions](./Utilities/README.md#number-extensions)
    - [Custom Hash Extensions](./Utilities/README.md#custom-hash-extensions)
    - [Boolean Extensions](./Utilities/README.md#boolean-extensions)
    - [Span Extensions](./Utilities/README.md#span-extensions)
    - [Unity Pool Extensions](./Utilities/README.md#unity-pool-extensions)
    - [Vector Extensions](./Utilities/README.md#vector-extensions)
        - [View Cone Checks](./Utilities/README.md#view-cone-checks)

## Contributing

Feel free to submit issues and enhancement requests!

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

Made with ❤️ by Infinite Canvas