using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace InfiniteCanvas.Utilities.Editor
{
    public class AddressableLabelUtility : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string  _targetGroup = "Default Local Group";
        private string  _targetLabel = "CharacterData";
        private string  _customFilter;
        private Type    _targetType;

    #region Event Functions

        private void OnGUI()
        {
            GUILayout.Label("ScriptableObject Labeling Settings", EditorStyles.boldLabel);

            DrawTypeSelection();
            DrawMainUI();
        }

    #endregion

        [MenuItem("Window/Asset Management/Addressables/Label By Type")]
        public static void ShowWindow() => GetWindow<AddressableLabelUtility>();

        private void DrawTypeSelection()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Selected Type:",
                                           _targetType != null ? _targetType.FullName : "None",
                                           EditorStyles.wordWrappedLabel);

                if (GUILayout.Button("Select Type", GUILayout.Width(100))) ShowTypePicker();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawMainUI()
        {
            _targetLabel = EditorGUILayout.TextField("Target Label", _targetLabel);
            _targetGroup = EditorGUILayout.TextField("Target Group", _targetGroup);

            _customFilter = EditorGUILayout.TextField(new GUIContent("Additional Filter",
                                                                     "Supports Unity search syntax:\n"
                                                                   + "- Name fragments: 'player'\n"
                                                                   + "- Labels: 'l:ui'\n"
                                                                   + "- References: 'ref:SceneName'"),
                                                      _customFilter);

            if (_targetType != null && GUILayout.Button("Apply Labels"))
                LabelAssetsOfType(_targetType, _targetLabel, _targetGroup, _customFilter);
            else if (_targetType == null)
                EditorGUILayout.HelpBox("Select a ScriptableObject type first", MessageType.Warning);
        }

        private void ShowTypePicker()
            => TypePickerWindow.ShowWindow(selectedType =>
                                           {
                                               if (selectedType != null)
                                                   _targetType = selectedType;
                                           },
                                           typeof(UnityEngine.Object));


        private static void LabelAssetsOfType(Type type, string label, string groupName, string customFilter)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Settings not found");
                return;
            }

            // Ensure label exists
            if (!settings.GetLabels().Contains(label)) settings.AddLabel(label);

            // Find target group
            var group = settings.groups.Find(g => g.Name == groupName);
            if (group == null)
            {
                Debug.LogError($"Group {groupName} not found");
                return;
            }

            // Find all assets of type
            var filter = $"t:{type.Name} {customFilter}";
            var guids = AssetDatabase.FindAssets(filter, new[] { "Assets" });
            Debug.Log($"Found {guids.Length} with filter {filter}");

            var processed = 0;
            var skipped = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

                if (asset == null) continue;

                // Get or create entry
                var entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, group);

                // Add label if missing
                if (!entry.labels.Contains(label))
                {
                    entry.labels.Add(label);
                    processed++;
                }
                else { skipped++; }
            }

            Debug.Log($"Labeled {processed} {type.Name} assets with '{label}', {skipped} skipped");
            AssetDatabase.SaveAssets();
        }


        public class TypePickerWindow : EditorWindow
        {
            private static Type         _baseType;
            private        List<Type>   _filteredTypes = new();
            private        Action<Type> _onTypeSelected;
            private        Vector2      _scrollPosition;
            private        string       _searchPattern = "";
            private        Type         _selectedType;

        #region Event Functions

            private void OnGUI()
            {
                DrawSearchBar();
                DrawTypeList();
            }

        #endregion

            public static void ShowWindow(Action<Type> callback, Type baseType, string initialSearch = "")
            {
                _baseType = baseType;
                var window = GetWindow<TypePickerWindow>();
                window.titleContent = new GUIContent("Type Picker");
                window._searchPattern = initialSearch;
                window._onTypeSelected = callback;
                window.minSize = new Vector2(300, 400);
                window.RefreshTypeList();
                window.Show();
            }

            private void DrawSearchBar()
            {
                EditorGUI.BeginChangeCheck();
                _searchPattern = EditorGUILayout.TextField("Filter:", _searchPattern);
                if (EditorGUI.EndChangeCheck()) RefreshTypeList();
            }

            private void DrawTypeList()
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                foreach (var type in _filteredTypes)
                    if (GUILayout.Button(type.FullName, EditorStyles.miniButton))
                    {
                        _onTypeSelected?.Invoke(type);
                        Close();
                    }

                EditorGUILayout.EndScrollView();
            }

            private void RefreshTypeList()
                => _filteredTypes = GetFilteredTypes(_baseType)
                                   .OrderBy(t => t.Namespace)
                                   .ThenBy(t => t.Name)
                                   .ToList();

            private IEnumerable<Type> GetFilteredTypes(Type baseType)
            {
                var regex = new Regex(_searchPattern, RegexOptions.IgnoreCase);
                return AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .SelectMany(a => a.GetExportedTypes())
                                .Where(t => t.IsClass
                                         && !t.IsAbstract
                                         && baseType.IsAssignableFrom(t)
                                         && regex.IsMatch(t.FullName))
                                .Distinct();
            }
        }
    }
}