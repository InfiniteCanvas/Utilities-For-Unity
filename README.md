# Unity Utilities

Some utilities and other things I usually need and ended up writing again and again when using Unity.

## Installation

### Via Unity Package Manager
1. Open the Package Manager window in Unity
2. Click the + button and select "Add package from git URL"
3. Enter: `https://github.com/InfiniteCanvas/Unity-Utilities.git`

### Via manifest.json
Add this line to your `manifest.json` under the dependencies section:
```json
{
  "dependencies": {
    "io.infinitecanvas.unityutilities": "https://github.com/InfiniteCanvas/Unity-Utilities.git"
  }
}
```

## Requirements

- Unity 6000.0.1 or later
- Addressables package installed

## [Utility Classes](./Utilities/README.md)

Classes and extension methods that make life easier in Unity.

## [Pooling](./Analyzers/Pooling/README.md)

Use the [Pooled] attribute on any instantiatable class to add static methods that handles pooling! It uses Unity's own [pooling solution](https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html).

## Contributing

Feel free to submit issues and enhancement requests!

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/InfiniteCanvas/Unity-Utilities).

---

Made with ❤️ by Infinite Canvas