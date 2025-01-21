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

## [Pooling Utility](./Analyzers/Pooling/README.md)

Use the [Pooled] attribute on any instantiatable class to add static methods that handles pooling! It uses Unity's own [pooling solution](https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html).


## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/InfiniteCanvas/Unity-Utilities).
