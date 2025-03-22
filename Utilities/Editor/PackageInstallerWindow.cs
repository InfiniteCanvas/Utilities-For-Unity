using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace InfiniteCanvas.Utilities.Editor
{
	public class PackageInstallerWindow : EditorWindow
	{
		private readonly Queue<string> _installationQueue = new();

		private readonly Dictionary<string, (string url, string description, bool selected)> _packages = new()
		{
			{
				"VContainer", (
					"https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer",
					"Lightweight DI (Dependency Injection) container for Unity.\n\n"
				  + "Features:\n"
				  + "• Fast resolve performance\n"
				  + "• Unity component support\n"
				  + "• Scoped container\n"
				  + "• Async object building\n"
				  + "• Constructor injection\n\n"
				  + "Repository: https://github.com/hadashiA/VContainer",
					false
				)
			},
			{
				"UniTask", (
					"https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
					"Provides an efficient async/await integration for Unity.\n\n"
				  + "Benefits:\n"
				  + "• Zero allocation async/await\n"
				  + "• Unity-specialized async methods\n"
				  + "• Compatible with standard async/await\n"
				  + "• Cancellation support\n\n"
				  + "Repository: https://github.com/Cysharp/UniTask",
					false
				)
			},
			{
				"MessagePipe", (
					"https://github.com/Cysharp/MessagePipe.git?path=src/MessagePipe.Unity/Assets/Plugins/MessagePipe",
					"High-performance in-memory message bus for Unity.\n\n"
				  + "Features:\n"
				  + "• Pub/Sub messaging system\n"
				  + "• Async message handling\n"
				  + "• Filtering capabilities\n"
				  + "• Memory pool optimization\n\n"
				  + "Repository: https://github.com/Cysharp/MessagePipe",
					false
				)
			},
			{
				"MessagePipe VContainer", (
					"https://github.com/Cysharp/MessagePipe.git?path=src/MessagePipe.Unity/Assets/Plugins/MessagePipe.VContainer",
					"VContainer integration for MessagePipe.\n\n"
				  + "Requires:\n"
				  + "• MessagePipe\n"
				  + "• VContainer\n\n"
				  + "Provides seamless integration between MessagePipe and VContainer DI system.\n\n"
				  + "Repository: https://github.com/Cysharp/MessagePipe",
					false
				)
			},
			{
				"Master Memory", (
					"https://github.com/Cysharp/MasterMemory.git?path=src/MasterMemory.Unity/Assets/Scripts/MasterMemory",
					"High-performance embedded database for Unity.\n\n"
				  + "Features:\n"
				  + "• Zero-allocation data queries\n"
				  + "• Binary serialization\n"
				  + "• Code generation for type-safe queries\n"
				  + "• Fast data lookup and range queries\n"
				  + "• Memory-efficient storage\n\n"
				  + "Perfect for managing game data, configurations, and master data tables.\n\n"
				  + "Repository: https://github.com/Cysharp/MasterMemory",
					false
				)
			},
			{
				"Nuget For Unity", (
					"https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity",
					"NuGet package manager integration for Unity.\n\n"
				  + "Features:\n"
				  + "• Install NuGet packages directly in Unity\n"
				  + "• Manage package versions\n"
				  + "• Automatic dependency resolution\n"
				  + "• Package restore support\n\n"
				  + "Repository: https://github.com/GlitchEnzo/NuGetForUnity",
					false
				)
			},
			{
				"Serilog Integration (Unity Sink)", (
					"https://github.com/InfiniteCanvas/Serilog-Integration.git",
					"Adds a sink for Serilog, logging to debug console.\n\n"
				  + "Features:\n"
				  + "• Adds a sink to log to Unity's debug console\n\n"
				  + "Requirements:\n"
				  + "• Serilog.dll from the Serilog NuGet package\n\n"
				  + "Repository: https://github.com/InfiniteCanvas/Serilog-Integration",
					false
				)
			},
			{
				"R3", (
					"https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity",
					"Reactive Extensions for Unity (R3).\n\n"
				  + "Features:\n"
				  + "• High-performance Rx implementation\n"
				  + "• Unity-specific optimizations\n"
				  + "• Minimal garbage collection\n"
				  + "• Enhanced operator set\n\n"
				  + "CAUTION:\n"
				  + "Install NuGet for Unity first, and install the R3 package from NuGet\n\n"
				  + "Repository: https://github.com/Cysharp/R3",
					false
				)
			},
			{
				"Health And Damage Systems", (
					"https://github.com/InfiniteCanvas/health-and-damage.git",
					"Customizable health and damage systems with modifiers.\n\n"
				  + "Features:\n"
				  + "• Fast performance with structs and jobs\n"
				  + "• Configuration with MonoBehaviours and ScriptableObjects\n"
				  + "• Integrates with VContainer and MessagePipe\n"
				  + "• Hook your own effects to the system\n"
				  + "• Sends messages for stages of doing damage and healing for custom hooks\n\n"
				  + "Repository: https://github.com/InfiniteCanvas/health-and-damage",
					false
				)
			},
			{
				"Pooling Utility", (
					"https://github.com/InfiniteCanvas/Pooling-Utility.git",
					"A source generator package that automatically adds object pooling functionality to any class marked with the `[Pooled]` attribute.\n\n"
				  + "Adds these static methods to marked classes:\n"
				  + "• `static T Get()` - Get an instance from the pool\n"
				  + "• `static PooledObject<T> Get(out T instance)` - Get an instance with IDisposable support\n"
				  + "• `static void Release(T instance)` - Return an instance to the pool\n"
				  + "• `static void Clear()` - Clear the pool\n"
				  + "Repository: https://github.com/InfiniteCanvas/Pooling-Utility.git",
					false
				)
			},
			{
				"Ink Unity Integration", (
					"https://github.com/inkle/ink-unity-integration.git#upm",
					"Integrates inkle's ink narrative scripting language with Unity.\n\n"
				  + "Features:\n"
				  + "• Compile, play and debug ink stories\n"
				  + "• Auto-compilation of ink files to JSON\n"
				  + "• Powerful Ink Player Window for testing and debugging\n"
				  + "• Inspector tools with custom icons and information\n"
				  + "• C# runtime API for controlling stories in-game\n\n"
				  + "Repository: https://github.com/inkle/ink-unity-integration",
					false
				)
			},
			{
				"Custom Ink Integration", (
					"https://github.com/InfiniteCanvas/Ink-Integration.git",
					"Uses `Ink Unity Integration` to integrate ink narrative scripting language with Unity, using MessagePipe to step through the story.\n\n"
				  + "Features:\n"
				  + "• Step through narrative content with a message-based architecture\n"
				  + "• Handle choices and branching narratives\n"
				  + "• Process special commands for audio, animation, UI, and scene management\n"
				  + "• Separation of concerns using dependency injection with VContainer\n"
				  + "Requirements:\n"
				  + "• Ink Unity Integration package installed\n"
				  + "Repository: https://github.com/InfiniteCanvas/Ink-Integration.git",
					false
				)
			},
		};

		private readonly Dictionary<string, bool> _pendingChanges = new();
		private          AddAndRemoveRequest      _installRequest;
		private          bool                     _isInstalling;
		private          Vector2                  _scrollPosition;
		private          string                   _selectedPackageKey;

	#region Event Functions

		private void OnGUI()
		{
			EditorGUILayout.Space(10);

			// Left side: Package list
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f));

			DrawPackageList();

			EditorGUILayout.EndVertical();

			// Right side: Package description
			EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.6f));

			DrawPackageDescription();

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(10);

			DrawBottomControls();
		}

	#endregion

		private void DrawPackageList()
		{
			EditorGUILayout.LabelField("Available Packages", EditorStyles.boldLabel);
			EditorGUILayout.Space(5);

			GUI.enabled = !_isInstalling;
			if (GUILayout.Button("Select All"))
				foreach (var package in _packages
					        .ToList())
					_pendingChanges[package.Key] = true;

			if (GUILayout.Button("Deselect All"))
				foreach (var package in _packages
					        .ToList())
					_pendingChanges[package.Key] = false;

			EditorGUILayout.Space(5);

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			foreach (var package in
			         _packages.ToList())
			{
				EditorGUILayout.BeginHorizontal();

				// check if key exists in changes
				var currentValue = _pendingChanges.TryGetValue(package.Key, out var pendingChange)
					? pendingChange
					: package.Value.selected;

				var newSelected = EditorGUILayout.ToggleLeft(package.Key, currentValue, GUILayout.Width(200));
				if (newSelected != currentValue)
				{
					_pendingChanges[package.Key] = newSelected;
					_selectedPackageKey = package.Key;
				}

				if (GUILayout.Button("Info", GUILayout.Width(50)))
					_selectedPackageKey = package.Key;

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndScrollView();
		}

		private void DrawPackageDescription()
		{
			EditorGUILayout.LabelField("Package Information", EditorStyles.boldLabel);
			EditorGUILayout.Space(5);

			if (string.IsNullOrEmpty(_selectedPackageKey))
			{
				EditorGUILayout.HelpBox("Select a package to view its description.", MessageType.Info);
				return;
			}

			if (!_packages.TryGetValue(_selectedPackageKey, out var package)) return;
			EditorGUILayout.LabelField(_selectedPackageKey, EditorStyles.boldLabel);
			EditorGUILayout.Space(5);
			EditorGUILayout.HelpBox(package.description, MessageType.None);
		}

		private void DrawBottomControls()
		{
			// Apply pending changes
			if (_pendingChanges.Count > 0)
			{
				foreach (var change in _pendingChanges)
				{
					if (_packages.ContainsKey(change.Key))
						_packages[change.Key] = (
							_packages[change.Key].url, _packages[change.Key].description, change.Value);
				}

				_pendingChanges.Clear();
			}

			GUI.enabled = !_isInstalling && _packages.Any(p => p.Value.selected);
			if (GUILayout.Button("Add Selected Packages"))
				InstallSelectedPackages();

			// GUI.enabled = true;
			if (!_isInstalling) return;
			EditorGUILayout.Space(5);
			EditorGUILayout.HelpBox("Installing packages... Please wait.", MessageType.Info);
			CheckAddRequest();
		}

		[MenuItem("Tools/Package Installer")]
		public static void ShowWindow()
		{
			var window = GetWindow<PackageInstallerWindow>();
			window.titleContent = new GUIContent("Package Installer");
			window.minSize = new Vector2(650, 400);
			window.Show();
		}

		private void InstallSelectedPackages()
		{
			_installationQueue.Clear();
			var selectedPackages =
				_packages.Where(p => p.Value.selected).ToList();
			foreach (var (_, (url, _, _)) in selectedPackages)
			{
				_installationQueue.Enqueue(url);
				if (url.Contains("VContainer"))
					GetWindow<VContainerSourceGeneratorDownloaderWindow>("VContainer Source Generator DLL Downloader");
			}

			if (_installationQueue.Count <= 0) return;
			Debug.Log($"Installing {string.Join(", ", selectedPackages.Select(p => p.Key))}");
			_isInstalling = true;
			_installRequest = Client.AddAndRemove(_installationQueue.ToArray());
		}

		private void CheckAddRequest()
		{
			if (!_installRequest.IsCompleted) return;
			if (_installRequest.Status == StatusCode.Success)
				Debug.Log("Packages installed successfully");
			if (_installRequest.Status >= StatusCode.Failure)
				Debug.LogError($"Package installation failed: {_installRequest.Error.message}");

			_isInstalling = false;
		}
	}
}