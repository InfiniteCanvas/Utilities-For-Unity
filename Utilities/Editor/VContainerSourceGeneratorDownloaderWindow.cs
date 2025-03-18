using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace InfiniteCanvas.Utilities.Editor
{
	public class VContainerSourceGeneratorDownloaderWindow : EditorWindow
	{
		private string   _dllName       = "VContainer.SourceGenerator.dll";
		private string[] _releases      = Array.Empty<string>();
		private string   _repositoryUrl = "https://github.com/hadashiA/VContainer";
		private string   _savePath      = "Assets/Plugins/";
		private int      _selectedReleaseIndex;

	#region Event Functions

		private void OnEnable() => FetchReleases();

		private void OnGUI()
		{
			GUILayout.Label("Download VContainer.SourceGenerator.dll from GitHub",                                          EditorStyles.boldLabel);
			GUILayout.Label("Downloads the source generator dll for VContainer and labels it appropriately for it to work", EditorStyles.helpBox);

			EditorGUI.BeginChangeCheck();
			_repositoryUrl = EditorGUILayout.TextField("Repository URL:", _repositoryUrl);
			if (EditorGUI.EndChangeCheck()) FetchReleases();

			_dllName = EditorGUILayout.TextField("DLL Name:",   _dllName);
			_savePath = EditorGUILayout.TextField("Save Path:", _savePath);

			if (_releases.Length > 0) _selectedReleaseIndex = EditorGUILayout.Popup("Release:", _selectedReleaseIndex, _releases);
			else EditorGUILayout.LabelField("Release:", "No releases found");

			EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(_releases.Length == 0);
			if (GUILayout.Button("Download and Setup DLL")) DownloadAndSetupDll();

			EditorGUI.EndDisabledGroup();

			if (GUILayout.Button("Refresh Releases")) FetchReleases();
		}

	#endregion

		[MenuItem("Tools/VContainer Source Generator DLL Downloader")]
		public static void ShowWindow() => GetWindow<VContainerSourceGeneratorDownloaderWindow>("VContainer Source Generator DLL Downloader");

		private void FetchReleases()
		{
			try
			{
				// Extract owner and repo from URL
				var match = Regex.Match(_repositoryUrl, @"github\.com/([^/]+)/([^/]+)");
				if (!match.Success)
				{
					Debug.LogError("Invalid GitHub repository URL");
					return;
				}

				var owner = match.Groups[1].Value;
				var repo = match.Groups[2].Value;

				var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases";

				using (var client = new WebClient())
				{
					// GitHub API requires a user agent
					client.Headers.Add("User-Agent", "Unity-GitHub-Release-Downloader");
					var json = client.DownloadString(apiUrl);

					// Simple parsing to extract tag names (in a real implementation, use proper JSON parsing)
					var tagMatches = Regex.Matches(json, "\"tag_name\":\"([^\"]+)\"");
					_releases = new string[tagMatches.Count];

					for (var i = 0; i < tagMatches.Count; i++) _releases[i] = tagMatches[i].Groups[1].Value;

					if (_releases.Length > 0) _selectedReleaseIndex = 0;
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Error fetching releases: {e.Message}");
				_releases = new string[0];
			}
		}

		private void DownloadAndSetupDll()
		{
			if (_releases.Length == 0 || _selectedReleaseIndex >= _releases.Length)
			{
				Debug.LogError("No release selected");
				return;
			}

			try
			{
				// Extract owner and repo from URL
				var match = Regex.Match(_repositoryUrl, @"github\.com/([^/]+)/([^/]+)");
				if (!match.Success)
				{
					Debug.LogError("Invalid GitHub repository URL");
					return;
				}

				var owner = match.Groups[1].Value;
				var repo = match.Groups[2].Value;
				var release = _releases[_selectedReleaseIndex];

				// Ensure directory exists
				if (!Directory.Exists(_savePath)) Directory.CreateDirectory(_savePath);

				var fullPath = Path.Combine(_savePath, _dllName);
				var downloadUrl = $"https://github.com/{owner}/{repo}/releases/download/{release}/{_dllName}";

				// Download the file
				using (var client = new WebClient())
				{
					EditorUtility.DisplayProgressBar("Downloading DLL", "Downloading file from GitHub...", 0.5f);
					client.DownloadFile(new Uri(downloadUrl), fullPath);
				}

				EditorUtility.DisplayProgressBar("Processing DLL", "Refreshing Asset Database...", 0.7f);

				// Refresh the asset database to recognize the new file
				AssetDatabase.Refresh();

				// Apply the RoslynAnalyzer label if requested
				var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);

				if (asset != null)
				{
					// Apply the RoslynAnalyzer label
					AssetDatabase.SetLabels(asset, new[] { "RoslynAnalyzer" });
					AssetDatabase.SaveAssets();

					// Configure plugin settings for RoslynAnalyzer
					var importer = AssetImporter.GetAtPath(fullPath) as PluginImporter;
					if (importer != null)
					{
						// Disable for any platform
						importer.SetCompatibleWithAnyPlatform(false);

						// Disable for Editor and Standalone
						importer.SetCompatibleWithEditor(false);
						importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows,   false);
						importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
						importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64,   false);
						importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX,       false);

						importer.SaveAndReimport();
					}

					Debug.Log($"Successfully applied RoslynAnalyzer label to {_dllName}");
				}
				else
				{
					Debug.LogError($"Failed to load asset at path: {fullPath}");
				}

				EditorUtility.ClearProgressBar();
				Debug.Log($"Successfully downloaded {_dllName} to {_savePath}");
			}
			catch (Exception e)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogError($"Error downloading or setting up DLL: {e.Message}");
			}
		}
	}
}